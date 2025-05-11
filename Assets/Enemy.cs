using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    private Vector3 moveDirrection;

    private EnemyAI ai_movement;

    public Transform HandItem;

    private HealthSystem health;

    private Rigidbody2D rb;
    private Vector2 movement;
    public Vector3 lookAtPos;

    private SpriteRenderer skin;
    public float lookDir;

    private GameObject main_hand;
    public float Hand_Radius = 0.5f;
    private float rotationSpeed = 10f;

    [HideInInspector] public float recoilForce = 2f;  // Base recoil force
    [HideInInspector] public float recoilRecoverySpeed = 5f; // How fast recoil returns to normal
    [HideInInspector] public float dampingFactor = 0.1f; // Smooth damping factor
    [HideInInspector] private float recoilOffset = 0f; // Recoil displacement (single float)
    [HideInInspector] private float recoilVelocity = 0f; // Smooth recoil movement

    [HideInInspector] public float swingSpeed = 10f; // Speed of swing increasing
    [HideInInspector] public float swingOffset = 30f;
    [HideInInspector] public float swingRecoil = -90f;
    [HideInInspector] private float currentSwing = 0f; // Current swing value
    [HideInInspector] private bool isSwinging = false; // Whether swinging is happening

    [HideInInspector] private float targetSwing = 0f; // The target swing angle (can be set dynamically)
    [HideInInspector] private float inverse = 1f;

    [HideInInspector] private Transform model;

    private bool isMoving = false;
    private float legProgresion = 0f;

    private Transform leg_R;
    private Transform leg_L;
    private Transform body;

    private float leg_R_swing = 0f;
    private float leg_L_swing = 0f;

    private float body_swing = 0f;

    private PlayerInventory inventory;

    private Collider2D enemy_collider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skin = GetComponent<SpriteRenderer>();
        enemy_collider = GetComponent<BoxCollider2D>();
        ai_movement = GetComponent<EnemyAI>();
        health = GetComponent<HealthSystem>();

        main_hand = transform.Find("Main").gameObject;

        HandItem = main_hand.transform.Find("Item").transform;

        model = transform.Find("Model").transform;

        leg_L = model.Find("LegL").transform;
        leg_R = model.Find("LegR").transform;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        
    }

    // Function to set the target swing angle dynamically
    public void TriggerSwing(float angle)
    {
        targetSwing = angle; // Change the target swing angle
    }

    public void TriggerRecoil(float intensity)
    {
        recoilOffset = recoilForce * intensity;
    }

    public void SetHealth(float amount)
    {
        HealthSystem healthSystem = transform.GetComponent<HealthSystem>();
        healthSystem.SetHealth((int)amount);
    }

    public void SetArmor(float amount)
    {
        HealthSystem healthSystem = transform.GetComponent<HealthSystem>();
        healthSystem.SetArmor((int)amount);
    }

    void FixedUpdate()
    {
        Boolean lastMovingState = isMoving;
        isMoving = (ai_movement.MoveDirection.magnitude > 0.2f);
        float forward = 1f;
        if (ai_movement.MoveDirection.x < 0)
        {
            forward = -1f;
        }
        if (isMoving)
        {
            if (lastMovingState == false)
            {
                legProgresion = 0f;
                body_swing = 0f;
            }

            legProgresion += ai_movement.MoveDirection.magnitude * forward * inverse * 0.2f;
            body_swing = 0.05f * Mathf.Sin(legProgresion);
            leg_L_swing = Mathf.MoveTowards(leg_L_swing, 60 * Mathf.Cos(legProgresion), Time.fixedDeltaTime * 1000);
            leg_R_swing = Mathf.MoveTowards(leg_R_swing, 60 * Mathf.Cos(legProgresion + Mathf.PI), Time.fixedDeltaTime * 1000);
        }
        else
        {
            body_swing = Mathf.MoveTowards(body_swing, 0, Time.fixedDeltaTime * 100);
            leg_L_swing = Mathf.MoveTowards(leg_L_swing, 0, Time.fixedDeltaTime * 250);
            leg_R_swing = Mathf.MoveTowards(leg_R_swing, 0, Time.fixedDeltaTime * 250);
        }

        Quaternion legLRotation = Quaternion.Euler(
            new Vector3(0, 0, leg_L_swing)
            );
        leg_L.localRotation = legLRotation;

        Quaternion legRRotation = Quaternion.Euler(
            new Vector3(0, 0, leg_R_swing)
            );
        leg_R.localRotation = legRRotation;

        model.localPosition = new Vector3(0, body_swing, 0);

        // Update Rendering
        Vector3 mousePosition = lookAtPos;
        Vector3 localScale = main_hand.transform.localScale;

        Vector3 scale = model.localScale;

        if (mousePosition.x > transform.position.x)
        {
            scale.x = Math.Abs(scale.x);
            inverse = 1f;
            localScale.y = Mathf.Abs(localScale.y);
        }
        else
        {
            scale.x = -Math.Abs(scale.x);
            inverse = -1f;
            localScale.y = -Mathf.Abs(localScale.y);
        }

        model.localScale = scale;

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
        lookDir = angle;
        // Hand Update
        main_hand.transform.rotation = Quaternion.Lerp(main_hand.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle + (currentSwing + swingOffset) * inverse)), rotationSpeed * Time.deltaTime);
        main_hand.transform.position = new Vector2(model.position.x, model.position.y) + new Vector2(mousePosition.x - rb.position.x, mousePosition.y - rb.position.y).normalized * (1 + recoilOffset) * Hand_Radius;
        
        if (health.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
