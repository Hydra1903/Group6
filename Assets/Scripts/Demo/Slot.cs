﻿using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;  // Vật phẩm hiện tại trong slot
    public GameObject highlightImage;  // Highlight hiển thị khi slot được chọn
    public static Slot selectedSlot;  // Lưu trữ slot được chọn gần nhất

    private Item item;  // Tham chiếu đến vật phẩm (nếu có)
    private Player player;  // Tham chiếu đến Player

    private void Start()
    {
        highlightImage.SetActive(false); // Ẩn highlight mặc định
        player = Player.instance;  // Lấy instance của Player
    }

    // Gắn vật phẩm vào slot
    public void SetItem(Item newItem)
    {
        item = newItem;
    }

    public void OnClick()
    {
        // Tắt highlight slot trước đó (nếu có)
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.Deselect();
        }

        // Chọn slot hiện tại và hiển thị highlight
        Select();
        selectedSlot = this;

        // Nếu slot có vật phẩm và vật phẩm là công cụ
        if (item != null && item.itemType == ItemType.Tool)
        {
            player.ToggleTool(item);  // Kích hoạt công cụ tương ứng
        }
    }

    public void Select()
    {
        if (highlightImage != null)
        {
            highlightImage.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (highlightImage != null)
        {
            highlightImage.SetActive(false);
        }
    }
}
