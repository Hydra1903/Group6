using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public Transform player; // Transform của nhân vật
    public Transform highlight; // Transform của highlight
    public Grid grid; // Grid của ô đất
    public float highlightDistance = 1f; // Khoảng cách highlight (theo ô)

    private Vector3Int currentCell; // Ô hiện tại trên Grid

    void Update()
    {
        // Lấy input từ người dùng (WASD hoặc Arrow keys)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            // Tính hướng di chuyển
            Vector3 direction = new Vector3(horizontal, vertical, 0);

            // Lấy ô hiện tại của nhân vật
            Vector3Int playerCell = grid.WorldToCell(player.position);

            // Tính ô mới dựa trên hướng di chuyển
            Vector3Int targetCell = playerCell + Vector3Int.RoundToInt(direction);

            // Kiểm tra ô mới có nằm trong khoảng cách highlight không
            if (Vector3.Distance(grid.GetCellCenterWorld(playerCell), grid.GetCellCenterWorld(targetCell)) <= highlightDistance)
            {
                // Cập nhật vị trí highlight
                highlight.position = grid.GetCellCenterWorld(targetCell);
                currentCell = targetCell; // Lưu lại ô đang được highlight
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Debug phạm vi highlight xung quanh nhân vật
        if (grid != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, highlightDistance);
        }
    }
}

