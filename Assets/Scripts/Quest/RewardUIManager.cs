using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIManager : MonoBehaviour
{
    public static RewardUIManager instance;

    public GameObject rewardPanel;         // Panel hiển thị phần thưởng
    public Text titleText;                // Tiêu đề
    public Transform rewardItemsParent;   // Nơi chứa các phần tử phần thưởng (Horizontal Layout Group)
    public GameObject rewardItemPrefab;   // Prefab hiển thị mỗi phần thưởng
    public Button closeButton;            // Nút đóng

    public Sprite moneyIcon;

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
        rewardPanel.SetActive(false);
        closeButton.onClick.AddListener(() => rewardPanel.SetActive(false));
    }

    public void DisplayRewards(Quest quest)
    {
        // Hiển thị phần thưởng
        rewardPanel.SetActive(true);
        titleText.text = $"Phần Thưởng: {quest.questName}";

        // Xóa các phần tử phần thưởng cũ
        foreach (Transform child in rewardItemsParent)
        {
            Destroy(child.gameObject);
        }

        // Thêm các phần thưởng vật phẩm
        foreach (var reward in quest.rewards)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemsParent);
            Image itemImage = rewardItem.GetComponent<Image>();
            Text itemNameText = rewardItem.GetComponentInChildren<Text>();

            // Cập nhật hình ảnh và tên vật phẩm
            itemImage.sprite = reward.icon; // Đảm bảo `Item` có thuộc tính `icon` kiểu `Sprite`
            itemNameText.text = reward.itemName;
        }

        // Thêm tiền thưởng (nếu có)
        if (quest.moneyReward > 0)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemsParent);
            Image itemImage = rewardItem.GetComponent<Image>();
            Text itemNameText = rewardItem.GetComponentInChildren<Text>();

            itemImage.sprite = moneyIcon;
            itemNameText.text = $"+{quest.moneyReward} Tiền";
        }

        // Thêm kinh nghiệm thưởng (nếu có)
        if (quest.experienceReward > 0)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemsParent);
            Image itemImage = rewardItem.GetComponent<Image>();
            Text itemNameText = rewardItem.GetComponentInChildren<Text>();

            itemNameText.text = $"+{quest.experienceReward} Kinh Nghiệm";
        }
    }
}


