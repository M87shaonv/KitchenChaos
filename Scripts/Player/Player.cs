using System;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    internal GameInputInteract gameInputInteract;
    internal GameInputMovement gameInputMovement;
    internal PlayerInteraction playerInteraction;
    public float playerRadius = 0.7f;
    public float playerHeight = 2;
    public event Action<bool> OnWalkingAction;

    private void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        gameInputMovement = GetComponent<GameInputMovement>();
        gameInputInteract = GetComponent<GameInputInteract>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying() is not true) return;

        gameInputMovement.HandlePlayerMovement();
        OnWalkingAction?.Invoke(gameInputMovement.isWalking);
        gameInputInteract.InputPlayerInteractions();
        playerInteraction.CheckInInteractRange();
    }
}