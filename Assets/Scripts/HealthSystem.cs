using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

public enum HealthType
{
    Player,
    Enemy
}

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviourPunCallbacks
{
    [Header("Type Settings")]
    [SerializeField] private HealthType healthType;
    [SerializeField] private EnemyData enemyData; // Chỉ dùng nếu là Enemy

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Visual Feedback")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private GameObject deathEffect;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Canvas healthCanvas;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (healthCanvas == null)
            healthCanvas = GetComponentInChildren<Canvas>();

        if (healthType == HealthType.Enemy && enemyData != null)
            maxHealth = (int)enemyData.Health;

        currentHealth = maxHealth;

        InitializeUI();
    }

    public void TakeDamage(int amount)
    {
        // Đã bỏ kiểm tra photonView.IsMine
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, currentHealth);

        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
    }

    public void Heal(int amount)
    {
        if (!photonView.IsMine || !PhotonNetwork.IsConnectedAndReady) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, currentHealth);
    }

    [PunRPC]
    private void RPC_UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        UpdateUI();
    }

    [PunRPC]
    private void RPC_Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        switch (healthType)
        {
            case HealthType.Enemy:
                SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
                spawnManager?.EnemyDefeated(gameObject);
                break;
            case HealthType.Player:
                if (GameManager.Instance != null && photonView.IsMine)
                    GameManager.Instance.ShowGameOver();
                break;
        }

        if (destroyOnDeath)
        {
            Destroy(healthCanvas?.gameObject);
            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void InitializeUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    private void LateUpdate()
    {
        if (healthCanvas != null && Camera.main != null)
            healthCanvas.transform.rotation = Camera.main.transform.rotation;
    }
}
