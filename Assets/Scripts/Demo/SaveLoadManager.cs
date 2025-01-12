using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "playerSaveData.json";
    private const string BACKUP_FILE_NAME = "playerSaveData_backup.json";
    private string saveFilePath;
    private string backupFilePath;

    private void Awake()
    {
        string savePath = Application.persistentDataPath;
        saveFilePath = Path.Combine(savePath, SAVE_FILE_NAME);
        backupFilePath = Path.Combine(savePath, BACKUP_FILE_NAME);
    }

    public void SaveGame()
    {
        try
        {
            // Tạo backup file cũ nếu tồn tại
            if (File.Exists(saveFilePath))
            {
                File.Copy(saveFilePath, backupFilePath, true);
            }

            PlayerData playerData = new PlayerData(
                PlayerStat.Instance,
                Toolbar.instance,
                InventoryController.instance
            );

            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"Game Saved Successfully! Path: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        string fileToLoad = File.Exists(saveFilePath) ? saveFilePath : backupFilePath;

        if (!File.Exists(fileToLoad))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        try
        {
            string json = File.ReadAllText(fileToLoad);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            UpdatePlayerStats(playerData);
            UpdateToolbar(playerData.toolbarItems);
            UpdateInventory(playerData.inventoryItems);

            Debug.Log($"Game Loaded Successfully from {fileToLoad}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
        }
    }

    private void UpdatePlayerStats(PlayerData playerData)
    {
        var player = PlayerStat.Instance;
        player.coins = playerData.coins;
        player.energy = playerData.energy;
        player.maxEnergy = playerData.maxEnergy;
    }

    private void UpdateToolbar(List<ItemData> toolbarItems)
    {
        var toolbar = Toolbar.instance;
        toolbar.toolbarItems.Clear();
        UpdateItemsList(toolbarItems, toolbar.toolbarItems);
    }

    private void UpdateInventory(List<ItemData> inventoryItems)
    {
        var inventory = InventoryController.instance;
        inventory.inventoryItems.Clear();
        UpdateItemsList(inventoryItems, inventory.inventoryItems);
    }

    private void UpdateItemsList(List<ItemData> sourceItems, List<Item> targetList)
    {
        foreach (var itemData in sourceItems)
        {
            Item item = InventoryController.instance.GetItemByName(itemData.itemName);
            if (item != null)
            {
                UpdateItemFromData(item, itemData);
                targetList.Add(item);
            }
        }
    }

    private void UpdateItemFromData(Item item, ItemData itemData)
    {
        item.quantity = itemData.quantity;
        item.icon = itemData.GetIcon();
        item.toolType = (ToolType)System.Enum.Parse(typeof(ToolType), itemData.toolType);
        item.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.itemType);
        item.description = itemData.description;
        item.toolPrefab = itemData.GetPrefab();
    }
}