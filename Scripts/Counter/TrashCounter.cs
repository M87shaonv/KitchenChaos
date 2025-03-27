using UnityEngine;

public class TrashCounter : BaseCounter
{
    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) // 如果玩家有 KitchenObject
        {
            player.GetKitchenObject().SetKitchenObjectParent(this);
            kitchenObject.RemoveKitchenObject(); // 移除 KitchenObject
        }
    }
}