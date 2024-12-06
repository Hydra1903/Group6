using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // Singleton để truy cập từ bất kỳ đâu

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject continueButton;
    public GameObject shopButton;
    //public GameObject questButton;
    public Text npcNameText;
    private string currentNpc;
   

    private GameObject currentShopPanel;  // Shop Panel hiện tại
    //private GameObject currentQuestPanel; // Quest Panel hiện tại


    [Header("Settings")]
    public float wordSpeed = 0.05f; // Tốc độ hiển thị từng ký tự

    private string[] currentDialogue; // Mảng hội thoại hiện tại
    private int dialogueIndex; // Chỉ số của câu thoại hiện tại
    private Coroutine typingCoroutine; // Dùng để quản lý coroutine hiển thị từng ký tự

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        ResetOptions();
    }

    /// <summary>
    /// Bắt đầu hiển thị hội thoại từ đầu
    /// </summary>
    /// <param name="dialogue">Mảng hội thoại cần hiển thị</param>
    public void StartDialogue(string[] dialogue, GameObject shopPanel, string npcName)//, GameObject questPanel)
    {
        ResetDialogue();

        currentDialogue = dialogue;
        currentShopPanel = shopPanel;   // Lưu tham chiếu ShopPanel của NPC
        //currentQuestPanel = questPanel; // Lưu tham chiếu QuestPanel của NPC
        currentNpc = npcName;

        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeDialogue());
    }
    /// <summary>
    /// Hiển thị câu thoại tiếp theo hoặc kết thúc hội thoại nếu đã đến câu cuối
    /// </summary>
    public void NextDialogue()
    {
        if (dialogueIndex < currentDialogue.Length - 1)
        {
            dialogueIndex++;
            dialogueText.text = "";
            continueButton.SetActive(false);

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeDialogue());
        }
        else
        {
            EndDialogue(); // Kết thúc hội thoại khi hết câu
        }
    }

    /// <summary>
    /// Kết thúc hội thoại và hiển thị các nút tùy chọn
    /// </summary>
    public void EndDialogue()
    {
        continueButton.SetActive(false);
        ShowOptions(); // Hiển thị các nút Option khi kết thúc
    }

    /// <summary>
    /// Reset lại trạng thái hội thoại
    /// </summary>
    private void ResetDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        currentDialogue = null;
        dialogueIndex = 0;

        currentShopPanel = null;  // Reset ShopPanel
        //currentQuestPanel = null; // Reset QuestPanel
        ResetOptions();
    }

    /// <summary>
    /// Hiển thị từng ký tự của câu thoại hiện tại
    /// </summary>
    private IEnumerator TypeDialogue()
    {
        foreach (char letter in currentDialogue[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        // Hiển thị nút Continue nếu không phải câu cuối
        if (dialogueIndex < currentDialogue.Length - 1)
        {
            continueButton.SetActive(true);
        }
        else
        {
            //continueButton.SetActive(false); // Ẩn nút Continue ở câu cuối
            EndDialogue();
        }
    }

    /// <summary>
    /// Hiển thị các tùy chọn (Shop, Quest) khi hội thoại kết thúc
    /// </summary>
    private void ShowOptions()
    {
        // Chỉ hiển thị các nút nếu có panel tương ứng
        shopButton.SetActive(currentShopPanel != null);
        //questButton.SetActive(currentQuestPanel != null);
    }
    public void OpenShop()
    {
        if (currentShopPanel != null)
        {
            currentShopPanel.SetActive(true);
            CloseDialogue(); // Đóng hội thoại khi mở Shop
        }
    }

    //public void OpenQuest()
    //{
    //    if (currentQuestPanel != null)
    //    {
    //        currentQuestPanel.SetActive(true);
    //        CloseDialogue(); // Đóng hội thoại khi mở Quest
    //    }
    //}

    /// <summary>
    /// Ẩn các tùy chọn (Shop, Quest)
    /// </summary>
    private void ResetOptions()
    {
        shopButton.SetActive(false);
        //questButton.SetActive(false);
    }

    /// <summary>
    /// Tắt hội thoại và reset trạng thái
    /// </summary>
    public void CloseDialogue()
    {
        ResetDialogue();
        dialoguePanel.SetActive(false );
    }
    public void UpdateNpcNameDisplay(string name)
    {
        if (npcNameText != null)
        {
            npcNameText.text = name;
        }
        else
        {
            Debug.LogWarning("Currency Text UI not found in the current Scene.");
        }
    }
}

