using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public GameObject currentItem;
    public static Slot selectedSlot;
    public Item item;
    private Player player;

    [SerializeField] private GameObject highlightImage;
    [SerializeField] private Text quantityText;
    [SerializeField] private Image itemIcon;

    private void Start()
    {
        highlightImage.SetActive(false);
        player = Player.instance;
        UpdateUI();
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ItemDragHandler draggedItem = eventData.pointerDrag.GetComponent<ItemDragHandler>();
            if (draggedItem != null)
            {
                Slot sourceSlot = draggedItem.GetComponentInParent<Slot>();
                if (sourceSlot != null && sourceSlot != this)
                {
                    // Kiểm tra nếu là cùng loại item
                    if (item != null && sourceSlot.item != null && item.itemName == sourceSlot.item.itemName)
                    {
                        // Cộng số lượng
                        CombineQuantity(sourceSlot);
                    }
                    else
                    {
                        // Hoán đổi items nếu khác loại
                        SwapItems(sourceSlot);
                    }
                }
            }
        }
    }

    private void CombineQuantity(Slot sourceSlot)
    {
        // Cộng số lượng từ source slot
        item.quantity += sourceSlot.item.quantity;

        // Clear source slot
        sourceSlot.ClearSlot();

        // Cập nhật UI
        UpdateUI();
    }

    private void SwapItems(Slot sourceSlot)
    {
        GameObject tempCurrentItem = currentItem;
        Item tempItem = item;

        currentItem = sourceSlot.currentItem;
        item = sourceSlot.item;
        if (currentItem != null)
        {
            currentItem.transform.SetParent(transform);
            currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        sourceSlot.currentItem = tempCurrentItem;
        sourceSlot.item = tempItem;
        if (tempCurrentItem != null)
        {
            tempCurrentItem.transform.SetParent(sourceSlot.transform);
            tempCurrentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        UpdateUI();
        sourceSlot.UpdateUI();
    }

    public void OnClick()
    {
        if (selectedSlot == this)
        {
            Deselect();
            selectedSlot = null;
            return;
        }

        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
        }

        Select();
        selectedSlot = this;

        if (item != null && player != null)
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

    private void UpdateUI()
    {
        // Cập nhật icon
        if (itemIcon != null)
        {
            if (item != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }

        // Cập nhật số lượng và kiểm tra số lượng âm
        UpdateQuantityText();

        // Kiểm tra và xử lý nếu quantity <= 0
        if (item != null && item.quantity <= 0)
        {
            ClearSlot();
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
        UpdateUI();
        Deselect();
    }

    public bool CanAcceptItem(Item newItem)
    {
        // Kiểm tra slot trống
        if (item == null) return true;

        // Kiểm tra cùng loại item
        if (item.itemName == newItem.itemName) return true;

        return false;
    }
}