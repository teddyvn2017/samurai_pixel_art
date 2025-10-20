using System.Collections;
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

    public GameObject dustPrefab;
    public GameObject dartPrefab;
    public Transform dustPosition;
    public Transform throwPosition; // Vị trí phi tiêu sẽ được phóng ra


    [Header("Dash Settings")]
    public float dashSpeed = 10f;       // Tốc độ lướt
    public float dashTime = 0.3f; // thời gian lướt
    public float dashCooldown = 1f;  // Thời gian hồi chiêu

    private bool isDashing = false;
    private float nextDashTime = 0f; // Thời điểm lần lướt tiếp theo có thể xảy ra  

    // Thêm một biến để kiểm soát trạng thái đang ném
    private bool isThrowing = false;
    private float nextThrowTime = 0f;
    public float throwCooldown = 0.5f; // Thời gian hồi chiêu

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.SetBool("isRunning", false);
        // anim.SetBool("isJumping", false);
        rb.freezeRotation = true;
    }
    void Update()
    {
        Move();
        Jump();
        Melee();
        Landed();
        HandleDashInput();
        HandleThrow();
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

        }

    }

    void Melee()
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

    void HandleDashInput()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && Time.time >= nextDashTime && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;

        // Lấy hướng Dash dựa trên hướng nhân vật đang quay mặt (local scale X)
        float facingDir = transform.localScale.x > 0 ? 1 : -1;
        Vector2 dashDirection = new Vector2(facingDir, 0);

        rb.gravityScale = 0;
        anim.SetBool("isRunning", false);
        rb.linearVelocity = Vector2.zero;
        // Tạm dừng player 0.4s trước khi thực thi Dash
        yield return new WaitForSeconds(0.4f);
        anim.SetTrigger("Dashing");

        float start = Time.time;
        while (Time.time < start + dashTime)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            yield return null;
        }
        // 2. Đợi hết thời gian Dash
        // yield return new WaitForSeconds(dashTime);
        CreatingLandDust();
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1;

        // Tạo cảm giác "hồi chiêu" hoặc khựng lại sau cú Dash.
        yield return new WaitForSeconds(0.4f); // <-- THÊM DÒNG NÀY

        // 4. gán lại giá trị nextDashTime cho lần kế tiếp
        nextDashTime = Time.time + dashCooldown;

    }

    void HandleThrow()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && Time.time >= nextThrowTime && !isDashing)
        {
            // PerformThrow();
            // Debug
            anim.SetTrigger("Throwing");
            isThrowing = true;
            nextThrowTime = Time.time + throwCooldown;
        }
    }

    //hàm ném phi tiêu sẽ được gắn với frame trong animtion "Throw"
    //thông qua event gọi tên hàm PerformThrow
    void PerformThrow()
    {
        // Tạo bản sao của phi tiêu, tại vị trí phía trước player
        GameObject dartObj = Instantiate(dartPrefab, throwPosition.position, dartPrefab.transform.rotation);
        //lấy hướng của player, đang xoay mặt về trái/phải
        float facingDir = transform.localScale.x > 0 ? 1 : -1;
        Vector2 shootDirection = new Vector2(facingDir, 0);
        DartProjectile dartScript = dartObj.GetComponent<DartProjectile>();

        // điều chỉnh hướng phi tiêu sao cho phù hợp với hướng player
        dartObj.transform.localScale = new Vector3(dartObj.transform.localScale.x * facingDir, dartObj.transform.localScale.y, dartObj.transform.localScale.z);
        
        dartScript.Launch(shootDirection);
        
    }

    void EndThrow()
    {
        isThrowing = false;
    }

}   