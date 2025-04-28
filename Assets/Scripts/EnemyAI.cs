using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// Yêu cầu các component cần thiết
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthSystem))]
public class EnemyAI : MonoBehaviourPunCallbacks, IPunObservable
{
    // Biến lưu vị trí mục tiêu và Transform của mục tiêu (người chơi)
    private Vector3 targetPos;
    private Transform target;

    // Component NavMeshAgent để điều khiển di chuyển
    private NavMeshAgent agent;
    // LayerMask để kiểm tra chướng ngại vật khi phát hiện người chơi
    public LayerMask obstacleMask;
    // ScriptableObject chứa dữ liệu cấu hình của enemy
    public EnemyData data;

    // Các bộ đếm thời gian để quản lý hành vi
    private float chaseTimer = 0f; // Thời gian đuổi theo
    private float detectTimer = 0f; // Thời gian phát hiện
    private float attackTimer = 0f; // Thời gian giữa các lần tấn công
    private float waitingTimer = 0f; // Thời gian chờ sau khi tấn công

    // Component HealthSystem của enemy
    private HealthSystem healthSystem;

    // Vị trí mạng để đồng bộ hóa qua Photon
    private Vector3 networkPosition;

    // Khởi tạo
    void Start()
    {
        // Lấy component NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // Tắt cập nhật rotation và upAxis vì sử dụng 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // Thiết lập tốc độ di chuyển từ dữ liệu
        agent.speed = data.Speed;
        // Lấy component HealthSystem
        healthSystem = GetComponent<HealthSystem>();
    }

    // Tìm người chơi gần nhất trong tầm nhìn
    private Transform FindPlayer()
    {
        Transform detectedPlayer = null;
        float minDistance = Mathf.Infinity;
        // Tìm tất cả người chơi có tag "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            // Tính hướng và khoảng cách đến người chơi
            Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Sử dụng Raycast để kiểm tra chướng ngại vật
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask);

            // Nếu không có chướng ngại vật, kiểm tra khoảng cách
            if (hit.collider == null)
            {
                if (minDistance > distanceToPlayer)
                {
                    minDistance = distanceToPlayer;
                    detectedPlayer = player.transform;
                }
            }
        }
        return detectedPlayer;
    }

    // Kiểm tra xem enemy có gần một vị trí cụ thể trong bán kính cho trước không
    private bool CheckIfNear(Vector3 pos, float radius)
    {
        return Vector3.Distance(transform.position, pos) <= radius;
    }

    // Cập nhật logic mỗi frame
    private void Update()
    {
        // Nếu không phải là instance chính (không phải owner), đồng bộ vị trí
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            return;
        }

        // Cập nhật các bộ đếm thời gian
        detectTimer += Time.fixedDeltaTime;
        attackTimer += Time.fixedDeltaTime;
        chaseTimer += Time.fixedDeltaTime;
        waitingTimer += Time.fixedDeltaTime;

        // Nếu đang trong thời gian chờ, không làm gì
        if (waitingTimer < 0f)
        {
            return;
        }

        // Phát hiện người chơi
        if (detectTimer >= 0f)
        {
            Transform detected = FindPlayer();
            if (detected != null && CheckIfNear(detected.position, data.Detection_Range))
            {
                target = detected;
                targetPos = detected.position;
                chaseTimer = -data.Chasing_Time_Threshold;
            }
            else
            {
                if (chaseTimer >= 0f)
                {
                    target = null;
                }
            }
            detectTimer = -data.Detection_Rate;
        }

        // Logic tấn công
        if (attackTimer >= data.Attack_Rate && target != null)
        {
            if (CheckIfNear(target.position, data.Range))
            {
                // Đặt lại bộ đếm thời gian tấn công và chờ
                attackTimer = 0f;
                waitingTimer = -data.WaitTime;
                // Gọi RPC để thực hiện tấn công
                photonView.RPC("RPC_TriggerAttack", RpcTarget.All, data.Damage, target.gameObject.GetPhotonView().ViewID);
                // Dừng di chuyển khi tấn công
                agent.SetDestination(transform.position);
            }
        }

        // Di chuyển đến mục tiêu nếu không trong thời gian chờ
        if (waitingTimer >= 0f && target != null)
        {
            agent.SetDestination(targetPos);
        }
    }

    // RPC để thực hiện tấn công
    [PunRPC]
    void RPC_TriggerAttack(float damage, int targetViewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.LogWarning("Target PhotonView not found!");
            return;
        }

        HealthSystem targetHealth = targetPhotonView.gameObject.GetComponent<HealthSystem>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage((int)damage); // Gọi TakeDamage trực tiếp
            Debug.Log($"Enemy attacked player {targetPhotonView.gameObject.name} for {damage} damage.");
        }
        else
        {
            Debug.LogWarning("Target does not have HealthSystem component!");
        }
    }

    // Đồng bộ hóa qua Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(healthSystem.CurrentHealth);
            stream.SendNext(target != null ? target.position : Vector3.zero); // Đồng bộ vị trí mục tiêu
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            int receivedHealth = (int)stream.ReceiveNext();
            Vector3 targetPos = (Vector3)stream.ReceiveNext();
            healthSystem.TakeDamage(healthSystem.CurrentHealth - receivedHealth);
            if (targetPos != Vector3.zero)
                this.targetPos = targetPos;
        }
    }
}