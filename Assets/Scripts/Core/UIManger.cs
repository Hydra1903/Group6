using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Singleton instance
    //TimeManager
    // Tham chiếu đến các phần tử UI
    public Text timeText; // Thời gian
    //InventoryManager
    public Text currencyText;      // Text để hiển thị số tiền trên UI

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        // Kiểm tra xem instance đã tồn tại chưa
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ nguyên UIManager khi chuyển Scene
            SceneManager.sceneLoaded += OnSceneLoaded; // Đăng ký sự kiện khi Scene được tải
        }
        else
        {
            Destroy(gameObject); // Xóa đối tượng mới nếu instance đã tồn tại
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      
    }

    // Phương thức cập nhật thời gian hiển thị trên UI
    public void UpdateTimeDisplay(string currentTime)
    {
        if (timeText != null)
        {
            timeText.text = "Time: " + currentTime;
        }
        else
        {
            Debug.LogWarning("Time Text UI not found in the current Scene.");
        }
    }
    public void UpdateCurrencyDisplay(string currency)
    {
        if (currencyText != null)
        {
            currencyText.text = currency;
        }
        else
        {
            Debug.LogWarning("Currency Text UI not found in the current Scene.");
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi UIManager bị hủy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}



