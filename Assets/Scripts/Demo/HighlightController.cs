using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class HighlightController : MonoBehaviour
{
    public Transform player;
    public Transform highlight;
    public Grid grid; // Grid hiện tại
    public float highlightDistance = 1f;
    public Vector3Int currentCell { get; private set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tự động tìm Grid mới trong scene
        Grid newGrid = FindObjectOfType<Grid>();
        if (newGrid != null)
        {
            grid = newGrid;
            Debug.Log("Grid mới đã được gán cho HighlightController.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Grid trong scene mới!");
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0);
            Vector3Int playerCell = grid.WorldToCell(player.position);
            Vector3Int targetCell = playerCell + Vector3Int.RoundToInt(direction);

            // Kiểm tra khoảng cách
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
