using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu sử dụng TextMeshPro
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public int hours = 6;
    public int minutes = 0;
    public int days = 1;
    public int season = 1; // 0 = Spring, 1 = Summer, 2 = Fall, 3 = Winter
    public int year = 1;

    private float timeMultiplier = 60f;
    private float timer = 0f;

    public Light2D globalLight; // Ánh sáng toàn cảnh
    public Light2D playerLight; // Ánh sáng theo nhân vật


    [SerializeField] private Text timeText;  // Text để hiển thị thời gian
    [SerializeField] private Text seasonText; // Text để hiển thị mùa
    [SerializeField] private Text dayText; // Text để hiển thị ngày 
    [SerializeField] private Text yearText; // Text để hiển thị năm
    private string[] seasonNames = { "Spring", "Summer", "Fall", "Winter" };

    public static TimeManager instance;

    private void Update()
    {
        UpdateTime();
        UpdateLighting();
        if (timeText != null)
        {
            timeText.text = GetCurrentTime();
        }
        if (seasonText != null)
        {
            seasonText.text = GetCurrentSeason();
        }
        if (dayText != null)
        {
            dayText.text = GetCurrentDay();
        }    
        if (yearText != null)
        {
            yearText.text = GetCurrentYear();
        }
    }

    void UpdateTime()
    {
        timer += Time.deltaTime * timeMultiplier;

        if (timer >= 60f)
        {
            timer = 0f;
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
                if (hours >= 24)
                {
                    hours = 0;
                    days++;
                    if (days > 30)
                    {
                        days = 1;
                        season++;
                        if (season >= 4)
                        {
                            season = 0;
                            year++;
                        }
                    }
                }
            }
        }
    }

    void UpdateLighting()
    {
        if (hours >= 18 || hours < 6)
        {
            SetLighting(0.01f, Color.cyan, true);
        }
        else if (hours >= 6 && hours < 12)
        {
            SetLighting(1.2f, new Color(1f, 1f, 0.9f), false);
        }
        else if (hours >= 12 && hours < 16)
        {
            SetLighting(1.8f, new Color(1f, 0.95f, 0.6f), false);
        }
        else if (hours >= 16 && hours < 18)
        {
            SetLighting(1.5f, new Color32(217, 95, 140, 255), false);
        }
    }

    void SetLighting(float intensity, Color color, bool enablePlayerLight)
    {
        globalLight.intensity = intensity;
        globalLight.color = color;
        playerLight.enabled = enablePlayerLight;
    }

    private string GetCurrentTime()
    {
        return $"{hours:00}:{minutes:00}";
    }
    private string GetCurrentDay()
    {
        return $"{days}";
    }
    private string GetCurrentYear()
    {
        return $"{year}";
    }
    private string GetCurrentSeason()
    {
        return $"{seasonNames[season]}";
    }
}
