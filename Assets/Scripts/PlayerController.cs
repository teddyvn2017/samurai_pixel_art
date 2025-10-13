using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private bool isFacingRight = true;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float speed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;
    private bool isGrounded;

    private bool wasGrounded = false;
    private bool isLanded;

    public GameObject dustPrefab;
    public Transform dustPosition;
    
    private bool isJumping = false;   
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.SetBool("isRunning", false);
        rb.freezeRotation = true;
    }    
    void Update()
    {
        Move();
        Jump();
        Attack();
        Landed();
    }
    
    void Move()
    {        
        float move = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)
            move = -1f;
        if (Keyboard.current.rightArrowKey.isPressed)      
            move = 1f;
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move == 0)
        {            
            anim.SetTrigger("Idle");
            anim.SetBool("isRunning", false);
        
        }
        else
        {            
            anim.SetBool("isRunning", true);        
            anim.ResetTrigger("Idle");        
            if (move > 0 && !isFacingRight)
                Flip();
            else if (move < 0 && isFacingRight)
                Flip();
        }       
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    void Jump()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        if (isGrounded && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            anim.SetTrigger("isJumping");
            anim.SetBool("isRunning", false);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            CreatingLandDust();
            // isJumping = true;
            // isLanded = false;
        }

    }

    void Attack()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            anim.SetTrigger("Attack");
    }

    void Landed()
    {
        
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        if (isGrounded && !wasGrounded)
        {
            CreatingLandDust();
        }
        wasGrounded = isGrounded;
    }
    
    void CreatingLandDust()
    {
        Instantiate(dustPrefab, dustPosition.position, dustPrefab.transform.rotation);
    }
}