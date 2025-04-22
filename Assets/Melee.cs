using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class Melee : MonoBehaviourPun
{
    private Collider2D attackCollider;
    private LayerMask enemyLayer;
    private LayerMask playerLayer;
    private Rigidbody2D rb;
    private float currentDamage; // Lưu giá trị sát thương hiện tại
    private bool hasDealtDamage; // Đảm bảo chỉ gây sát thương một lần mỗi lần tấn công

    private void Start()
    {
        // Lấy Rigidbody2D và đặt là Kinematic để không bị ảnh hưởng bởi vật lý
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        // Tìm Hitbox và lấy Collider
        Transform hitbox = transform.Find("Hitbox");
        if (hitbox != null)
        {
            attackCollider = hitbox.GetComponent<PolygonCollider2D>();
            if (attackCollider == null)
            {
                Debug.LogWarning($"Không tìm thấy PolygonCollider2D trên Hitbox của {gameObject.name}!");
            }
            else
            {
                // Bật Is Trigger để dùng OnTriggerEnter2D
                attackCollider.isTrigger = true;
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy Hitbox trên {gameObject.name}!");
        }

        // Thiết lập LayerMask
        int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
        int playerLayerIndex = LayerMask.NameToLayer("Player");

        if (enemyLayerIndex == -1)
        {
            Debug.LogError("Layer 'Enemy' không tồn tại! Vui lòng kiểm tra Layers trong Unity.");
        }
        else
        {
            enemyLayer = 1 << enemyLayerIndex;
        }

        if (playerLayerIndex == -1)
        {
            Debug.LogError("Layer 'Player' không tồn tại! Vui lòng kiểm tra Layers trong Unity.");
        }
        else
        {
            playerLayer = 1 << playerLayerIndex;
        }
    }

    public void TriggerAttack(float damage)
    {
        // Gửi RPC để đồng bộ hành động tấn công
        photonView.RPC("RPC_TriggerSwing", RpcTarget.All);
        Debug.Log("Gửi RPC_TriggerSwing để đồng bộ tấn công cận chiến.");

        // Kiểm tra xem attackCollider có hợp lệ không
        if (attackCollider == null)
        {
            Debug.LogWarning($"Không thể tấn công: attackCollider chưa được thiết lập trên {gameObject.name}!");
            return;
        }

        // Lưu giá trị sát thương và reset trạng thái gây sát thương
        currentDamage = damage;
        hasDealtDamage = false;

        // Bật Collider để phát hiện va chạm
        attackCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ client sở hữu mới xử lý va chạm
        if (!photonView.IsMine)
        {
            return;
        }

        // Nếu đã gây sát thương rồi, không xử lý thêm
        if (hasDealtDamage)
        {
            return;
        }

        if (gameObject.CompareTag("Player") && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            PhotonView targetPhotonView = other.GetComponent<PhotonView>();
            if (targetPhotonView == null)
            {
                Debug.Log($"Đối tượng {other.name} không có PhotonView!");
                return;
            }

            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                // Gửi RPC tới client sở hữu kẻ địch để gây sát thương
                photonView.RPC("RPC_DealDamage", targetPhotonView.Owner, targetPhotonView.ViewID, (int)currentDamage);
                Debug.Log($"Gửi RPC_DealDamage tới {other.name} với sát thương: {currentDamage}");
                hasDealtDamage = true; // Đánh dấu đã gây sát thương
            }
            else
            {
                Debug.Log($"Đối tượng {other.name} không có HealthSystem!");
            }
        }
        else if (gameObject.CompareTag("Enemy") && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PhotonView targetPhotonView = other.GetComponent<PhotonView>();
            if (targetPhotonView == null)
            {
                Debug.Log($"Đối tượng {other.name} không có PhotonView!");
                return;
            }

            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                // Gửi RPC tới client sở hữu người chơi để gây sát thương
                photonView.RPC("RPC_DealDamage", targetPhotonView.Owner, targetPhotonView.ViewID, (int)currentDamage);
                Debug.Log($"Gửi RPC_DealDamage tới {other.name} với sát thương: {currentDamage}");
                hasDealtDamage = true; // Đánh dấu đã gây sát thương
            }
            else
            {
                Debug.Log($"Đối tượng {other.name} không có HealthSystem!");
            }
        }

        // Tắt Collider sau khi va chạm
        attackCollider.enabled = false;
    }

    [PunRPC]
    void RPC_TriggerSwing()
    {
        Debug.Log($"Nhận RPC_TriggerSwing trên {gameObject.name}. Đồng bộ hành động tấn công cận chiến.");
        // Thêm logic để đồng bộ hoạt hình hoặc hiệu ứng tấn công nếu cần
    }

    [PunRPC]
    void RPC_DealDamage(int targetViewID, int damage)
    {
        // Tìm đối tượng dựa trên ViewID
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.Log($"Không tìm thấy đối tượng với ViewID {targetViewID}!");
            return;
        }

        // Chỉ client sở hữu mới thực hiện TakeDamage
        if (!targetPhotonView.IsMine)
        {
            Debug.Log($"Client không sở hữu đối tượng {targetPhotonView.gameObject.name}, bỏ qua TakeDamage.");
            return;
        }

        HealthSystem healthSystem = targetPhotonView.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
            Debug.Log($"Đã gây sát thương {damage} cho {targetPhotonView.gameObject.name}!");
        }
        else
        {
            Debug.Log($"Đối tượng {targetPhotonView.gameObject.name} không có HealthSystem!");
        }
    }
}