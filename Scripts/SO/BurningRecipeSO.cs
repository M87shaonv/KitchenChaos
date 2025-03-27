using UnityEngine;

[CreateAssetMenu]
public class BurningRecipeSO : ScriptableObject
{
    public KitchenObjectSO inputObject;
    public KitchenObjectSO outputObject;
    public float BurningTime;
}