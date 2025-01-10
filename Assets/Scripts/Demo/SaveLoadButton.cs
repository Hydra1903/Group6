using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadButton : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        // Tải game khi bắt đầu
        saveLoadManager.LoadGame();
    }

    public void OnSaveButtonPressed()
    {
        saveLoadManager.SaveGame();
    }

    public void OnLoadButtonPressed()
    {
        saveLoadManager.LoadGame();
    }
}
