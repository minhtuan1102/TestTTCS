using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public float range = 5f; // Player range

    private float vision = 5f; // Player vision
    public float minVision = -2f; // limit zoom in
    public float maxVision = 2f; // limit zoom out
    public float zoomSpeed = 5f;  // Zoom speed
    public float moveZoomDecrease = 2f; // Vison reduce when moving

    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 15f; // Speed up smoothly
    public float deceleration = 10f; // Slow down smoothly
    public float linearDrag = 4f; // Adjust for natural movement
    private Vector3 moveDirrection;

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public float dashSpeed = 10f; // Dash speed
    public float dashDuration = 0.2f; // Dash time
    private bool isDashing = false;
    private float dashTime;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 movement;
    private float targetVision;

    private SpriteRenderer skin;

    private float cellSize = 1f;

    private GameObject main_hand;
    public float Hand_Radius = 0.5f;
    private float rotationSpeed = 10f;

    public float recoilForce = 2f;  // Base recoil force
    public float recoilRecoverySpeed = 5f; // How fast recoil returns to normal
    public float dampingFactor = 0.1f; // Smooth damping factor
    private float recoilOffset = 0f; // Recoil displacement (single float)
    private float recoilVelocity = 0f; // Smooth recoil movement

    public float maxSwing = 10f; // Maximum swing value
    public float swingSpeed = 1f; // Speed of swing increasing
    public float returnSpeed = 1f; // Speed at which the swing value returns to 0
    private float currentSwing = 0f; // Current swing value
    private bool isSwinging = false; // Whether swinging is happening

    private float targetSwing = 0f; // The target swing angle (can be set dynamically)

    private Collider2D player_collider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skin = GetComponent<SpriteRenderer>();
        player_collider = GetComponent<BoxCollider2D>();
        main_hand = transform.Find("Main").gameObject;

        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap != null)
        {
            cellSize = tilemap.layoutGrid.cellSize.x;
        }

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.linearDamping = linearDrag;
        cam = Camera.main;
        targetVision = vision;
    }

    void Update()
    {
        // Get Input (WASD or Arrow Keys)
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // If the player presses Q and is not already dashing, start the dash
        if (Input.GetKeyDown(KeyCode.Q) && !isDashing)
        {
            // Set the dash direction to the movement direction
            if (rb.linearVelocity.magnitude > 0)
            {
                moveDirrection = rb.linearVelocity.normalized;
            }

            // Start dashing
            isDashing = true;
            dashTime = 0;
        }

        if (Input.GetMouseButtonDown(0)) // Left mouse button (fire)
        {
            Vector2 fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            // Example: Normal shot with base recoil
            TriggerRecoil(1f);
            TriggerSwing(30f);
            // Example: Charged shot with **double recoil**
            // TriggerRecoil(fireDirection, 2f);
        }

        // Normalize diagonal movement
        if (movement.magnitude > 1)
            movement.Normalize();

        if (movement.magnitude > 0.1f)
        {
            // Reduce vision when moving
            targetVision = Mathf.Max(range + minVision, range + maxVision - moveZoomDecrease);
        }
        else
        {
            // Icrease vision when idle
            targetVision = range + maxVision;
        }

        // Smooth zoom
        vision = Mathf.Lerp(vision, targetVision * cellSize, Time.deltaTime * zoomSpeed);

        // Camera update
        if (cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, vision, Time.deltaTime * zoomSpeed);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, vision * zoomSpeed, Time.deltaTime * zoomSpeed);

        // Smooth Camera Follow
        Vector3 targetCamPos = new Vector3(rb.position.x, rb.position.y, -10);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetCamPos, Time.deltaTime * 3f);
    }

    // Function to set the target swing angle dynamically
    public void TriggerSwing(float angle = 0f)
    {
        targetSwing = angle; // Change the target swing angle
    }

    public void TriggerRecoil(float intensity = 1f)
    {
        recoilOffset = recoilForce * intensity;
    }

    void FixedUpdate()
    {

        if (isDashing)
        {
            // Move the player in the dash direction
            rb.linearVelocity = moveDirrection * dashSpeed;

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

            // Timer for the dash duration
            dashTime += Time.fixedDeltaTime;

            // Stop dashing after the duration
            if (dashTime >= dashDuration)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
                isDashing = false;
                rb.linearVelocity = Vector2.zero; // Stop player movement after dash
            }
        }

        if (movement.magnitude > 0)
        {
            // Apply acceleration-based force for smoother movement
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, movement * maxSpeed, Time.fixedDeltaTime * acceleration);
        }
        else
        {
            // Apply deceleration to slow down smoothly
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * deceleration);
        }

        // Keep player inside bounds
        rb.position = new Vector2(
            Mathf.Clamp(rb.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(rb.position.y, minBounds.y, maxBounds.y)
        );

        // Update Rendering
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition); // Mouse pos
        Vector3 localScale = main_hand.transform.localScale;
        if (mousePosition.x > transform.position.x)
        {
            skin.flipX = false; // Flip skin
            localScale.y = Mathf.Abs(localScale.y);
        }
        else
        {
            skin.flipX = true;
            localScale.y = -Mathf.Abs(localScale.y);
        }

        main_hand.transform.localScale = localScale;

        // Hand movement

        // Smoothly interpolate recoil
        recoilOffset = Mathf.SmoothDamp(recoilOffset, 0f, ref recoilVelocity, recoilRecoverySpeed * Time.deltaTime);
        currentSwing = Mathf.MoveTowards(currentSwing, targetSwing, swingSpeed * Time.deltaTime);

        // If we reached the target swing angle, stop swinging and reset to 0
        if (currentSwing == targetSwing)
        {
            targetSwing = 0f;
        }

        Vector3 direction = mousePosition - main_hand.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Hand Update
        main_hand.transform.rotation = Quaternion.Lerp(main_hand.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle + currentSwing)), rotationSpeed * Time.deltaTime);
        main_hand.transform.position = rb.position + new Vector2(mousePosition.x - rb.position.x, mousePosition.y - rb.position.y).normalized * (1 + recoilOffset)  * Hand_Radius;
    }
}
