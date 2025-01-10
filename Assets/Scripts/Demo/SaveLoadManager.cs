using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerSaveData.json");
    }

    // Lưu dữ liệu người chơi
    public void SaveGame()
    {
        PlayerData playerData = new PlayerData(PlayerStat.Instance, Toolbar.instance, InventoryController.instance);
        string json = JsonUtility.ToJson(playerData, true); // true để format JSON đẹp
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved!");
        Debug.Log("Save file path: " + saveFilePath);
    }

    // Tải dữ liệu người chơi
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // Cập nhật thông tin người chơi
            PlayerStat.Instance.coins = playerData.coins;
            PlayerStat.Instance.energy = playerData.energy;
            PlayerStat.Instance.maxEnergy = playerData.maxEnergy;

            // Cập nhật toolbar
            Toolbar.instance.toolbarItems.Clear();
            foreach (ItemData itemData in playerData.toolbarItems)
            {
                Item item = InventoryController.instance.GetItemByName(itemData.itemName); // Tìm item theo tên
                if (item != null)
                {
                    item.quantity = itemData.quantity;
                    item.icon = itemData.GetIcon();  // Lấy lại icon từ đường dẫn
                    item.toolType = (ToolType)System.Enum.Parse(typeof(ToolType), itemData.toolType); // Convert chuỗi thành ToolType
                    item.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.itemType); // Convert chuỗi thành ItemType
                    item.description = itemData.description;
                    item.toolPrefab = itemData.GetPrefab(); // Lấy lại prefab từ đường dẫn

                    Toolbar.instance.toolbarItems.Add(item);
                }
            }

            // Cập nhật inventory
            InventoryController.instance.inventoryItems.Clear();
            foreach (ItemData itemData in playerData.inventoryItems)
            {
                Item item = InventoryController.instance.GetItemByName(itemData.itemName);
                if (item != null)
                {
                    item.quantity = itemData.quantity;
                    item.icon = itemData.GetIcon();
                    item.toolType = (ToolType)System.Enum.Parse(typeof(ToolType), itemData.toolType);
                    item.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.itemType);
                    item.description = itemData.description;
                    item.toolPrefab = itemData.GetPrefab();

                    InventoryController.instance.inventoryItems.Add(item);
                }
            }

            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogError("Save file not found.");
        }
    }
}
