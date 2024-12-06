using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayReward : MonoBehaviour
{
    public static DisplayReward instance;
    public GameObject floatingRewardPrefab;  // Tham chiếu tới Prefab phần thưởng
    public float floatUpDistance = 2f;      // Khoảng cách bay lên
    public float displayDuration = 2f;      // Thời gian hiển thị

    // Hiển thị phần thưởng văn bản
    public void ShowFloatingReward(string message, Sprite icon)
    {
        if (floatingRewardPrefab == null)
        {
            Debug.LogWarning("Floating Reward Prefab chưa được gắn.");
            return;
        }

        // Tạo ra một instance của Prefab
        GameObject rewardInstance = Instantiate(floatingRewardPrefab, transform.position, Quaternion.identity);

        // Gắn Prefab vào Canvas hoặc chính Player
        rewardInstance.transform.SetParent(GameObject.Find("Canvas_WorldSpace").transform, false);

        // Gán vị trí của reward ngay trên đầu Player
        rewardInstance.transform.position = transform.position + new Vector3(0, 0.5f, 0);

        // Tìm và cập nhật Text và Image của Prefab
        UnityEngine.UI.Text rewardText = rewardInstance.GetComponentInChildren<UnityEngine.UI.Text>();
        UnityEngine.UI.Image rewardImage = rewardInstance.GetComponentInChildren<UnityEngine.UI.Image>();

        if (rewardText != null)
        {
            rewardText.text = message;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Text trong Prefab.");
        }

        if (rewardImage != null && icon != null)
        {
            rewardImage.sprite = icon;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Image trong Prefab hoặc icon null.");
        }
        // Gọi Coroutine để di chuyển phần thưởng và hủy sau thời gian
        StartCoroutine(FloatAndDestroy(rewardInstance));
        //// Tự động hủy sau 2-3 giây
        //Destroy(rewardInstance, 2f);
    }


    public void ReceiveGold(int amount)
    {
        Debug.Log("Nhận được vàng: " + amount);
        ShowFloatingReward("+" + amount + " Gold", null);  // Không cần icon cho vàng
    }

    public void ReceiveItem(Item item)
    {
        Debug.Log("Nhận được item: " + item.itemName);
        ShowFloatingReward("+" + item.itemName, item.icon);  // Hiển thị tên item và icon
    }
    private System.Collections.IEnumerator FloatAndDestroy(GameObject rewardInstance)
    {
        Vector3 startPosition = rewardInstance.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, floatUpDistance, 0);

        float elapsedTime = 0f;

        // Di chuyển UI bay lên từ từ
        while (elapsedTime < displayDuration)
        {
            float t = elapsedTime / displayDuration;
            rewardInstance.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Xóa đối tượng sau khi hiển thị xong
        Destroy(rewardInstance);
    }

}
