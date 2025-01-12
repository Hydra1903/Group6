using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuLoadGame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "GameScene"; // Tên scene game sẽ load vào
    [SerializeField] private Button loadGameButton; // Button để load game

    private static bool shouldLoadGame = false;
    private static MenuLoadGame instance;

    private void Awake()
    {
        // Đảm bảo object này không bị hủy khi chuyển scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Thêm listener cho button
        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(LoadGameFromMenu);

            // Check xem có file save không để enable/disable button
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, "playerSaveData.json");
            loadGameButton.interactable = System.IO.File.Exists(savePath);
        }
    }

    private void LoadGameFromMenu()
    {
        shouldLoadGame = true;
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldLoadGame && scene.name == gameSceneName)
        {
            shouldLoadGame = false;

            // Tìm SaveLoadManager trong scene mới
            SaveLoadManager saveLoadManager = FindObjectOfType<SaveLoadManager>();
            if (saveLoadManager != null)
            {
                saveLoadManager.LoadGame();
            }

            // Hủy object này sau khi load xong
            Destroy(gameObject);
        }
    }
}