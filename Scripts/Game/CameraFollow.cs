using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1; // 第一个玩家
    public Transform player2; // 第二个玩家
    public float zoomFactor = 1.5f; // 缩放因子，用于调整摄像机的缩放
    public float smoothTime = 0.3f; // 平滑时间
    public float minFOV = 20f; // 最小视野
    public float maxFOV = 33f; // 最大视野

    private Vector3 velocity = Vector3.zero;
    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        // if (virtualCamera != null)
        // {
        //     // 确保 Follow 和 LookAt 属性设置正确
        //     virtualCamera.Follow = player1; // 或者你可以设置为中点
        //     virtualCamera.LookAt = player1; // 或者你可以设置为中点
        // }
        // else
        // {
        //     Debug.LogError("CinemachineVirtualCamera component not found on the game object.");
        // }
    }

    private void LateUpdate()
    {
        if (player1 != null && player2 != null)
        {
            // 计算两个玩家之间的中点
            Vector3 midpoint = (player1.position + player2.position) / 2f;

            // 计算两个玩家之间的距离
            float distance = Vector3.Distance(player1.position, player2.position);

            // 根据距离调整摄像机的缩放
            float targetZoom = Mathf.Clamp(distance * zoomFactor, minFOV, maxFOV);
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetZoom, Time.deltaTime);

            // 平滑移动摄像机到中点
            Vector3 targetPosition = new Vector3(midpoint.x, midpoint.y + 25, midpoint.z + 15);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}