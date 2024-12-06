using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> quests = new List<Quest>(); // Danh sách nhiệm vụ
    public Quest currentQuest;                    // Nhiệm vụ hiện tại
    public GameObject claimeReward;
    public GameObject acceptQuestButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        claimeReward.SetActive(false);
        // Bắt đầu với nhiệm vụ đầu tiên
        if (quests.Count > 0)
        {
            currentQuest = quests[0];
            DisplayQuest();
            acceptQuestButton.SetActive(true);
        }
    }
    public void AcceptQuest()
    {
        if (currentQuest != null && !currentQuest.isAccepted)
        {
            currentQuest.isAccepted = true;
            Debug.Log($"Nhận nhiệm vụ: {currentQuest.questName}");
            acceptQuestButton.SetActive(false); // Ẩn nút nhận nhiệm vụ
        }
    }
    private void Update()
    {
        if (currentQuest != null && currentQuest.isCompleted)
        {
            claimeReward.SetActive(true);
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuest != null && currentQuest.isCompleted)
        {
            Debug.Log($"Completed: {currentQuest.questName}");
            GiveRewards(currentQuest.rewards);

            // Hiển thị UI phần thưởng
            RewardUIManager.instance.DisplayRewards(currentQuest);

            // Chuyển sang nhiệm vụ tiếp theo
            int currentIndex = quests.IndexOf(currentQuest);
            if (currentIndex + 1 < quests.Count)
            {
                currentQuest = quests[currentIndex + 1];
                DisplayQuest();
                claimeReward.SetActive(false);
                acceptQuestButton.SetActive(true);
            }
            else
            {
                Debug.Log("All quests completed!");
                currentQuest = null;
                claimeReward.SetActive(false);
                acceptQuestButton.SetActive(false);
            }
        }
    }

    private void GiveRewards(List<Item> rewards)
    {
        foreach (var reward in rewards)
        {
            Debug.Log($"Received: {reward.itemName}");
            // Thêm logic nhận thưởng (cập nhật tiền, vật phẩm, kinh nghiệm,...)
            InventoryManager.instance.AddItem(reward);
        }
        if (currentQuest.moneyReward > 0)
        {
            InventoryManager.instance.AddCurrency(currentQuest.moneyReward);
            Debug.Log($"Received {currentQuest.moneyReward} tiền!");
        }

        // Phát thưởng kinh nghiệm
        if (currentQuest.experienceReward > 0)
        {
            PlayerStat.Instance.AddExperience(currentQuest.experienceReward);
            Debug.Log($"Received {currentQuest.experienceReward} kinh nghiệm!");
        }
    }

    private void DisplayQuest()
    {
        if (currentQuest != null)
        {
            Debug.Log($"New Quest: {currentQuest.questName}");
            // Cập nhật giao diện nhiệm vụ tại đây

        }
    }

    public void UpdateQuestProgress(ActionType action, string targetId = null)
    {
        // Chỉ cập nhật nếu nhiệm vụ hiện tại đã được nhận và chưa hoàn thành
        if (currentQuest != null && currentQuest.isAccepted && !currentQuest.isCompleted)
        {
            if (currentQuest.actionType == action)
            {
                currentQuest.CompleteObjective(action, targetId);
                Debug.Log($"Cập nhật nhiệm vụ: {currentQuest.questName}, Tiến trình: {currentQuest.currentCount}/{currentQuest.targetCount}");

                if (currentQuest.isCompleted)
                {
                    Debug.Log($"Nhiệm vụ {currentQuest.questName} đã hoàn thành!");
                    claimeReward.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("Không có nhiệm vụ hiện tại hoặc nhiệm vụ chưa được nhận!");
        }
    }

}

