using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviourPun, IPunObservable
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private bool _destroyOnDeath = true;

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
        _currentHealth = _maxHealth;

        InitializeUI();
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        UpdateUI();
        StartCoroutine(FlashEffect());

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        UpdateUI();
    }

    private void Die()
    {
        photonView.RPC("ShowDeathEffect", RpcTarget.All, transform.position);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOver();
        }
    }

    [PunRPC]
    void ShowDeathEffect(Vector3 position)
    {
        if (_deathEffect != null)
        {
            Instantiate(_deathEffect, position, Quaternion.identity);
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
            _healthCanvas.transform.rotation = Quaternion.identity;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
        }
        else
        {
            _currentHealth = (int)stream.ReceiveNext();
            UpdateUI();
        }
    }
}