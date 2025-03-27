using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public GameObject selectedObject;
    [SerializeField] protected Transform counterApex;
    protected KitchenObject kitchenObject;

    public virtual void Interact(PlayerInteraction player) { }
    public virtual Transform GetKitchenObjectTransform() => counterApex;

    public virtual void SetKitchenObject(KitchenObject kitchen) => this.kitchenObject = kitchen;

    public virtual KitchenObject GetKitchenObject() => kitchenObject;
    public virtual void ClearKitchenObject() => kitchenObject = null;

    public virtual bool HasKitchenObject() => kitchenObject != null;
}