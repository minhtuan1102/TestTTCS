using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public enum HealthType
{
<<<<<<< Updated upstream
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
=======

    private List<Transform> renderedPart = new List<Transform>();
    private List<Color> partOriginalColor = new List<Color>();

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private bool _destroyOnDeath = true;
>>>>>>> Stashed changes

    [Header("Armor Settings")]
    [SerializeField] private float _maxArmor = 100;
    [SerializeField] private float _currentArmor = 0f;
    public float CurrentArmor => _currentArmor;

    [Header("Visual Feedback")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private GameObject deathEffect;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Canvas healthCanvas;

<<<<<<< Updated upstream
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
=======
    private float _currentHealth;
    private Transform _model;
    private Color _originalColor;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float MaxArmor => _maxArmor;

    private void Awake()
    {
        foreach(Transform i in transform.Find("Model").transform)
        {
            renderedPart.Add(i);
            partOriginalColor.Add(i.GetComponent<SpriteRenderer>().color);
        }
        _currentHealth = _maxHealth;
>>>>>>> Stashed changes
    }

    public void TakeDamage(int amount)
    {
<<<<<<< Updated upstream
        // Đã bỏ kiểm tra photonView.IsMine
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, currentHealth);

=======

        float damageDeal = (float)damage;

        if (_currentArmor > 0)
        {
            if (_currentArmor > damageDeal)
            {
                _currentArmor -= damageDeal;
                return;
            } else
            {
                damageDeal -= _currentArmor;
                _currentArmor = 0f;
            }
        }

        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
    
>>>>>>> Stashed changes
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
            photonView.RPC(nameof(RPC_Die), RpcTarget.All);
    }

    public void Heal(int amount)
    {
<<<<<<< Updated upstream
        if (!photonView.IsMine || !PhotonNetwork.IsConnectedAndReady) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.AllBuffered, currentHealth);
    }

    [PunRPC]
    private void RPC_UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        UpdateUI();
=======
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    public void AddArmor(int amount)
    {
        _currentArmor = Mathf.Min(_maxArmor, _currentArmor + amount);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
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
=======
        foreach (Transform i in renderedPart)
        {
            i.GetComponent<SpriteRenderer>().color = _damageColor;
        }
        yield return new WaitForSeconds(_flashDuration);

        for (int i=0; i<renderedPart.Count; i++)
        {
            renderedPart[i].GetComponent<SpriteRenderer>().color = partOriginalColor[i];
        }
>>>>>>> Stashed changes
    }
}
