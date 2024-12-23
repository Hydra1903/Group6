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

    private Item item;
    private System.Action<Item> onBuy;

    public void SetUp(Item item, System.Action<Item> onBuyCallback)
    {
        this.item = item;
        itemNameText.text = item.itemName;
        itemIcon.sprite = item.icon;
        itemPriceText.text = item.price.ToString();

        onBuy = onBuyCallback;

        buyButton.onClick.AddListener(() => onBuy(item));
    }

}