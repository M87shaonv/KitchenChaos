using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    private readonly int color = Animator.StringToHash("4EE73E");
    [SerializeField] private PlateKitchenObject plate;
    [SerializeField] private Transform[] iconPositions;
    [SerializeField] private List<PlateCompleteVisual.KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

    private void Start()
    {
        plate.OnIngredientAdded += HandleIngredientIcon;
        foreach (var so in kitchenObjectSO_GameObjectList)
            so.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        plate.OnIngredientAdded -= HandleIngredientIcon;
    }
    private void HandleIngredientIcon(KitchenObjectSO So)
    {
        foreach (var so in kitchenObjectSO_GameObjectList.Where(so => so.kitchenObjectSO == So))
            so.gameObject.SetActive(true);
    }
}