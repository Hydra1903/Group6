using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Watering,
        Digging,
        Hoe,
        Axe
    }

    public static PlayerController instance;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 move;
    private PlayerState currentState = PlayerState.Idle; // Trạng thái hiện tại
    public float lastMoveX;
    public float lastMoveY;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Nhận input
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        // Cập nhật trạng thái nhân vật dựa trên input
        if (move.sqrMagnitude > 0) // Đang di chuyển
        {
            currentState = PlayerState.Moving;
            lastMoveX = move.x;
            lastMoveY = move.y;
        }
        else // Không di chuyển
        {
            currentState = PlayerState.Idle;
        }
        // Di chuyển

        rb.velocity = move * moveSpeed;

        SetAnimationState();
        //anim
        if (Input.GetKeyDown(KeyCode.F) && Player.instance != null)
        {
            // Kiểm tra xem người chơi có thể đào đất
            if (Player.instance.CanDig())
            {
                currentState = PlayerState.Digging;
            }
        }
    }
    // Phương thức chuyển đổi trạng thái animation
    private void SetAnimationState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                anim.SetFloat("Speed", 0);
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetBool("watering", false); // Tắt trạng thái watering
                anim.SetBool("digging", false);  // Tắt trạng thái digging
                anim.SetBool("hoe", false);      // Tắt trạng thái hoe
                anim.SetBool("axe", false);      // Tắt trạng thái axe
                break;

            case PlayerState.Moving:
                anim.SetFloat("Horizontal", move.x);
                anim.SetFloat("Vertical", move.y);
                anim.SetFloat("Speed", move.sqrMagnitude);
                anim.SetBool("watering", false); // Tắt trạng thái watering
                anim.SetBool("digging", false);  // Tắt trạng thái digging
                anim.SetBool("hoe", false);      // Tắt trạng thái hoe
                anim.SetBool("axe", false);      // Tắt trạng thái axe
                break;

            case PlayerState.Watering:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetBool("watering", true);  // Bật trạng thái watering
                break;

            case PlayerState.Digging:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetBool("digging", true);   // Bật trạng thái digging
                break;

            case PlayerState.Hoe:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetBool("hoe", true);       // Bật trạng thái hoe
                break;

            case PlayerState.Axe:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetBool("axe", true);       // Bật trạng thái axe
                break;
        }
    }
}
    //void UpdateToolColliderDirection()
    //{
       // if (movementDirection != Vector2.zero)
       // {
            // Đặt ToolCollider theo hướng của Player
       //     if (movementDirection.x > 0) // Di chuyển sang phải
       //     {
       //         toolCollider.transform.localPosition = new Vector3(0.02f, 0f, 0f); // Vị trí bên phải Player
       //         toolCollider.transform.localRotation = Quaternion.Euler(0, 0, 0); // Không xoay
        //    }
        //    else if (movementDirection.x < 0) // Di chuyển sang trái
        //    {
        //        toolCollider.transform.localPosition = new Vector3(-0.5f, 0f, 0f); // Vị trí bên trái Player
         //       toolCollider.transform.localRotation = Quaternion.Euler(0, 0, 0); // Không xoay
       //     }
            //else if (movementDirection.y > 0) // Di chuyển lên
            //{
            //    toolCollider.transform.localPosition = new Vector3(0f, 1f, 0f); // Vị trí bên trên Player
            //    toolCollider.transform.localRotation = Quaternion.Euler(0, 0, 90); // Xoay 90 độ
            //}
            //else if (movementDirection.y < 0) // Di chuyển xuống
            //{
            //    toolCollider.transform.localPosition = new Vector3(0f, -1f, 0f); // Vị trí bên dưới Player
            //    toolCollider.transform.localRotation = Quaternion.Euler(0, 0, -90); // Xoay -90 độ
            //}
       // }
    //}
