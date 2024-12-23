using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // Kiểm tra phím F được nhấn
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Nếu có slot được chọn và player đang cầm tool
            if (Slot.selectedSlot != null && Player.instance.isToolActive)
            {
              //  Player.instance.UseTool();
            }
        }
    }
}
