using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Text itemNameText;
    public Image itemIcon;
    public Text itemPriceText;
    public Button buyButton;
    public Button sellButton;

    private Item item;
    private System.Action<Item> onBuy;
    private System.Action<Item> onSell;

    public void SetUp(Item item, System.Action<Item> onBuyCallback, System.Action<Item> onSellCallback)
    {
        this.item = item;
        itemNameText.text = item.itemName;
        itemIcon.sprite = item.icon;
        itemPriceText.text = item.price.ToString();

        onBuy = onBuyCallback;
        onSell = onSellCallback;

        buyButton.onClick.AddListener(() => onBuy(item));
        sellButton.onClick.AddListener(() => onSell(item));
    }
}
