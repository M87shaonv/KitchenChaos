using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private float spawnPlateInterval;
    [SerializeField] private int maxPlateCount;
    List<Transform> plates = new();
    private const float plateOffsetX = .1f;

    private void Start()
    {
        StartCoroutine(SpawnPlates());
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) // 如果玩家有 KitchenObject
        { }
        else // 如果玩家没有 KitchenObject
        {
            if (plates.Count <= 0) return; // 如果桌子上没有盘子
            KitchenObject.CreateKitchenObject(plateKitchenObjectSO, player);
            RemovePlate();
        }
    }

    private IEnumerator SpawnPlates()
    {
        yield return new WaitForSeconds(spawnPlateInterval);
        if (plates.Count > maxPlateCount)
            yield return StartCoroutine(SpawnPlates());
        else
        {
            Transform plateVisual = Instantiate(plateVisualPrefab, counterApex);

            plateVisual.localPosition = new Vector3(0, plateOffsetX * plates.Count, 0);
            plates.Add(plateVisual);
            yield return StartCoroutine(SpawnPlates());
        }
    }

    private void RemovePlate()
    {
        Transform plate = plates[^1];
        plates.Remove(plate);
        Destroy(plate.gameObject);
    }
}