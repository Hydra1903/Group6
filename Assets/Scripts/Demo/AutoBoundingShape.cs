using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBoundingShape : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Virtual Camera trong scene
    public PolygonCollider2D boundingShape; // Bounding Shape 2D (Polygon Collider 2D hoặc Composite Collider 2D)

    void Start()
    {
        // Nếu Virtual Camera chưa được gắn, tự động tìm trong scene
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        // Nếu Bounding Shape chưa được gắn, tự động tìm Collider 2D trong scene
        if (boundingShape == null)
        {
            boundingShape = FindObjectOfType<PolygonCollider2D>();
        }

        // Kiểm tra và gán Bounding Shape cho CinemachineConfiner
        if (virtualCamera != null && boundingShape != null)
        {
            var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.m_BoundingShape2D = boundingShape;
                Debug.Log("Bounding Shape đã được gán tự động!");
            }
            else
            {
                Debug.LogWarning("Virtual Camera không có CinemachineConfiner.");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Virtual Camera hoặc Bounding Shape trong scene.");
        }
    }
}
