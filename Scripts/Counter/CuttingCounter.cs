using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSo;
    private Animator animator;
    private bool isCutted;
    private float cutProgress; //切割进度
    private Coroutine cuttingCoroutine; //切割协程
    private float cutTime; //切割时间
    private readonly int CUT = Animator.StringToHash("Cut");

    public event Action<float> OnCutProgressChanged; //切割进度改变事件

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
        isCutted = false;
        cutTime = cutProgress = 0;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) //如果玩家身上有KitchenObject
        {
            if (player.GetKitchenObject() is PlateKitchenObject) // 如果玩家拿着盘子
            {
                PlateKitchenObject plate = player.GetKitchenObject() as PlateKitchenObject; // 获取盘子
                if (isCutted)
                {
                    if (plate != null && plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().RemoveKitchenObject(); // 尝试从柜台上移除KitchenObject
                        isCutted = false;
                        cutProgress = 0;
                    }
                }
            }

            if (!HasKitchenObject() &&
                HasOutputForRecipe(player.GetKitchenObject()
                    .GetKitchenObjectSO())) //如果当前没有KitchenObject,且KitchenObject可以切割
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else //如果玩家身上没有KitchenObject
        {
            if (!HasKitchenObject()) return; //如果当前没有KitchenObject
            if (isCutted)
            {
                kitchenObject.SetKitchenObjectParent(player);
                isCutted = false;
                cutProgress = 0;
            }
            else if (cuttingCoroutine == null) // 只有在cuttingCoroutine为null时才启动新的协程
            {
                cutTime = GetCutTime(GetKitchenObject().GetKitchenObjectSO());
                cuttingCoroutine = StartCoroutine(Cutting());
            }
        }
    }

    public void StopCutting()
    {
        if (cuttingCoroutine == null) return;
        StopAnim();
        AudioManager.Instance.StopSound(AudioManager.Instance.CutAudioSource);
        StopCoroutine(cuttingCoroutine);
        cuttingCoroutine = null;
    }

    private IEnumerator Cutting()
    {
        PlayAnim();
        AudioManager.Instance.PlaySoundLoop(AudioManager.Instance.GetAudioClip().chop,
            AudioManager.Instance.CutAudioSource, 0.1f);
        while (cutProgress < cutTime)
        {
            cutProgress += Time.deltaTime;
            OnCutProgressChanged?.Invoke((float)cutProgress / cutTime);
            yield return null;
        }

        Cut();
    }

    private void Cut()
    {
        StopAnim();
        AudioManager.Instance.StopSound(AudioManager.Instance.CutAudioSource);
        if (cuttingCoroutine != null)
            StopCoroutine(cuttingCoroutine);
        GetKitchenObject().RemoveKitchenObject();
        KitchenObjectSO output = GetOutputForRecipe(GetKitchenObject().GetKitchenObjectSO());
        KitchenObject.CreateKitchenObject(output, this);
        isCutted = true;
        cutProgress = 0;
    }

    private bool HasOutputForRecipe(KitchenObjectSO input)
    {
        return cuttingRecipeSo.Any(c => c.inputObject == input);
    }

    private KitchenObjectSO GetOutputForRecipe(KitchenObjectSO input)
    {
        foreach (var recipe in cuttingRecipeSo)
        {
            if (recipe.inputObject == input)
                return recipe.outputObject;
        }

        return null;
    }

    private float GetCutTime(KitchenObjectSO input)
    {
        foreach (var recipe in cuttingRecipeSo)
        {
            if (recipe.inputObject == input)
                return recipe.CutTime;
        }

        return 0;
    }

    private void PlayAnim() => animator.SetBool(CUT, true);
    private void StopAnim() => animator.SetBool(CUT, false);
}