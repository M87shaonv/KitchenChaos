using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IKitchenObjectParent
{
    private const float interactDistance = 1;
    [SerializeField] private LayerMask countersLayer;
    private Vector3 lastInteractDir;
    private Player player;
    private float playerHeight;
    private float playerRadius;
    private BaseCounter selectedCounter; //当前选中的柜台
    private KitchenObject kitchenObject; //当前选中的物品
    [SerializeField] private Transform KitchenHoldPoint; // 玩家当前持有的物品应该显示的位置

    private void Awake()
    {
        player = GetComponent<Player>();
        playerRadius = player.playerRadius;
        playerHeight = player.playerHeight;
    }

    private void Start()
    {
        player.gameInputInteract.OnInteract += HandleInteractions;
    }

    private void OnDestroy()
    {
        player.gameInputInteract.OnInteract -= HandleInteractions;
    }

    private void HandleInteractions()
    {
        if (selectedCounter == null) return;
        selectedCounter.Interact(this);
    }

    public void CheckInInteractRange() // 添加持续检查交互范围的逻辑
    {
        Vector3 inputVector = player.gameInputMovement.GetInputVector();
        if (inputVector != Vector3.zero)
            lastInteractDir = inputVector;

        BaseCounter newSelectedCounter = null;
        if (Physics.CapsuleCast(player.transform.position, player.transform.position + (Vector3.up * playerHeight),
                playerRadius, lastInteractDir, out RaycastHit hit, interactDistance, countersLayer))
            newSelectedCounter = hit.transform.GetComponent<BaseCounter>();

        // 检查selectedCounter是否需要更新
        {
            if (newSelectedCounter != selectedCounter)
            {
                if (selectedCounter != null)
                {
                    selectedCounter.selectedObject.SetActive(false);
                    switch (selectedCounter)
                    {
                        // 如果是切菜柜台，停止切菜
                        case CuttingCounter cuttingCounter:
                            cuttingCounter.StopCutting();
                            break;
                    }
                }

                selectedCounter = newSelectedCounter;

                if (selectedCounter != null) selectedCounter.selectedObject.SetActive(true);
            }
            else if (newSelectedCounter == null && selectedCounter != null)
            {
                // 如果新选中的柜台为null，确保之前的selectedCounter被禁用
                selectedCounter.selectedObject.SetActive(false);
                selectedCounter = null;
            }
        }
    }

    public Transform GetKitchenObjectTransform() => KitchenHoldPoint;

    public void SetKitchenObject(KitchenObject kitchen) => this.kitchenObject = kitchen;

    public KitchenObject GetKitchenObject()
    {
        if (kitchenObject == null) Debug.LogError("kitchenObject is null");
        return kitchenObject;
    }

    public void ClearKitchenObject() => kitchenObject = null;

    public bool HasKitchenObject() => kitchenObject != null;
}