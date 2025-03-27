using System;
using UnityEngine;

public class GameInputMovement : MonoBehaviour
{
    public static event Action<bool> OnPauseAction;
    private const float moveSpeed = 6f; //可以打翻的东西会降低速度
    private const float rotateSpeed = 9f;
    internal bool isWalking;
    private bool wasWalking; // 用于记录上一次的状态
    private Player player;
    private float playerHeight;
    private float playerRadius;
    private bool isPaused;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        playerRadius = player.playerRadius;
        playerHeight = player.playerHeight;
        isWalking = false;
        isPaused = false;
    }

    /// <summary>
    ///     获取玩家的输入方向
    /// </summary>
    public Vector3 GetInputVector()
    {
        Vector3 inputVector = Vector3.zero;
        if (Input.GetKey(upKey)) inputVector.z -= 1;
        if (Input.GetKey(downKey)) inputVector.z += 1;
        if (Input.GetKey(leftKey)) inputVector.x += 1;
        if (Input.GetKey(rightKey)) inputVector.x -= 1;
        return new Vector3(inputVector.x, 0, inputVector.z).normalized;
    }


    /// <summary>
    ///     如果没有障碍物，则可以移动, 返回真则可以移动，返回假不能移动
    /// </summary>
    /// <param name="playerPos"> 玩家的位置</param>
    /// <param name="moveDir"> 玩家的移动方向</param>
    /// <param name="moveDistance"> 玩家的移动距离</param>
    private bool CanMove(Vector3 playerPos, Vector3 moveDir, float moveDistance)
    {
        return !Physics.CapsuleCast(playerPos, playerPos + (Vector3.up * playerHeight), playerRadius, moveDir,
            moveDistance);
    }

    internal void HandlePlayerMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pause");
            isPaused = !isPaused;
            OnPauseAction?.Invoke(isPaused);
        }

        Vector3 moveDir = GetInputVector();
        Transform playerTransform = player.transform;
        Vector3 playerPos = playerTransform.position;
        var moveDistance = Time.deltaTime * moveSpeed;
        player.transform.forward
            = Vector3.Slerp(playerTransform.forward, moveDir, Time.deltaTime * rotateSpeed); //旋转玩家朝向

        if (!CanMove(playerPos, moveDir, moveDistance)) //如果玩家的位置和输入方向之间有障碍物，则无法向指定方向移动,因此尝试仅在x轴上移动
        {
            Vector3 moveDirX = new(moveDir.x, 0, 0);
            if (moveDir.x != 0 && CanMove(playerPos, moveDirX, moveDistance))
                moveDir = moveDirX.normalized;

            else //如果在x轴上也无法移动，则尝试仅在z轴上移动
            {
                Vector3 moveDirZ = new(0, 0, moveDir.z);
                if (moveDir.z != 0 && CanMove(playerPos, moveDirZ, moveDistance))
                    moveDir = moveDirZ.normalized;
            }
        }

        if (CanMove(playerPos, moveDir, moveDistance))
            playerTransform.position += moveDir * moveDistance;


        isWalking = moveDir != Vector3.zero; //如果玩家正在移动，则isWalking为true
    }
}