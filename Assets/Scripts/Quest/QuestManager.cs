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
            }
            else
            {
                Debug.Log("All quests completed!");
                currentQuest = null;
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
        if (currentQuest != null && !currentQuest.isCompleted)
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
    }


}

