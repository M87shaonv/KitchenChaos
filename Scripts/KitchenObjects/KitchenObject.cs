using System;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent KitchenObjectParent;
    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (kitchenObjectParent is Player)
            AudioManager.Instance.PlaySound(AudioManager.Instance.GetAudioClip().objectPickup, 0.1f);
        else
        {
            if (kitchenObjectParent is TrashCounter)
                AudioManager.Instance.PlaySound(AudioManager.Instance.GetAudioClip().trash, 0.1f);
            else
                AudioManager.Instance.PlaySound(AudioManager.Instance.GetAudioClip().objectDrop, 0.1f);
        }

        this.KitchenObjectParent?.ClearKitchenObject();
        this.KitchenObjectParent = kitchenObjectParent;
        this.KitchenObjectParent.SetKitchenObject(this);
        transform.SetParent(KitchenObjectParent.GetKitchenObjectTransform());
        transform.localPosition = Vector3.zero;
    }

    public void RemoveKitchenObject()
    {
        this.KitchenObjectParent?.ClearKitchenObject();
        this.KitchenObjectParent?.SetKitchenObject(this);
        transform.SetParent(null);
        transform.localPosition = Vector3.zero;
        Destroy(this.gameObject);
    }

    public static KitchenObject CreateKitchenObject(KitchenObjectSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }

    public IKitchenObjectParent GetKitchenObjectParent() => KitchenObjectParent;
}