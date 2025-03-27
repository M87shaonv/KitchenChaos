using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectTransform();
    public void SetKitchenObject(KitchenObject kitchen);
    public KitchenObject GetKitchenObject();
    public void ClearKitchenObject();
    public bool HasKitchenObject();
}