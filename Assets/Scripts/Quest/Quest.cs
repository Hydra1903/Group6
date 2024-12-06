using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    DigSoil,    // Đào đất
    SowSeeds,   // Gieo hạt
    WaterSoil,  // Tưới nước
    TalkToNPC,   // Nói chuyện với NPC
    HarvestCrop // Thu hoạch 
}

[System.Serializable]
public class Quest
{
    public string questName;          // Tên nhiệm vụ
    public string description;        // Mô tả nhiệm vụ
    public string requirementDescription; // Mô tả yêu cầu cụ thể

    public int targetCount;           // Số lượng yêu cầu cần đạt (vd: đào 5 ô đất)
    public int currentCount;          // Tiến trình hiện tại của người chơi
    public bool isCompleted;          // Trạng thái nhiệm vụ
    public bool isAccepted;           // Trạng thái đã nhận nhiệm vụ

    public ActionType actionType;       // Loại hành động cần thực hiện

    public List<Item> rewards;        // Phần thưởng (vật phẩm, kinh nghiệm, tiền)

    public string targetId;           // ID của mục tiêu cụ thể (vd: carrot, tomato, ...)

    public int moneyReward;           // Tiền thưởng
    public int experienceReward;      // Kinh nghiệm thưởng

    public void CompleteObjective(ActionType action, string id = null)
    {
        // Chỉ thực hiện nếu nhiệm vụ chưa hoàn thành và hành động khớp
        if (actionType == action && !isCompleted)
        {
            // Kiểm tra loại mục tiêu (nếu có)
            if (targetId != null && id != null && id != targetId)
            {
                Debug.Log($"Nhiệm vụ yêu cầu {targetId}, nhưng bạn thực hiện với {id}. Không hợp lệ.");
                return;
            }

            // Cập nhật tiến trình
            currentCount++;
            Debug.Log($"Cập nhật tiến trình {questName}: {currentCount}/{targetCount}");

            if (currentCount >= targetCount)
            {
                isCompleted = true;
                Debug.Log($"Nhiệm vụ {questName} đã hoàn thành!");
            }
        }
    }
}