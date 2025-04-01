using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        skin = GetComponent<SpriteRenderer>();

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

        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition); // Mouse pos
        if (mousePosition.x > transform.position.x)
            skin.flipX = false; // Flip skin
        else
            skin.flipX = true;

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

    void FixedUpdate()
    {

        if (isDashing)
        {
            // Move the player in the dash direction
            rb.linearVelocity = moveDirrection * dashSpeed;

            // Timer for the dash duration
            dashTime += Time.fixedDeltaTime;

            // Stop dashing after the duration
            if (dashTime >= dashDuration)
            {
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
    }
}
