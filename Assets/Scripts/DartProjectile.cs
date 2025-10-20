using UnityEngine;

public class DartProjectile : MonoBehaviour
{
    public float speed = 15f; // Tốc độ di chuyển của phi tiêu
    public int damage = 1;   // Lượng sát thương
    public float lifetime = 1.5f; // Thời gian phi tiêu tự hủy nếu không trúng

    private Vector2 direction; // Hướng bay
    private Rigidbody2D rb;
    [Header("Rotation Settings")]
    public float rotationSpeed = 360f; // Tốc độ xoay (độ trên giây, 720 là 2 vòng/giây)
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //nhớ set gravity scale = 0 ngoài inspector, hoặc set code như bên dưới
        rb.gravityScale = 0;
    }


    void Update()
    {
        // Di chuyển phi tiêu về phía trước theo hướng đã định       
        // transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * Mathf.Sign(direction.x));
    }


    // Xử lý khi phi tiêu va chạm (sử dụng Is Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là enemy không
        if (other.CompareTag("Enemy"))
        {
            // Gây sát thương
            // *Bạn cần một script quản lý HP của enemy*
            // EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(damage);
            // }
        }

        // Hủy phi tiêu sau khi va chạm (trừ khi nó là Player)
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    
    public void Launch(Vector2 direction)
    {
        this.direction = direction;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
            //    rb.angularVelocity = rotationSpeed; 
        }
        
        Destroy(gameObject, lifetime);
    }
}
