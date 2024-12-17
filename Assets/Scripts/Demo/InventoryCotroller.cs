using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel; // Panel chứa các slot
    public GameObject slotPrefab;     // Prefab của slot
    public int slotCount;             // Số lượng slot trong inventory
    public GameObject[] itemPrefabs;  // Các prefab của vật phẩm

    private void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            // Tạo slot và gắn vào panel
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();

            // Nếu có item tương ứng, thêm item vào slot
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                item.GetComponent<ItemDragHandler>(); // Gắn script kéo thả cho item
                slot.currentItem = item; // Gán item cho slot
            }
        }
    }
}
