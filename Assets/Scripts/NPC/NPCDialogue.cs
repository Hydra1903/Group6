using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string npcId;

    [Header("NPC Data")]
    public NPCInfo npcData; // Dữ liệu hội thoại của NPC

    private bool playerIsClose;

   
    private void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Q) && playerIsClose)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(npcData.dialogue, npcData.shopPanel,npcData.npcName);//, npcData.questPanel);
                DialogueManager.Instance.UpdateNpcNameDisplay(UpdateNameUI());
                QuestManager.instance.UpdateQuestProgress(ActionType.TalkToNPC, npcId);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.CloseDialogue();
            }
        }
    }
    private string UpdateNameUI()
    {
        return string.Format($"{npcData.npcName:0}");
    }
}

