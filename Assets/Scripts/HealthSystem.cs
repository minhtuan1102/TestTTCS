using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private bool _destroyOnDeath = true;
    [SerializeField] private EnemyData enemyData; // Tùy chọn: lấy máu từ EnemyData

    [Header("Visual Feedback")]
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private GameObject _deathEffect;

    [Header("UI References (Set in Editor)")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private Canvas _healthCanvas;

    private int _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;

        // Tự tìm healthCanvas nếu chưa gán
        if (_healthCanvas == null)
        {
            _healthCanvas = GetComponentInChildren<Canvas>();
        }

        // Lấy maxHealth từ EnemyData nếu có
        _maxHealth = enemyData != null ? (int)enemyData.Health : _maxHealth;
        _currentHealth = _maxHealth;

        InitializeUI();
    }


    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine || !PhotonNetwork.IsConnectedAndReady) return;

        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        photonView.RPC("RPC_UpdateHealth", RpcTarget.AllBuffered, _currentHealth);
        StartCoroutine(FlashEffect());

        if (_currentHealth <= 0)
        {
            photonView.RPC("RPC_Die", RpcTarget.All);
        }
    }

    public void Heal(int amount)
    {
        if (!photonView.IsMine || !PhotonNetwork.IsConnectedAndReady) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        photonView.RPC("RPC_UpdateHealth", RpcTarget.AllBuffered, _currentHealth);
    }

    [PunRPC]
    void RPC_UpdateHealth(int newHealth)
    {
        _currentHealth = newHealth;
        UpdateUI();
    }

    [PunRPC]
    void RPC_Die()
    {
        if (_deathEffect != null)
        {
            Instantiate(_deathEffect, transform.position, Quaternion.identity);
        }

        if (gameObject.CompareTag("Enemy"))
        {
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            spawnManager?.EnemyDefeated(gameObject);
        }

        if (_destroyOnDeath)
        {
            Destroy(_healthCanvas?.gameObject);
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (GameManager.Instance != null && photonView.IsMine && gameObject.CompareTag("Player"))
        {
            GameManager.Instance.ShowGameOver();
        }
    }

    private IEnumerator FlashEffect()
    {
        _spriteRenderer.color = _damageColor;
        yield return new WaitForSeconds(_flashDuration);
        _spriteRenderer.color = _originalColor;
    }

    private void InitializeUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0;
            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _currentHealth;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _currentHealth;
        }

        if (_healthText != null)
        {
            _healthText.text = $"{_currentHealth}/{_maxHealth}";
        }
    }

    private void LateUpdate()
    {
        if (_healthCanvas != null)
        {
            _healthCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }
}