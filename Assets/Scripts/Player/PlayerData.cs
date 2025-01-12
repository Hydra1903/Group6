using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int energy;
    public int maxEnergy;
    public List<ItemData> toolbarItems;
    public List<ItemData> inventoryItems;

    public PlayerData(PlayerStat playerStat, Toolbar toolbar, InventoryController inventory)
    {
        coins = playerStat.coins;
        energy = playerStat.energy;
        maxEnergy = playerStat.maxEnergy;

        toolbarItems = ConvertItemsToData(toolbar.toolbarItems);
        inventoryItems = ConvertItemsToData(inventory.inventoryItems);
    }

    private List<ItemData> ConvertItemsToData(List<Item> items)
    {
        List<ItemData> dataList = new List<ItemData>();
        foreach (Item item in items)
        {
            if (item != null)
            {
                dataList.Add(new ItemData(item));
            }
        }
        return dataList;
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string spritePath; // Đường dẫn đến sprite trong Resources
    public int quantity;
    public int price;
    public int energy;
    public string description;
    public string toolType;
    public string itemType;
    public string prefabPath;

    public ItemData(Item item)
    {
        if (item == null) return;

        itemName = item.itemName;
        quantity = item.quantity;
        price = item.price;
        energy = item.energy;
        description = item.description;
        toolType = item.toolType.ToString();
        itemType = item.itemType.ToString();

        // Lưu đường dẫn tương đối trong thư mục Resources
        if (item.icon != null)
        {
            string path = GetResourcePath(item.icon);
            spritePath = path ?? "";
        }

        if (item.toolPrefab != null)
        {
            string path = GetResourcePath(item.toolPrefab);
            prefabPath = path ?? "";
        }
    }

    private string GetResourcePath(Object obj)
    {
        if (obj == null) return null;

        // Thử lấy đường dẫn của asset
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(assetPath)) return null;

        // Tìm vị trí của thư mục Resources trong đường dẫn
        int resourcesIndex = assetPath.IndexOf("Resources/");
        if (resourcesIndex == -1) return null;

        // Lấy đường dẫn tương đối từ thư mục Resources
        string relativePath = assetPath.Substring(resourcesIndex + 10); // 10 là độ dài của "Resources/"

        // Loại bỏ phần mở rộng của file (.png, .prefab, etc.)
        return Path.ChangeExtension(relativePath, null);
    }

    public Sprite GetIcon()
    {
        if (string.IsNullOrEmpty(spritePath)) return null;
        return Resources.Load<Sprite>(spritePath);
    }

    public GameObject GetPrefab()
    {
        if (string.IsNullOrEmpty(prefabPath)) return null;
        return Resources.Load<GameObject>(prefabPath);
    }
}