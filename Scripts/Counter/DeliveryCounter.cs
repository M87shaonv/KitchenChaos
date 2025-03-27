using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public event Action<PlateKitchenObject> OnDelivery; // 物品交付事件
    private List<KitchenObject> movingObjects = new List<KitchenObject>();
    [SerializeField] private Vector3 moveDirection; // 移动方向
    [SerializeField] private float moveSpeed; // 移动速度
    [SerializeField] private float destroyTime; // 物品销毁时间
    private PlateKitchenObject plateKitchenObject; // 物品对象

    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) //如果玩家身上有KitchenObject
        {
            if (player.GetKitchenObject() is PlateKitchenObject)
            {
                plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject; // 获取物品对象
                plateKitchenObject.SetKitchenObjectParent(this); //将PlateKitchenObject的父物体设置为DeliveryCounter
                movingObjects.Add(kitchenObject); // 添加到移动列表
                StartCoroutine(DestroyObject()); // 销毁物品
            }
        }
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(destroyTime); // 等待销毁时间
        if (!movingObjects.Contains(kitchenObject))
            yield break;
        // 如果物品已经被销毁，则返回

        movingObjects.Remove(kitchenObject);
        OnDelivery?.Invoke(plateKitchenObject); // 触发物品交付事件
        kitchenObject.RemoveKitchenObject(); // 移除物品 
    }

    private void Update()
    {
        // 遍历所有需要移动的物品
        for (int i = movingObjects.Count - 1; i >= 0; i--)
        {
            KitchenObject movingObject = movingObjects[i];
            if (movingObject != null)
                movingObject.transform.position += moveDirection * (moveSpeed * Time.deltaTime); // 更新物品的位置
        }
    }
}