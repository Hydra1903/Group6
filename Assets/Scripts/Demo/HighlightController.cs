using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightController : MonoBehaviour
{
    public Transform player;
    public Transform highlight;
    public Grid grid;
    public float highlightDistance = 1f;
    public Vector3Int currentCell { get; private set; }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0);
            Vector3Int playerCell = grid.WorldToCell(player.position);
            Vector3Int targetCell = playerCell + Vector3Int.RoundToInt(direction);

            // Simplify by only checking distance
            if (Vector3.Distance(grid.GetCellCenterWorld(playerCell), grid.GetCellCenterWorld(targetCell)) <= highlightDistance)
            {
                highlight.position = grid.GetCellCenterWorld(targetCell);
                currentCell = targetCell;
            }
        }
    }

    public Vector3Int GetCurrentCell()
    {
        return currentCell;
    }
}