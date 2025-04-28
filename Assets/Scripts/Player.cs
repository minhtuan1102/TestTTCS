using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(PlayerStamina))]
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public float range = 5f;
    private float vision = 5f;
    public float minVision = -2f;
    public float maxVision = 2f;
    public float zoomSpeed = 5f;
    public float moveZoomDecrease = 2f;

    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 10f;
    public float linearDrag = 4f;
    private Vector3 moveDirrection;

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f; // Thời gian chờ giữa các lần dash
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTimer;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 movement;
    private float targetVision;

    private SpriteRenderer skin;
    private float cellSize = 1f;

    private GameObject main_hand;
    public float Hand_Radius = 0.5f;
    private float rotationSpeed = 10f;

    public float recoilForce = 2f;
    public float recoilRecoverySpeed = 5f;
    public float dampingFactor = 0.1f;
    private float recoilOffset = 0f;
    private float recoilVelocity = 0f;

    public float swingSpeed = 10f;
    public float swingOffset = 30f;
    public float swingRecoil = -90f;
    private float currentSwing = 0f;
    private float targetSwing = 0f;
    private float inverse = 1f;

    private Collider2D player_collider;
    private HealthSystem healthSystem;
    private PlayerStamina staminaSystem;

    private Vector3 networkPosition;
    private Quaternion networkHandRotation;
    private Vector3 networkScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skin = GetComponent<SpriteRenderer>();
        player_collider = GetComponent<BoxCollider2D>();
        main_hand = transform.Find("Main").gameObject;
        healthSystem = GetComponent<HealthSystem>();
        staminaSystem = GetComponent < PlayerStamina>();

        // Kiểm tra các component cần thiết
        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D không được tìm thấy trên {gameObject.name}!");
        }
        if (skin == null)
        {
            Debug.LogError($"SpriteRenderer không được tìm thấy trên {gameObject.name}!");
        }
        if (player_collider == null)
        {
            Debug.LogError($"BoxCollider2D không được tìm thấy trên {gameObject.name}!");
        }
        if (main_hand == null)
        {
            Debug.LogError("Main hand GameObject không được tìm thấy! Vui lòng đảm bảo có GameObject con tên 'Main'.");
        }
        if (healthSystem == null)
        {
            Debug.LogError($"HealthSystem không được tìm thấy trên {gameObject.name}!");
        }
        if (staminaSystem == null)
        {
            Debug.LogError($"PlayerStamina không được tìm thấy trên {gameObject.name}!");
        }

        // Kiểm tra Tilemap
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        if (tilemap != null)
        {
            cellSize = tilemap.layoutGrid.cellSize.x;
        }
        else
        {
            Debug.LogWarning("Tilemap không được tìm thấy trong scene. Sử dụng cellSize mặc định.");
        }

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.linearDamping = linearDrag;
        cam = Camera.main;
        targetVision = vision;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Xử lý input di chuyển
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Xử lý input dash
        dashCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && !isDashing && dashCooldownTimer <= 0f && staminaSystem.CanDash())
        {
            moveDirrection = movement.magnitude > 0 ? movement.normalized : new Vector2(inverse, 0);
            staminaSystem.ConsumeStamina(staminaSystem.dashStaminaCost);
            photonView.RPC("RPC_StartDash", RpcTarget.All);
            dashCooldownTimer = dashCooldown;
            staminaSystem.ResetDash(); // Đảm bảo canDash được đặt lại sau cooldown
        }

        // Xử lý input tấn công
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            FireBullet fireBullet = main_hand.GetComponent<FireBullet>();
            Melee melee = main_hand.GetComponent<Melee>();
            if (fireBullet != null)
            {
                photonView.RPC("RPC_Shoot", RpcTarget.All, fireDirection, 20f, 5f, 1);
            }
            else if (melee != null)
            {
                photonView.RPC("RPC_MeleeAttack", RpcTarget.All, 20f);
            }
        }

        // Chuẩn hóa vector di chuyển
        if (movement.magnitude > 1)
            movement.Normalize();

        // Điều chỉnh tầm nhìn camera
        if (movement.magnitude > 0.1f)
        {
            targetVision = Mathf.Max(range + minVision, range + maxVision - moveZoomDecrease);
        }
        else
        {
            targetVision = range + maxVision;
        }

        vision = Mathf.Lerp(vision, targetVision * cellSize, Time.deltaTime * zoomSpeed);

        // Cập nhật camera
        if (cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, vision, Time.deltaTime * zoomSpeed);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, vision * zoomSpeed, Time.deltaTime * zoomSpeed);

        Vector3 targetCamPos = new Vector3(rb.position.x, rb.position.y, -10);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetCamPos, Time.deltaTime * 3f);
    }

    public void TriggerSwing(float angle)
    {
        targetSwing = angle;
        photonView.RPC("RPC_TriggerSwing", RpcTarget.All, angle);
    }

    public void TriggerRecoil(float intensity)
    {
        recoilOffset = recoilForce * intensity;
        photonView.RPC("RPC_TriggerRecoil", RpcTarget.All, intensity);
    }

    [PunRPC]
    void RPC_Shoot(Vector2 fireDirection, float damage, float spread, int fireAmount)
    {
        FireBullet fireBullet = main_hand.GetComponent<FireBullet>();
        if (fireBullet != null)
        {
            fireBullet.Shoot(damage, spread, fireAmount);
            TriggerRecoil(1f);
            TriggerSwing(swingOffset);
        }
    }

    [PunRPC]
    void RPC_MeleeAttack(float damage)
    {
        Melee melee = main_hand.GetComponent<Melee>();
        if (melee != null)
        {
            melee.TriggerAttack(damage);
            TriggerRecoil(1f);
            TriggerSwing(swingOffset);
        }
    }

    [PunRPC]
    void RPC_TriggerSwing(float angle)
    {
        targetSwing = angle;
    }

    [PunRPC]
    void RPC_TriggerRecoil(float intensity)
    {
        recoilOffset = recoilForce * intensity;
    }

    [PunRPC]
    public void RPC_StartDash()
    {
        isDashing = true;
        dashTime = 0;
    }

    [PunRPC]
    public void RPC_EndDash()
    {
        isDashing = false;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (isDashing)
            {
                rb.linearVelocity = moveDirrection * dashSpeed;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

                dashTime += Time.fixedDeltaTime;
                if (dashTime >= dashDuration)
                {
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
                    isDashing = false;
                    rb.linearVelocity = Vector2.zero;
                    photonView.RPC("RPC_EndDash", RpcTarget.All);
                }
            }

            if (movement.magnitude > 0 && !isDashing)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * maxSpeed, Time.fixedDeltaTime * acceleration);
            }
            else if (!isDashing)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * deceleration);
            }

            // Giới hạn vị trí
            rb.position = new Vector2(
                Mathf.Clamp(rb.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(rb.position.y, minBounds.y, maxBounds.y)
            );

            // Xử lý xoay tay
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localScale = main_hand.transform.localScale;
            if (mousePosition.x > transform.position.x)
            {
                skin.flipX = false;
                inverse = 1f;
                localScale.y = Mathf.Abs(localScale.y);
            }
            else
            {
                skin.flipX = true;
                inverse = -1f;
                localScale.y = -Mathf.Abs(localScale.y);
            }

            main_hand.transform.localScale = localScale;

            // Xử lý recoil và swing
            recoilOffset = Mathf.SmoothDamp(recoilOffset, 0f, ref recoilVelocity, recoilRecoverySpeed * Time.deltaTime);
            currentSwing = Mathf.MoveTowards(currentSwing, targetSwing, swingSpeed * Time.deltaTime);

            if (currentSwing == targetSwing)
            {
                targetSwing = 0f;
            }

            Vector3 direction = mousePosition - main_hand.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            main_hand.transform.rotation = Quaternion.Lerp(main_hand.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle + (currentSwing + swingOffset) * inverse)), rotationSpeed * Time.deltaTime);
            main_hand.transform.position = rb.position + new Vector2(mousePosition.x - rb.position.x, mousePosition.y - rb.position.y).normalized * (1 + recoilOffset) * Hand_Radius;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            main_hand.transform.rotation = Quaternion.Lerp(main_hand.transform.rotation, networkHandRotation, Time.deltaTime * 10);
            main_hand.transform.localScale = networkScale;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"Player {gameObject.name} đã bị hủy, bỏ qua đồng bộ.");
            return;
        }

        if (stream.IsWriting)
        {
            // Kiểm tra null trước khi gửi dữ liệu
            if (main_hand == null || healthSystem == null || staminaSystem == null || skin == null)
            {
                Debug.LogWarning($"Không thể gửi dữ liệu đồng bộ từ {gameObject.name}: Một hoặc nhiều component là null.");
                return;
            }

            stream.SendNext(transform.position);                    // Vector3
            stream.SendNext(main_hand.transform.rotation);          // Quaternion
            stream.SendNext(healthSystem.CurrentHealth);            // int
            stream.SendNext(staminaSystem.currentStamina);          // float
            stream.SendNext(currentSwing);                         // float
            stream.SendNext(skin.flipX);                           // bool
            stream.SendNext(main_hand.transform.localScale);       // Vector3
            stream.SendNext(isDashing);                            // bool
        }
        else
        {
            // Kiểm tra null trước khi nhận dữ liệu
            if (healthSystem == null || skin == null)
            {
                Debug.LogWarning($"Không thể nhận dữ liệu đồng bộ trên {gameObject.name}: healthSystem hoặc skin là null.");
                return;
            }

            networkPosition = (Vector3)stream.ReceiveNext();       // Vector3
            networkHandRotation = (Quaternion)stream.ReceiveNext(); // Quaternion
            int receivedHealth = (int)stream.ReceiveNext();        // int
            float receivedStamina = (float)stream.ReceiveNext();   // float
            currentSwing = (float)stream.ReceiveNext();            // float
            skin.flipX = (bool)stream.ReceiveNext();               // bool
            networkScale = (Vector3)stream.ReceiveNext();          // Vector3
            isDashing = (bool)stream.ReceiveNext();                // bool

            // Cập nhật health một cách an toàn
            healthSystem.TakeDamage(healthSystem.CurrentHealth - receivedHealth);
        }
    }
}