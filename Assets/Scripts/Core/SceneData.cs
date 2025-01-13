using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Instance;
    public Vector2 targetPosition; // Vị trí tùy chỉnh trong scene mới.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ đối tượng này khi chuyển scene.
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

