using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineLoad : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Virtual Camera dùng chung

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm Bounding Shape trong scene mới
        var boundingShape = FindObjectOfType<PolygonCollider2D>();
        if (virtualCamera != null && boundingShape != null)
        {
            var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.m_BoundingShape2D = boundingShape;
                Debug.Log($"Bounding Shape đã được gán cho scene: {scene.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy Virtual Camera hoặc Bounding Shape trong scene: {scene.name}");
        }
    }
}
