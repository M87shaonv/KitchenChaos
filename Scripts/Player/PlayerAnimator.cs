using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Player player;
    private readonly int IsWalking = Animator.StringToHash("isWalking");
    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        player.OnWalkingAction += PlayerMoveAnim;
    }
    private void PlayerMoveAnim(bool isMoving)
    {
        animator.SetBool(IsWalking, isMoving);
    }
    private void OnDestroy()
    {
        player.OnWalkingAction -= PlayerMoveAnim;
    }
}