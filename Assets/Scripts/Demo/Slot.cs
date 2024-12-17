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
        // Nếu slot không có vật phẩm, không làm gì
        if (currentItem == null) return;

        // Tắt highlight slot trước đó (nếu có)
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.Deselect();
        }

        // Highlight slot hiện tại
        Select();
        selectedSlot = this;
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
