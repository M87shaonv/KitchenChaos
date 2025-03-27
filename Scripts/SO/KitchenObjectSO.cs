using System;
using UnityEngine;

[CreateAssetMenu]
public class KitchenObjectSO : ScriptableObject, IComparable<KitchenObjectSO>
{
    public Transform prefab;
    public Sprite sprite;
    public string name;
    public string description;

    public int CompareTo(KitchenObjectSO other)
    {
        return string.Compare(name, other.name, StringComparison.Ordinal); // 根据需要选择比较的字段
    }
}