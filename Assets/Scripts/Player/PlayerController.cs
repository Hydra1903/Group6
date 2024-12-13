using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Animation state constants
    private const string PLAYER_IDLE = "idle";
    private const string PLAYER_WALK = "walk";
    private const string PLAYER_MINING = "mining";
    private const string PLAYER_AXE = "axe";

    private Animator animator;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    // Theo dõi current animation state
    private string currentAnimationState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = rb.GetComponent<Animator>();
        // Đảm bảo bắt đầu từ trạng thái idle
        ChangeAnimationState(PLAYER_IDLE);
    }

    void Update()
    {
        HandleInput();
        SetAnimation();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void HandleInput()
    {
        // Cho phép di chuyển 4 hướng
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        // Lật nhân vật theo hướng di chuyển ngang
        if (moveX > 0)
            transform.localScale = new Vector3(4.4f, 4.4f, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-4.4f, 4.4f, 1);
    }

    void MoveCharacter()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    void SetAnimation()
    {
        // Ưu tiên hành động đặc biệt
        if (Input.GetKey(KeyCode.Z)) // Hành động Mining
        {
            ChangeAnimationState(PLAYER_MINING);
        }
        else if (Input.GetKey(KeyCode.X)) // Hành động dùng Axe
        {
            ChangeAnimationState(PLAYER_AXE);
        }
        else if (moveDirection.sqrMagnitude > 0.01f) // Đang di chuyển
        {
            ChangeAnimationState(PLAYER_WALK);
        }
        else // Trạng thái mặc định
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
    }

    void ChangeAnimationState(string newState)
    {
        // Ngăn chặn animation interrupt chính nó
        if (currentAnimationState == newState) return;

        // Sử dụng CrossFade để chuyển animation mượt
        animator.CrossFade(newState, 0.1f);

        // Cập nhật current state
        currentAnimationState = newState;
    }

    // Phương thức hữu ích để force change state từ bên ngoài (nếu cần)
    public void ForceChangeAnimationState(string forcedState)
    {
        ChangeAnimationState(forcedState);
    }

    // Getter cho current state nếu cần
    public string GetCurrentAnimationState()
    {
        return currentAnimationState;
    }
}