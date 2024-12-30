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

    // Theo dõi trạng thái hành động
    private string currentAnimationState;
    private bool isPerformingAction = false;

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

        if (!isPerformingAction) // Chỉ xử lý di chuyển và input nếu không đang thực hiện hành động
        {
            HandleInput();

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                ChangeAnimationState(PLAYER_WALK);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }

            // Nhấn F để thực hiện hành động tương ứng với công cụ hiện tại
            if (Input.GetKeyDown(KeyCode.F))
            {
                PerformToolAction();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isPerformingAction)
        {
            MoveCharacter();
        }
    }

    void HandleInput()
    {
        // Di chuyển 4 hướng
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
        if (currentTool == null)
        {
            Debug.LogWarning("Không có công cụ nào được trang bị.");
            return;
        }

        if (currentTool.itemType == ItemType.Tool)
        {
            switch (currentTool.toolType)
            {
                case ToolType.PickAxe:
                    StartAction(PLAYER_MINING);
                    break;

                case ToolType.Shovel:
                    StartAction(PLAYER_DIG);
                    break;

                case ToolType.WateringCan:
                    StartAction(PLAYER_WATERING);
                    break;

                default:
                    Debug.LogWarning("Công cụ không hợp lệ.");
                    break;
            }
        }
        else if (currentTool.itemType == ItemType.Seed)
        {
            StartAction(PLAYER_DOING);
        }
        else
        {
            Debug.LogWarning("Công cụ không hợp lệ hoặc chưa được trang bị.");
        }
    }

    void StartAction(string actionState)
    {
        isPerformingAction = true;
        ChangeAnimationState(actionState);
    }

    public void OnActionComplete()
    {
        // Hàm được gọi khi Animation Event kết thúc
        isPerformingAction = false;
        ChangeAnimationState(PLAYER_IDLE);
    }

    void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        animator.CrossFade(newState, 0.1f);
        currentAnimationState = newState;
    }
}
