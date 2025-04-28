using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(Player))]
public class PlayerStamina : MonoBehaviourPunCallbacks
{
    public Slider staminaBar; // Thanh UI hiển thị stamina
    public TextMeshProUGUI staminaText; // Text hiển thị giá trị stamina
    public float maxStamina = 100f; // Stamina tối đa
    public float currentStamina; // Stamina hiện tại
    public float dashStaminaCost = 20f; // Lượng stamina tiêu thụ khi dash
    public float minStaminaToDash = 30f; // Stamina tối thiểu để dash
    public float staminaRegenRate = 10f; // Tốc độ hồi stamina mỗi giây

    private bool canDash = true; // Trạng thái cho phép dash
    private bool isDashLocked = false; // Khóa dash nếu stamina không đủ
    private Player player; // Tham chiếu đến component Player

    void Start()
    {
        // Lấy component Player
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found on GameObject!");
        }

        // Kiểm tra staminaBar và staminaText
        if (staminaBar == null)
        {
            Debug.LogWarning("StaminaBar Slider not assigned in Inspector!");
        }
        if (staminaText == null)
        {
            Debug.LogWarning("StaminaText TextMeshProUGUI not assigned in Inspector!");
        }

        // Khởi tạo stamina
        currentStamina = maxStamina;
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }
        UpdateStaminaUI();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Kiểm tra điều kiện khóa dash
        isDashLocked = currentStamina < minStaminaToDash || currentStamina < dashStaminaCost;
    }

    // Tiêu thụ stamina
    public void ConsumeStamina(float amount)
    {
        if (!photonView.IsMine) return;

        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0);
        photonView.RPC("RPC_UpdateStamina", RpcTarget.AllBuffered, currentStamina);
    }

    // Cập nhật UI stamina
    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }
        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.RoundToInt(currentStamina)}/{Mathf.RoundToInt(maxStamina)}";
        }
    }

    [PunRPC]
    void RPC_UpdateStamina(float newStamina)
    {
        currentStamina = newStamina;
        UpdateStaminaUI();
    }

    // Kiểm tra xem có thể dash được không
    public bool CanDash()
    {
        return canDash && !isDashLocked && currentStamina >= dashStaminaCost && currentStamina >= minStaminaToDash;
    }

    // Đặt lại khả năng dash sau cooldown
    public void ResetDash()
    {
        canDash = true;
    }
}