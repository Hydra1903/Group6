using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSlot : MonoBehaviour
{
    public Image highlightImage; // Image dùng để highlight
    private static HighlightSlot selectedSlot; // Lưu ô đang được chọn

    private void Start()
    {
        // Đảm bảo highlight tắt mặc định
        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    public void OnSlotClick()
    {
        // Tắt highlight của ô trước đó
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.Deselect();
        }

        // Chọn ô này
        Select();
        selectedSlot = this;
    }

    private void Select()
    {
        if (highlightImage != null)
            highlightImage.enabled = true;
    }

    private void Deselect()
    {
        if (highlightImage != null)
            highlightImage.enabled = false;
    }
}

