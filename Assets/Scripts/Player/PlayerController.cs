using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Item currentTool;  // Công cụ hiện tại
    // Animation state constants
    private const string PLAYER_IDLE = "idle";
    private const string PLAYER_WALK = "walk";
    private const string PLAYER_MINING = "mining";
    private const string PLAYER_AXE = "axe";
    private const string PLAYER_WATERING = "watering";
    private const string PLAYER_DIG = "dig";
    private const string PLAYER_DOING = "doing";

    private Animator animator;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    // Theo dõi current animation state
    private string currentAnimationState;

    void Start()
    {
        // Lấy công cụ hiện tại từ Player.instance
        currentTool = Player.instance.equippedTool;

        rb = GetComponent<Rigidbody2D>();
        animator = rb.GetComponent<Animator>();
        // Đảm bảo bắt đầu từ trạng thái idle
        ChangeAnimationState(PLAYER_IDLE);
    }

    void Update()
    {
        // Cập nhật currentTool nếu công cụ thay đổi trong suốt quá trình
        if (currentTool != Player.instance.equippedTool)
        {
            currentTool = Player.instance.equippedTool;
        }

        HandleInput();

        // Kiểm tra di chuyển và thay đổi trạng thái animation tương ứng
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            ChangeAnimationState(PLAYER_WALK);
        }
        else
        {
            // Kiểm tra nếu không có hành động đặc biệt thì mới chuyển về Idle
            if (currentAnimationState != PLAYER_MINING &&
                currentAnimationState != PLAYER_DIG &&
                currentAnimationState != PLAYER_WATERING &&
                currentAnimationState != PLAYER_DOING)
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }

        // Nhấn F để thực hiện hành động tương ứng với công cụ hiện tại
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformToolAction();
        }
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

    void PerformToolAction()
    {
        // Kiểm tra nếu công cụ hợp lệ
        if (currentTool == null)
        {
            Debug.LogWarning("Không có công cụ nào được trang bị.");
            return;
        }

        switch (currentTool.toolType)
        {
            case ToolType.PickAxe:
                Debug.Log("Chuyển sang trạng thái mining.");
                ChangeAnimationState(PLAYER_MINING); // Thực hiện hành động mining với PickAxe
                break;

            case ToolType.Shovel:
                ChangeAnimationState(PLAYER_DIG);  // Thực hiện hành động đào với Shovel
                Debug.Log("Chuyển sang trạng thái dig.");
                break;

            case ToolType.WateringCan:
                ChangeAnimationState(PLAYER_WATERING); // Thực hiện hành động tưới nước với Watering Can
                break;
            case ToolType.SeedBag:
                ChangeAnimationState(PLAYER_DOING); //thực hiện hành động làm việc 
                break;
            default:
                Debug.LogWarning("Công cụ không hợp lệ hoặc chưa được trang bị.");
                break;
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
