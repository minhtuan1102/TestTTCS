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
    [SerializeField] private float _maxArmor = 0f;
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

    public float _defense = 0f;

    private PhotonView view;

    // Effect

    private bool isDamaged = false;
    private float timer = 0f;
    private float duration = 0.2f;

    private float armorCD = 0f;
    private float _armorGain = 0f;

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
        armorCD += Time.fixedDeltaTime;

        if (isDamaged)
        {
            timer += Time.fixedDeltaTime;
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

        if (_currentArmor < _maxArmor)
        {
            if (armorCD >= 0f)
            {
                armorCD -= 2f;
                AddArmor((int)_armorGain);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        float damageDeal = Mathf.Max(1, (float)damage - _defense);
        armorCD = -10f;
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

    public void calculateArmor(List<ItemInstance> items)
    {
        float maxArmor = 50f;
        float regenArmor = 0f;
        float defense = 0f;
        foreach (ItemInstance item in items)
        {
            if (item != null && item.itemRef != null)
            {
                maxArmor += item.itemRef.armor;
                regenArmor += item.itemRef.armor_regen;
                defense += item.itemRef.defense;
            }
        }

        _maxArmor = maxArmor;
        _defense = defense;
        _armorGain = regenArmor;

        _currentArmor = Mathf.Min(_currentArmor, _maxArmor);
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

    public void SetMaxHealth(int amount)
    {
        _maxHealth = amount;
        SetHealth(amount);
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