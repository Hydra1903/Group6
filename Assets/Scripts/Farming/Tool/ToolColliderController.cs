using UnityEngine;

public class ToolColliderController : MonoBehaviour
{
    public GameObject toolCollider; // Tham chiếu đến GameObject chứa Collider

    private void Start()
    {
        toolCollider.SetActive(false);
    }

    // Phương thức gọi từ Animation Event để bật Collider
    public void EnableToolCollider()
    {
        toolCollider.SetActive(true);
    }

    // Phương thức gọi từ Animation Event để tắt Collider
    public void DisableToolCollider()
    {
        toolCollider.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rock")) // Giả sử viên đá có tag là "Rock"
        {
            // Logic tương tác với viên đá, ví dụ: phá hủy hoặc gây hiệu ứng
            Debug.Log("Cuốc chạm vào viên đá!");
        }
    }
}

