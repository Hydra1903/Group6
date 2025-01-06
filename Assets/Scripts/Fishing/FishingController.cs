using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class FishingController : MonoBehaviour
{
    [Header("Thiết lập câu cá")]
    [SerializeField] private Transform fishingSpot;      // Điểm câu cá
    [SerializeField] private Transform playerPosition;   // Vị trí người chơi
    [SerializeField] private float maxFishingDistance = 1f;  // Khoảng cách tối đa
    [SerializeField] private float fishingWaitTime = 5f;     // Thời gian chờ cá
    [SerializeField] private List<GameObject> fishPrefabs;   // Danh sách cá

    [Header("Giao diện")]
    [SerializeField] private GameObject fishingResultPanel;  // Panel kết quả
    [SerializeField] private Image fishImage;               // Hình ảnh cá
    [SerializeField] private TextMeshProUGUI fishInfoText;  // Thông tin cá
    [SerializeField] private Slider fishingSlider;          // Thanh trượt
    [SerializeField] private RectTransform fishIcon;        // Icon cá
    [SerializeField] private RectTransform greenBar;        // Thanh xanh
    [SerializeField] private Slider progressBar;            // Thanh tiến độ

    [Header("Cài đặt minigame")]
    [SerializeField] private float requiredCatchProgress = 3f;
    [SerializeField] private float greenBarSpeed = 0.52f;
    [SerializeField] private float lowProgressThreshold = 3f;
    [SerializeField] private float fishHitboxSize = 0.1f;


    [Header("Animation")]
    public PlayerController playercontroller;
    [SerializeField] private float castingDuration = 1f;    // Thời gian animation ném cần
    [SerializeField] private float reelingDuration = 1.5f;  // Thời gian animation kéo
                                                            
    private bool isFishing;
    private bool isWaitingForFish;
    private const string BAIT_ITEM_NAME = "Spilua Bait";

    private static readonly Dictionary<Rarity, float> FishSpeedByRarity = new()
    {
        { Rarity.Common, 0.0025f },
        { Rarity.Uncommon, 0.0028f },
        { Rarity.Rare, 0.0032f },
        { Rarity.Legendary, 0.0038f }
    };

    private void Start()
    {
        InitializeUI();
        ValidateComponents();


    }

    private void Update()
    {
        // Chỉ cho phép bắt đầu câu khi không đang trong quá trình câu và đủ điều kiện
        if (Input.GetKeyDown(KeyCode.F) && !isFishing && CanStartFishing())
        {
            StartFishing();
        }

        // Cho phép hủy câu khi đang chờ cá
        if (Input.GetKeyDown(KeyCode.Escape) && isWaitingForFish)
        {
            CancelFishing();
        }
    }

    private bool CanStartFishing() =>
        IsNearWater() &&
        Player.instance.CanFishing() &&
        Toolbar.instance.HasItem(BAIT_ITEM_NAME);

    private bool IsNearWater() =>
        Vector2.Distance(playerPosition.position, fishingSpot.position) < maxFishingDistance;

    private void StartFishing()
    {
        StartCoroutine(FishingSequence());
        Toolbar.instance.RemoveItemName(BAIT_ITEM_NAME, 1);
    }

    private void CancelFishing()
    {
        if (isWaitingForFish)
        {
            StopAllCoroutines();
            ResetFishingState();
            // Reset về animation idle khi hủy
            if (playercontroller != null)
            {
                playercontroller.StartAction(PlayerController.PLAYER_IDLE);
            }
        }
    }

    private void ResetFishingState()
    {
        isFishing = false;
        isWaitingForFish = false;
        if (playercontroller != null)
        {
            playercontroller.EnableMovement();
            playercontroller.OnActionComplete();
        }
        SetupFishingUI(false);
    }
    private IEnumerator FishingSequence()
    {
        // Bắt đầu câu cá
        isFishing = true;
        isWaitingForFish = true;
        playercontroller.DisableMovement();

        playercontroller.StartAction(PlayerController.PLAYER_CASTING);
        yield return new WaitForSeconds(2);
        playercontroller.StartAction(PlayerController.PLAYER_WAITING);
        yield return new WaitForSeconds(5);

        isWaitingForFish = false;
        SetupFishingUI(true);

        // Bắt đầu minigame
        FishData selectedFish = SelectRandomFish();
        bool fishCaught = false;
        playercontroller.StartAction(PlayerController.PLAYER_REELING);

        if (selectedFish != null)
        {
            yield return StartCoroutine(FishingMinigame(selectedFish, (result) => fishCaught = result));
            // Animation kéo cá khi bắt được
           

            if (fishCaught)
            {
                // Animation bắt được cá
                playercontroller.StartAction(PlayerController.PLAYER_CAUGHT);
                yield return new WaitForSeconds(2f);
            }
        }

        // Xử lý kết quả
        SetupFishingUI(false);

        if (fishCaught)
        {
            yield return StartCoroutine(ShowFishingResult(selectedFish));
        }

        // Kết thúc và cho phép di chuyển
        ResetFishingState();
    }


    private IEnumerator FishingMinigame(FishData currentFishData, System.Action<bool> onComplete)
    {
        float fishSpeed = FishSpeedByRarity.GetValueOrDefault(currentFishData.rarity, FishSpeedByRarity[Rarity.Common]);
        float fishPosition = 0.5f;
        bool directionUp = true;
        float greenBarPosition = 0.5f;
        float catchProgress = 0f;
        float lowProgressTimer = 0f;

        while (catchProgress < requiredCatchProgress)
        {
            fishPosition = UpdateFishPosition(fishPosition, fishSpeed, ref directionUp);
            UpdateFishIconPosition(fishPosition);

            greenBarPosition = UpdateGreenBarPosition(greenBarPosition);
            UpdateGreenBarUIPosition(greenBarPosition);

            if (IsInCatchRange(fishPosition, greenBarPosition))
            {
                catchProgress += Time.deltaTime;
                lowProgressTimer = 0f;
            }
            else
            {
                catchProgress = Mathf.Max(0, catchProgress - Time.deltaTime);
                if (catchProgress <= 0.1f)
                {
                    lowProgressTimer += Time.deltaTime;
                    if (lowProgressTimer >= lowProgressThreshold) break;
                }
            }

            progressBar.value = catchProgress / requiredCatchProgress;
            yield return null;
        }

        onComplete(catchProgress >= requiredCatchProgress);
    }

    private float UpdateFishPosition(float currentPosition, float speed, ref bool goingUp)
    {
        if (goingUp)
        {
            currentPosition += speed;
            if (currentPosition >= 1) goingUp = false;
        }
        else
        {
            currentPosition -= speed;
            if (currentPosition <= 0) goingUp = true;
        }
        return currentPosition;
    }

    private void UpdateFishIconPosition(float position)
    {
        float newY = Mathf.Lerp(
            fishingSlider.GetComponent<RectTransform>().rect.min.y,
            fishingSlider.GetComponent<RectTransform>().rect.max.y,
            position
        );
        fishIcon.anchoredPosition = new Vector2(fishIcon.anchoredPosition.x, newY);
    }

    private float UpdateGreenBarPosition(float currentPosition)
    {
        float delta = Input.GetKey(KeyCode.Space) ? greenBarSpeed * Time.deltaTime : -greenBarSpeed * Time.deltaTime;
        return Mathf.Clamp(currentPosition + delta, 0f, 1f);
    }

    private void UpdateGreenBarUIPosition(float position)
    {
        float newY = Mathf.Lerp(
            fishingSlider.GetComponent<RectTransform>().rect.min.y,
            fishingSlider.GetComponent<RectTransform>().rect.max.y,
            position
        );
        greenBar.anchoredPosition = new Vector2(greenBar.anchoredPosition.x, newY);
    }

    private bool IsInCatchRange(float fishPos, float barPos) =>
        barPos >= fishPos - fishHitboxSize && barPos <= fishPos + fishHitboxSize;

    private FishData SelectRandomFish()
    {
        float totalChance = 0f;
        foreach (var fish in fishPrefabs)
        {
            if (fish.TryGetComponent<FishInstance>(out var instance) && instance.fishData != null)
            {
                totalChance += instance.fishData.catchChance;
            }
        }

        float roll = Random.Range(0f, totalChance);
        float cumulative = 0f;

        foreach (var fish in fishPrefabs)
        {
            if (fish.TryGetComponent<FishInstance>(out var instance) && instance.fishData != null)
            {
                cumulative += instance.fishData.catchChance;
                if (roll <= cumulative)
                {
                    return instance.fishData;
                }
            }
        }

        return null;
    }

    private void ProcessCaughtFish(FishData fishData)
    {
        var caughtFish = new Item(
            fishData.fishName,
            fishData.icon,
            fishData.quantity,
            fishData.itemType,
            fishData.toolType,
            fishData.price,
            fishData.energy,
            fishData.description,
            fishPrefabs.Find(f => f.GetComponent<FishInstance>()?.fishData == fishData)
        );

        Toolbar.instance.AddItemToToolbar(caughtFish, 1);
    }

    private IEnumerator ShowFishingResult(FishData fishData)
    {
        // Xử lý cá trước khi hiện kết quả
        ProcessCaughtFish(fishData);

        // Hiển thị panel kết quả
        if (fishData != null)
        {
            fishImage.sprite = fishData.icon;
            fishInfoText.text = $"Tên: {fishData.fishName}\nĐộ hiếm: {fishData.rarity}\n{fishData.description}";
        }
        else
        {
            fishImage.sprite = null;
            fishInfoText.text = "Chúc bạn may mắn lần sau!";
        }

        fishingResultPanel.SetActive(true);

        // Cho phép di chuyển ngay khi hiện kết quả
        if (playercontroller != null) playercontroller.EnableMovement();

        // Chờ một lúc rồi ẩn panel
        yield return new WaitForSeconds(3f);
        fishingResultPanel.SetActive(false);
    }

    private void InitializeUI()
    {
        fishingResultPanel.SetActive(false);
        fishingSlider.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }

    private void ValidateComponents()
    {
        if (fishingResultPanel == null) Debug.LogError("Thiếu Panel kết quả câu cá");
        if (fishImage == null) Debug.LogError("Thiếu hình ảnh cá");
        if (fishInfoText == null) Debug.LogError("Thiếu text thông tin cá");
        if (fishingSlider == null) Debug.LogError("Thiếu thanh trượt câu cá");
        if (fishIcon == null) Debug.LogError("Thiếu biểu tượng cá");
        if (greenBar == null) Debug.LogError("Thiếu thanh xanh");
        if (progressBar == null) Debug.LogError("Thiếu thanh tiến độ");
        if (playercontroller == null) Debug.LogWarning("Thiếu PlayerMovement - sẽ không thể khóa di chuyển");
    }

    private void SetupFishingUI(bool isActive)
    {
        fishingResultPanel.SetActive(false);
        fishingSlider.gameObject.SetActive(isActive);
        progressBar.gameObject.SetActive(isActive);
    }
}



