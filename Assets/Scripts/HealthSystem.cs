using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private bool _destroyOnDeath = true;

    [Header("Armor Settings")]
    [SerializeField] private float _maxArmor = 100;
    [SerializeField] private float _currentArmor = 0f;
    public float CurrentArmor => _currentArmor;

    [Header("Visual Feedback")]
    [SerializeField] private Color _damageColor = Color.red;
    private List<SpriteRenderer> parts = new List<SpriteRenderer>();

    private List<Transform> renderedPart = new List<Transform>();
    private List<Color> partOriginalColor = new List<Color>();

    private float _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float MaxArmor => _maxArmor;

    private PhotonView view;

    // Effect

    private bool isDamaged = false;
    private float timer = 0f;
    private float duration = 0.2f;

    private void Awake()
    {
        view = GetComponent<PhotonView>();

        foreach (Transform i in transform.Find("Model").transform)
        {
            renderedPart.Add(i);
            parts.Add(i.GetComponent<SpriteRenderer>());
            partOriginalColor.Add(i.GetComponent<SpriteRenderer>().color);
        }
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        if (isDamaged)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            for (int i = 0; i < renderedPart.Count; i++)
            {
                Color lerpColor = Color.Lerp(_damageColor, partOriginalColor[i], t);

                parts[i].color = lerpColor;
            }

            if (timer >= duration)
            {
                timer = 0f;
                isDamaged = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        float damageDeal = (float)damage;

        if (_currentArmor > 0)
        {
            if (_currentArmor > damageDeal)
            {
                _currentArmor -= damageDeal;
                UpdateStats();
                return;
            }
            else
            {
                damageDeal -= _currentArmor;
                _currentArmor = 0f;
            }
        }

        if (_currentHealth <= 0) return;
        view.RPC("RPC_FlashEffect", RpcTarget.All);

        _currentHealth = Mathf.Max(0, _currentHealth - damageDeal);
        UpdateStats();

    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        UpdateStats();
    }

    [PunRPC]
    public void RPC_UpdateStats(float health, float armor)
    {
        _currentHealth = health;
        _currentArmor = armor;
    }

    public void UpdateStats()
    {
        view.RPC("RPC_UpdateStats", RpcTarget.Others, CurrentHealth, _currentArmor);
    }

    public void AddArmor(int amount)
    {
        _currentArmor = Mathf.Min(_maxArmor, _currentArmor + amount);
        UpdateStats();
    }

    public void SetHealth(int amount)
    {
        _currentHealth = amount;
        UpdateStats();
    }

    public void SetArmor(int amount)
    {
        _currentArmor = amount;
        UpdateStats();
    }

    [PunRPC]
    public void RPC_FlashEffect()
    {
        timer = 0f;
        isDamaged = true;

        foreach (SpriteRenderer part in parts)
        {
            part.color = _damageColor;
        }
    }

    private IEnumerator FlashEffect()
    {
        foreach (Transform i in renderedPart)
        {
            i.GetComponent<SpriteRenderer>().color = _damageColor;
        }

        yield return new WaitForSeconds(0.25f);

        for (int i=0; i<renderedPart.Count; i++)
        {
            renderedPart[i].GetComponent<SpriteRenderer>().color = partOriginalColor[i];
        }
    }
}