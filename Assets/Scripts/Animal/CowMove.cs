using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowMove : MonoBehaviour
{
    private Animator anim;

    public float moveSpeed = 2f;       // Tốc độ di chuyển
    public Vector2 areaCenter = Vector2.zero; // Tâm khu vực di chuyển (tùy chỉnh trong Inspector)
    public Vector2 areaSize = new Vector2(5f, 5f); // Kích thước khu vực di chuyển
    public float waitTime = 2f;       // Thời gian chờ trước khi chọn điểm mới

    private Vector2 targetPosition;   // Vị trí điểm đến tiếp theo
    private float waitTimer;          // Bộ đếm thời gian chờ

    void Start()
    {
        anim = GetComponent<Animator>();
        SetNewTargetPosition(); // Chọn điểm đầu tiên
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        // Di chuyển về phía điểm đến
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Lật nhân vật nếu cần
        FlipCharacter();

        // Kiểm tra nếu đã đến nơi
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Chờ trước khi chọn điểm mới
            waitTimer += Time.deltaTime;
            anim.SetBool("isIdle", true);
            if (waitTimer >= waitTime)
            {
                SetNewTargetPosition();
                waitTimer = 0f;
            }
        }
        else
        {
            anim.SetBool("isIdle", false);
        }
    }

    void SetNewTargetPosition()
    {
        // Tạo vị trí ngẫu nhiên trong khu vực dựa trên tâm và kích thước
        float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float randomY = Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2);
        targetPosition = new Vector2(randomX, randomY);
    }

    void FlipCharacter()
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.x > 0 && transform.localScale.x < 0) // Di chuyển sang phải
        {
            transform.localScale = new Vector2(2.2f, 2.2f);
        }
        else if (direction.x < 0 && transform.localScale.x > 0) // Di chuyển sang trái
        {
            transform.localScale = new Vector2(-2.2f, 2.2f);
        }
    }


    // Debug để hiển thị khu vực di chuyển trong Scene
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(areaCenter, areaSize); // Khu vực di chuyển theo tâm và kích thước
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Animal"))
        {
            SetNewTargetPosition();
        }
    }
}
