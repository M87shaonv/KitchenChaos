using System;
using UnityEngine;

public class StoveVisualEffect : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveParticles;
    [SerializeField] private GameObject stoveFire;

    private void Start()
    {
        stoveCounter.OnStoveStateChanged += HandleStoveStateChange;
    }
    private void OnDestroy()
    {
        stoveCounter.OnStoveStateChanged -= HandleStoveStateChange;
    }
    private void HandleStoveStateChange(StoveCounter.StoveState state)
    {
        bool showParticles = state is not StoveCounter.StoveState.Idle;
        stoveParticles.SetActive(showParticles);
        stoveFire.SetActive(showParticles);
    }
}