using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string toScene; // Scene đích
    public Vector2 targetPosition; // Vị trí nhân vật trong scene mới
    private Animator anim; // Animator cho hiệu ứng
    private bool isTransitioning = false; // Ngăn kích hoạt nhiều lần
    [SerializeField] private GameObject panel;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(Transition(collision));
        }
    }

    private IEnumerator Transition(Collider2D playerStat)
    {
        // Bắt đầu hiệu ứng Animator
        if (anim != null)
        {
            panel.SetActive(true);
            anim.SetTrigger("NextScene");
        }

        // Đợi hiệu ứng hoàn thành
        yield return new WaitForSeconds(1f); // Thời gian chờ theo độ dài hiệu ứng

        // Đặt vị trí nhân vật
        PlayerStat.Instance.transform.position = targetPosition;

        // Chuyển scene
        SceneManager.LoadScene(toScene);
    }
}
