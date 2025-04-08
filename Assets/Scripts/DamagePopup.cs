using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        // Tạo prefab DamagePopup từ Resources
        GameObject damagePopupPrefab = Resources.Load<GameObject>("DamagePopup");
        GameObject damagePopupInstance = Instantiate(damagePopupPrefab, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupInstance.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = 1f; // Thời gian hiển thị
    }

    private void Update()
    {
        // Di chuyển popup lên trên
        transform.position += new Vector3(0, 1f) * Time.deltaTime;

        // Hiệu ứng biến mất
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Mờ dần
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}