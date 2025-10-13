using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Thời gian chờ cho animation phát xong
    public float duration = 0.2f; 

    void Start()
    {
        // Hủy đối tượng này sau 'duration' giây
        Destroy(gameObject, duration); 
    }
}
