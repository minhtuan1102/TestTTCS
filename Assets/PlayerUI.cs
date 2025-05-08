using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject Current_Player;

    [Header("UI Bar")]

    [SerializeField] GameObject HP_UI;
    [SerializeField] GameObject MP_UI;
    [SerializeField] GameObject AP_UI;

    [Header("UI Inventory")]

    [SerializeField] GameObject Storage;
    [SerializeField] GameObject Button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    HealthSystem health;
    Player player;

    void Start()
    {
        health = Current_Player.GetComponent<HealthSystem>();
        player = Current_Player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP_UI != null)
        {
            Transform bar = HP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)health.CurrentHealth / (float)health.MaxHealth);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = HP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{health.CurrentHealth}/{health.MaxHealth}";
        }

        if (AP_UI != null)
        {
            Transform bar = AP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)health.CurrentArmor / (float)health.MaxArmor);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = AP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{health.CurrentArmor}/{health.MaxArmor}";
        }

        if (MP_UI != null)
        {
            Transform bar = MP_UI.transform.Find("Hider");
            RectTransform hider = bar.GetComponent<RectTransform>();
            float scale = Mathf.Clamp01((float)player.CurrentMana / (float)player.MaxMana);
            hider.localScale = new Vector3(1f - scale, 1, 1);

            Transform display = MP_UI.transform.Find("Text");
            TMP_Text text = display.GetComponent<TMP_Text>();
            text.text = $"{player.CurrentMana}/{player.MaxMana}";
        }
    }
}
