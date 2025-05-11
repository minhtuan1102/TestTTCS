using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;

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

    private List<Transform> renderedPart = new List<Transform>();
    private List<Color> partOriginalColor = new List<Color>();

    private float _currentHealth;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float MaxArmor => _maxArmor;

    private void Awake()
    {
        foreach (Transform i in transform.Find("Model").transform)
        {
            renderedPart.Add(i);
            partOriginalColor.Add(i.GetComponent<SpriteRenderer>().color);
        }
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        float damageDeal = (float)damage;

        if (_currentArmor > 0)
        {
            if (_currentArmor > damageDeal)
            {
                _currentArmor -= damageDeal;
                return;
            }
            else
            {
                damageDeal -= _currentArmor;
                _currentArmor = 0f;
            }
        }

        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damageDeal);
        StartCoroutine(FlashEffect());
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    public void AddArmor(int amount)
    {
        _currentArmor = Mathf.Min(_maxArmor, _currentArmor + amount);
    }

    public void SetHealth(int amount)
    {
        _currentHealth = amount;
    }

    public void SetArmor(int amount)
    {
        _currentArmor = amount;
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