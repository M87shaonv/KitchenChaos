using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += HandleIngredientAdded;
        foreach (var so in kitchenObjectSO_GameObjectList)
            so.gameObject.SetActive(false);
    }

    private void HandleIngredientAdded(KitchenObjectSO So)
    {
        foreach (var so in kitchenObjectSO_GameObjectList.Where(so => so.kitchenObjectSO == So))
        {
            so.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        plateKitchenObject.OnIngredientAdded -= HandleIngredientAdded;
    }
}