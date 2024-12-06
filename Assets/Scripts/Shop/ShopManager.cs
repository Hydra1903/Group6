using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static List<ShopPanel> activeShops = new List<ShopPanel>();

    public static void RegisterShop(ShopPanel shop)
    {
        if (!activeShops.Contains(shop))
        {
            activeShops.Add(shop);
        }
    }

    public static void UnregisterShop(ShopPanel shop)
    {
        if (activeShops.Contains(shop))
        {
            activeShops.Remove(shop);
        }
    }

    public static void ResetAllShopsStock()
    {
        foreach (var shop in activeShops)
        {
            shop.ResetDailyStock();
        }
    }
}

