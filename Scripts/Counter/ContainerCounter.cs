using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO KitchenObjectSO;
    private Animator animator;
    private const string OPEN_CLOSE = "OpenClose";

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) return; //如果玩家手中有KitchenObject
        PlayAnim();
        KitchenObject.CreateKitchenObject(KitchenObjectSO, player); //拿到玩家手中
    }

    private void PlayAnim()
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}