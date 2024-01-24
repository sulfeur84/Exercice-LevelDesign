using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float JumpForce;
    [SerializeField] private bool JumpInertia;
    [SerializeField] private float Speed;
    [Range(0, .3f)] [SerializeField] private float MovementSmoothing;
    [SerializeField] private bool AirControl;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private float CoyoteTime;
    [SerializeField] private float JumpBufferTime;

    [Header("Objects")]
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private Animator PlayerAnimator;

    //Others
    const float GroundedRadius = .15f;
    private bool Grounded;
    private Rigidbody2D rb;
    private bool FacingRight = true;
    private Vector3 Velocity = Vector3.zero;
    private float HorizontalMove;
    private float CoyoteCounter;
    private float JumpBufferCounter;

    [Header("Events")]
    [Space] public UnityEvent OnLandEvent;
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }
    private void Awake()
    {
        //Anime n'est pas vrai au démarage
        Time.timeScale = 1;


        //Set Speed Cap

        rb = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null) OnLandEvent = new UnityEvent();

    }

    private void FixedUpdate()
    {
        Move(HorizontalMove * Time.fixedDeltaTime);
    }

    private void Update()
    {
        bool wasGrounded = Grounded;
        Grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundedRadius, WhatIsGround);

        //Detect ground
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                Grounded = true;
                if (!wasGrounded) OnLandEvent.Invoke();

                if (JumpBufferCounter > 0) Jump();
            }
            else
            {
                Grounded = false;
            }
        }

        //CoyoteTime
        if (Grounded)
        {
            if (!wasGrounded)
            {
                CoyoteCounter = CoyoteTime;
            }
            PlayerAnimator.SetBool("Grounded", true);
        }
        else
        {
            PlayerAnimator.SetBool("Grounded", false);
            CoyoteCounter -= Time.deltaTime;
        }

        JumpBufferCounter -= Time.deltaTime;

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = JumpBufferTime;
            if (CoyoteCounter > 0) Jump();
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            CoyoteCounter = 0;
        }

        //Movement
        HorizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
        PlayerAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        //Crouch Input


        //Run
    }
    public void Move(float move)
    {

        if (Grounded || AirControl)
        {
            //Crouch
            
            //Not Crouch
            

            Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);

            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref Velocity, MovementSmoothing);

            //Check for flip
            if (move > 0 && !FacingRight)
            {
                Flip();
            }

            else if (move < 0 && FacingRight)
            {
                Flip();
            }
        }
    }

    private void Jump()
    {
        CoyoteCounter = 0;
        JumpBufferCounter = 0;

        if (!JumpInertia) rb.velocity = Vector2.zero;
        rb.velocity = Vector2.up * JumpForce;

        //JumpSound
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}