using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Item equippedTool; // Tool hiện tại được trang bị
    public GameObject equippedToolIcon; // GameObject icon của công cụ đang được trang bị (dùng để bật/tắt)
    private Toolbar toolbar; //lấy từ toolbar

    public ToolType currentTool = ToolType.None;  // Công cụ hiện tại
    public ItemType currentItem = ItemType.None; // Item hiện tại
    public bool isToolActive = false;  // Trạng thái công cụ có đang active hay không
    public bool isUsingTool = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại Canvas khi chuyển Scene
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có một instance của InventoryUIManager
        }
    }

    private void Update()
    {
        // Logic xử lý các hành động hoặc trạng thái trong game
    }

    // Hàm để toggle (bật/tắt) công cụ
    public void ToggleTool(Item tool)
    {
        // Nếu công cụ hiện tại đang là công cụ đã trang bị và icon đã hiển thị, thì tắt công cụ
        if (equippedTool == tool && equippedToolIcon != null)
        {
            equippedToolIcon.SetActive(false); // Ẩn GameObject icon công cụ

            equippedTool = null;  // Đặt công cụ trang bị là null
            isUsingTool = false;
            isToolActive = false;  // Tắt công cụ
            currentTool = ToolType.None;  // Đặt công cụ hiện tại về "None"
            currentItem = ItemType.None;
            Debug.Log("Vật phẩm đã bị tắt");
            return;
        }

        // Gán công cụ mới
        equippedTool = tool;

        // Nếu có icon UI (GameObject), cập nhật sprite cho icon và bật GameObject
        if (equippedToolIcon != null)
        {
            equippedToolIcon.GetComponent<SpriteRenderer>().sprite = tool.icon;
            equippedToolIcon.SetActive(true); // Bật GameObject icon
        }

        // Cập nhật công cụ và trạng thái active
        if (tool != null)
        {
            if (tool.itemType == ItemType.Tool)
            {
                currentTool = tool.toolType;  // Cập nhật công cụ hiện tại từ toolType
                currentItem = ItemType.Tool; // Reset currentItem nếu là Tool
                isToolActive = true;  // Đảm bảo công cụ đang active
                Debug.Log("Công cụ hiện tại: " + currentTool + ", Trạng thái active: " + isToolActive);
            }
            else if (tool.itemType == ItemType.Seed)
            {
                currentItem = ItemType.Seed;  // Gán currentItem là Seed
                currentTool = ToolType.None;  // Không cập nhật công cụ
                isToolActive = true;  // Không cần active Tool khi là Seed
                Debug.Log("Công cụ hiện tại là Seed: " + tool.itemName);
            }
        }
    }

    // Hàm để tắt công cụ
    public void DeactivateTool()
    {
        isToolActive = false;  // Tắt công cụ

        if (equippedToolIcon != null)
        {
            equippedToolIcon.SetActive(false);  // Ẩn GameObject icon công cụ
        }

        currentTool = ToolType.None;
        currentItem = ItemType.None;
        equippedTool = null;
        Debug.Log("Công cụ đã bị tắt");
    }

    // Các hàm kiểm tra hành động có thể thực hiện
    public bool CanDig()
    {
        return currentTool == ToolType.Shovel && isToolActive;  // Kiểm tra nếu công cụ là Shovel và active
    }

    public bool CanPlantSeeds()
    {
        return currentItem == ItemType.Seed && isToolActive;
    }

    public bool CanWatering()
    {
        return currentTool == ToolType.WateringCan && isToolActive;
    }

    public bool CanHarvest()
    {
        return currentTool == ToolType.HandHarvest && isToolActive;
    }

    public bool CanFishing()
    {
        return currentTool == ToolType.FishingRod && isToolActive;
    }

    public bool CanMining()
    {
        return currentTool == ToolType.PickAxe && isToolActive;
    }
}
