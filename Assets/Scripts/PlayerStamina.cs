    using UnityEngine;
    using UnityEngine.UI; // Để sử dụng Slider
    using TMPro; // Để sử dụng TextMeshPro

    public class PlayerStamina : MonoBehaviour
    {
        public Slider staminaBar; // Thanh thể lực (Slider)
        public TextMeshProUGUI staminaText; // Text để hiển thị chỉ số thể lực
        public float maxStamina = 100f; // Thể lực tối đa
        public float currentStamina; // Thể lực hiện tại
        public float dashStaminaCost = 20f; // Lượng thể lực tiêu hao khi lướt
        public float minStaminaToDash = 30f; // Mức thể lực tối thiểu để có thể lướt

        private bool canDash = true; // Kiểm tra xem có thể lướt được không
        private bool isDashLocked = false; // Biến mới để khóa lướt khi thể lực không đủ

        void Start()
        {
            currentStamina = maxStamina; // Khởi tạo thể lực đầy
            staminaBar.maxValue = maxStamina; // Đặt giá trị tối đa cho Slider
            staminaBar.value = currentStamina; // Cập nhật giá trị ban đầu
            UpdateStaminaUI(); // Cập nhật giao diện ban đầu
        }

        void Update()
        {
            // Kiểm tra thể lực để quyết định có khóa lướt hay không
            if (currentStamina < minStaminaToDash || currentStamina < dashStaminaCost)
            {
                isDashLocked = true; // Khóa lướt nếu thể lực không đủ
            }
            else
            {
                isDashLocked = false; // Mở khóa lướt nếu thể lực đủ
            }

            // Kiểm tra input để lướt (sử dụng phím Q)
            if (Input.GetKeyDown(KeyCode.Q) && canDash && !isDashLocked)
            {
                if (currentStamina >= minStaminaToDash && currentStamina >= dashStaminaCost)
                {
                    Dash();
                }
   
            }
        }

        void Dash()
        {
            // Giảm thể lực khi lướt
            currentStamina -= dashStaminaCost;
            currentStamina = Mathf.Max(currentStamina, 0); // Đảm bảo không âm
            UpdateStaminaUI(); // Cập nhật giao diện

            // Thực hiện hành động lướt (dash) ở đây
            Debug.Log("Player dashed!");

            // Tạm thời khóa khả năng lướt (cooldown)
            canDash = false;
            Invoke("ResetDash", 1f); // Sau 1 giây có thể lướt lại
        }

        void ResetDash()
        {
            canDash = true;
        }

        public void RestoreStamina(float amount)
        {
            // Hồi phục thể lực
            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Giới hạn giá trị từ 0 đến maxStamina
            UpdateStaminaUI(); // Cập nhật giao diện
        }

        void UpdateStaminaUI()
        {
            // Cập nhật giá trị thanh thể lực
            staminaBar.value = currentStamina;

            // Cập nhật chỉ số thể lực dạng "current/max"
            if (staminaText != null)
            {
                staminaText.text = Mathf.RoundToInt(currentStamina) + "/" + Mathf.RoundToInt(maxStamina);
            }
        }

    
    }