using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public Text questNameText;
    public Text questDescriptionText;
    public Text questProgressText;

    private void Update()
    {
        if (QuestManager.instance.currentQuest != null)
        {
            Quest quest = QuestManager.instance.currentQuest;
            questNameText.text = quest.questName;
            questDescriptionText.text = quest.description;
            questProgressText.text = $"{quest.currentCount}/{quest.targetCount}";
        }
        else
        {
            questNameText.text = "No active quest";
            questDescriptionText.text = "";
            questProgressText.text = "";
        }
    }
}

