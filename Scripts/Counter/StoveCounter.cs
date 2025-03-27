using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    public enum StoveState
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] FryingRecipeSO;
    [SerializeField] private BurningRecipeSO[] BurningRecipeSO;
    private StoveState stoveState;
    private float cookProgress1; //烹饪进度1
    private float cookProgress2; //烹饪进度2
    private float cookTime; //烹饪时间
    private float burningTime;
    private AudioSource source;
    public event Action<StoveState> OnStoveStateChanged;
    public event Action<float, float> OnCookProgressChanged;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        stoveState = StoveState.Idle;
        burningTime = cookTime = cookProgress1 = cookProgress2 = 0;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) //如果玩家身上有KitchenObject
        {
            if (player.GetKitchenObject() is PlateKitchenObject) // 如果玩家拿着盘子
            {
                if (stoveState is StoveState.Frying) return;
                PlateKitchenObject plate = player.GetKitchenObject() as PlateKitchenObject; // 获取盘子
                if (plate != null && plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                {
                    GetKitchenObject().RemoveKitchenObject(); // 尝试从柜台上移除KitchenObject
                    StopCooking();
                    stoveState = StoveState.Idle;
                    cookProgress1 = cookProgress2 = 0;
                    OnCookProgressChanged?.Invoke(0f, 0f); // 重置进度条
                    OnStoveStateChanged?.Invoke(stoveState); // 通知状态改变
                }
            }

            if (!HasKitchenObject() &&
                HasOutputForRecipe(player.GetKitchenObject()
                    .GetKitchenObjectSO())) //如果当前没有KitchenObject,且KitchenObject可以烹饪
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cookTime = GetFryingTime(GetKitchenObject().GetKitchenObjectSO());
                StartCoroutine(Cooking());
                stoveState = StoveState.Frying;
                OnStoveStateChanged?.Invoke(stoveState);
            }
        }
        else //如果玩家身上没有KitchenObject
        {
            if (!HasKitchenObject() || stoveState is StoveState.Frying)
                return; //如果当前没有KitchenObject或在Frying状态下
            StopCooking();
            stoveState = StoveState.Idle;
            cookProgress1 = cookProgress2 = 0;
            OnCookProgressChanged?.Invoke(0f, 0f); // 重置进度条
            OnStoveStateChanged?.Invoke(stoveState); // 通知状态改变
            kitchenObject.SetKitchenObjectParent(player);
        }
    }


    private IEnumerator Cooking()
    {
        PlaySoundLoop(AudioManager.Instance.GetAudioClip().stoveSizzle, 0.1f);
        while (cookProgress1 < cookTime)
        {
            OnCookProgressChanged?.Invoke(cookProgress1 / cookTime, 0f);
            cookProgress1 += Time.deltaTime;
            yield return null;
        }
        Cook();
    }

    private IEnumerator Burned()
    {
        PlaySoundLoop(AudioManager.Instance.GetAudioClip().stoveSizzle, 0.2f);
        burningTime = GetBurningTime(GetKitchenObject().GetKitchenObjectSO());
        while (cookProgress2 < burningTime)
        {
            OnCookProgressChanged?.Invoke(1f, cookProgress2 / burningTime);
            cookProgress2 += Time.deltaTime;
            yield return null;
        }

        stoveState = StoveState.Burned;
        cookProgress1 = cookProgress2 = 0;
        OnStoveStateChanged?.Invoke(stoveState);
        kitchenObject.RemoveKitchenObject();
        KitchenObjectSO output = GetBurningOutputForRecipe(GetKitchenObject().GetKitchenObjectSO());
        KitchenObject.CreateKitchenObject(output, this);
    }

    private void Cook()
    {
        stoveState = StoveState.Fried;
        OnStoveStateChanged?.Invoke(stoveState);
        cookProgress1 = 0;
        GetKitchenObject().RemoveKitchenObject();
        KitchenObjectSO output = GetFryingOutputForRecipe(GetKitchenObject().GetKitchenObjectSO());
        KitchenObject.CreateKitchenObject(output, this);
        StartCoroutine(Burned());
    }

    private bool HasOutputForRecipe(KitchenObjectSO input)
    {
        return FryingRecipeSO.Any(c => c.inputObject == input);
    }

    private KitchenObjectSO GetFryingOutputForRecipe(KitchenObjectSO input)
    {
        foreach (var recipe in FryingRecipeSO)
        {
            if (recipe.inputObject == input)
                return recipe.outputObject;
        }

        return null;
    }

    private KitchenObjectSO GetBurningOutputForRecipe(KitchenObjectSO input)
    {
        return (from recipe in BurningRecipeSO where recipe.inputObject == input select recipe.outputObject)
            .FirstOrDefault();
    }

    private float GetBurningTime(KitchenObjectSO input)
    {
        foreach (var recipe in BurningRecipeSO)
        {
            if (recipe.inputObject == input)
                return recipe.BurningTime;
        }

        return 0;
    }

    private float GetFryingTime(KitchenObjectSO input)
    {
        return (from recipe in FryingRecipeSO where recipe.inputObject == input select recipe.FryingTime)
            .FirstOrDefault();
    }

    private void StopCooking()
    {
        StopSound();
        StopAllCoroutines();
    }
    private void PlaySoundLoop(AudioClip[] clips, float volume = 1f)
    {
        source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        source.volume = volume;
        source.loop = true;
        source.Play();
    }
    private void StopSound() => source.Stop();
}