using UnityEngine;
using UnityEngine.UI;


public class QuestUI : MonoBehaviour
{
    public Text questNameText;
    public Text questDescriptionText;
    public Text questProgressText;
    public Text questRewardsText; // Hiển thị phần thưởng

    private void Update()
    {
        if (QuestManager.instance.currentQuest != null)
        {
            Quest quest = QuestManager.instance.currentQuest;
            questNameText.text = quest.questName;

            if (!quest.isAccepted)
            {
                // Nếu nhiệm vụ chưa được nhận
                questDescriptionText.text = quest.description;
                questProgressText.text = ""; // Không hiển thị tiến trình
                questRewardsText.text = $"Phần thưởng: {GetRewardsText(quest)}";
            }
            else
            {
                // Nếu nhiệm vụ đã nhận
                questDescriptionText.text = quest.requirementDescription;
                questRewardsText.text = ""; // Ẩn phần thưởng
                if (quest.currentCount >= quest.targetCount)
                {
                    // Nếu nhiệm vụ hoàn thành
                    questProgressText.text = $"✔ Tiến trình: {quest.currentCount}/{quest.targetCount}";
                    questProgressText.color = Color.green;
                }
                else
                {
                    // Nếu chưa hoàn thành
                    questProgressText.text = $"Tiến trình: {quest.currentCount}/{quest.targetCount}";
                    questProgressText.color = Color.red;
                }
            }
        }
        else
        {
            // Khi không còn nhiệm vụ
            questNameText.text = "Hiện tại đã hết nhiệm vụ";
            questDescriptionText.text = "";
            questProgressText.text = "";
            questRewardsText.text = "";
        }
    }

    private string GetRewardsText(Quest quest)
    {
        string rewards = "";
        foreach (var reward in quest.rewards)
        {
            rewards += $"{reward.itemName}, ";
        }
        if (quest.moneyReward > 0)
        {
            rewards += $"{quest.moneyReward} tiền, ";
        }
        if (quest.experienceReward > 0)
        {
            rewards += $"{quest.experienceReward} kinh nghiệm, ";
        }
        return rewards.TrimEnd(',', ' ');
    }
}


