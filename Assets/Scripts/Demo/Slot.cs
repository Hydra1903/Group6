using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;
    public GameObject highlightImage;  // Highlight hiển thị khi slot được chọn
    public static Slot selectedSlot;  // Lưu trữ slot được chọn gần nhất

    public Item item;  // Tham chiếu đến vật phẩm (nếu có)
    private Player player;  // Tham chiếu đến Player

    public Text quantityText; // Text để hiển thị số lượng item

    private void Start()
    {
        highlightImage.SetActive(false); // Ẩn highlight mặc định
        player = Player.instance;  // Lấy instance của Player
        UpdateQuantityText(); // Cập nhật số lượng ban đầu
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateQuantityText();
    }

    public void OnClick()
    {
        // Nếu slot đã được chọn, bỏ chọn nó
        if (selectedSlot == this)
        {
            Deselect();
            selectedSlot = null;
            return;
        }

        // Tắt highlight slot trước đó (nếu có)
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
        }

        // Chọn slot hiện tại
        Select();
        selectedSlot = this;

        // Kích hoạt công cụ nếu có item
        if (item != null)
        {
            player.ToggleTool(item);
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

    private void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            if (item != null && item.quantity > 1)
            {
                quantityText.text = item.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        item = null;
        UpdateQuantityText();
        Deselect();
    }
}
