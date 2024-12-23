using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;  // Vật phẩm hiện tại trong slot
    public GameObject highlightImage;    // Highlight hiển thị khi slot được chọn
    public static Slot selectedSlot; // Lưu trữ slot được chọn gần nhất

    private void Start()
    {
        highlightImage.SetActive(false); // Ẩn highlight mặc định

    }

    public void OnClick()
    {

        // Tắt highlight slot trước đó (nếu có)
        Item item = currentItem.GetComponent<Item>();

        // Kiểm tra nếu vật phẩm là công cụ
        if (item.itemType == ItemType.Tool)
        {
            // Nếu vật phẩm là công cụ, gọi ToggleTool để kích hoạt công cụ
            Player.instance.ToggleTool(item);

            // Tắt highlight slot trước đó (nếu có)
            if (selectedSlot != null && selectedSlot != this)
            {
                selectedSlot.Deselect();
            }

            // Chọn slot hiện tại và hiển thị highlight
            Select();
            selectedSlot = this;
        }
        else
        {
            // Nếu vật phẩm không phải công cụ, tắt công cụ
            Player.instance.DeactivateTool();

            // Tắt highlight của slot trước đó (nếu có)
            if (selectedSlot != null && selectedSlot != this)
            {
                selectedSlot.Deselect();
            }

            // Bỏ chọn slot hiện tại
            selectedSlot = null;
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
