using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) // 如果玩家有 KitchenObject
        {
            if (HasKitchenObject()) // 当前柜台上有KitchenObject
            {
                if (player.GetKitchenObject() is PlateKitchenObject) // 如果玩家拿着盘子
                {
                    PlateKitchenObject plate = player.GetKitchenObject() as PlateKitchenObject; // 获取盘子
                    if (plate != null && plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                        GetKitchenObject().RemoveKitchenObject(); // 尝试从柜台上移除KitchenObject
                }

                if (GetKitchenObject() is PlateKitchenObject) //如果柜台上有盘子
                {
                    PlateKitchenObject plate = GetKitchenObject() as PlateKitchenObject; // 获取盘子
                    if (plate != null && plate.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        player.GetKitchenObject().RemoveKitchenObject(); // 尝试从玩家手中移除KitchenObject
                }
            }
            else //将KitchenObject从玩家手中移到柜台上 
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else // 如果玩家没有 KitchenObject
        {
            if (HasKitchenObject()) //当前柜台上有KitchenObject
                kitchenObject.SetKitchenObjectParent(player); // 将 KitchenObject 从柜台上移到玩家手中
        }
    }
}