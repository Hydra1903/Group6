using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
   [SerializeField] public float speed = 2f;
    private Rigidbody2D rb;
    private Animator anim;

   [SerializeField] private float stateSwitchTime = 2f;  // Thời gian giữa các lần chuyển đổi trạng thái
    private float timeSinceLastSwitch = 0f;  // Thời gian đã trôi qua kể từ lần chuyển đổi trước
    private bool isWalking = true;  // Biến kiểm tra trạng thái hiện tại (Walk hoặc Idle)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SwitchState();  // Chọn trạng thái ban đầu (Walk hoặc Idle)
    }

    void Update()
    {
        // Di chuyển về phía trước nếu đang ở trạng thái Walk
        if (isWalking)
        {
            rb.velocity = transform.right * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;  // Dừng lại nếu ở trạng thái Idle
        }

        // Kiểm tra va chạm với hàng rào
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1f, LayerMask.GetMask("Fence"));
        if (hit.collider != null)
        {
            // Đảo ngược hướng di chuyển khi va chạm
            transform.Rotate(0, 180, 0);
        }

        // Cập nhật thời gian và chuyển đổi trạng thái ngẫu nhiên
        timeSinceLastSwitch += Time.deltaTime;
        if (timeSinceLastSwitch >= stateSwitchTime)
        {
            SwitchState();  // Chuyển đổi trạng thái sau mỗi khoảng thời gian
            timeSinceLastSwitch = 0f;  // Đặt lại thời gian
        }
    }

    // Hàm chuyển đổi trạng thái giữa Walk và Idle
    void SwitchState()
    {
        if (isWalking)
        {
            anim.SetBool("isWalking", false);  // Chuyển sang trạng thái Idle
            isWalking = false;
        }
        else
        {
            anim.SetBool("isWalking", true);   // Chuyển sang trạng thái Walk
            isWalking = true;
        }
    }
}


