using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class OrderManager : MonoBehaviour
{
    public enum RecipeName
    {
        汉堡包,
        奶酪饼,
        生蔬饼,
        番茄饼,
        烤肉,
        牛肉面包,
        奶酪面包,
        生蔬面包,
        番茄面包,
    }

    [SerializeField] private Image remainingTimeImage;
    [SerializeField] private Text TipText;
    [SerializeField] private DeliveryCounter deliveryCounter;
    [SerializeField] private GameObject orderPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform orderParent;
    [SerializeField] private List<RecipeSO> recipes;
    [SerializeField] private List<RecipeSO> currentOrders;
    private const int maxOrderCount = 4;
    private const float orderInterval = 15f;
    private float remainingTime; // 剩余时间
    private int completedOrderCount = 0; // 已完成订单数
    private Dictionary<RecipeSO, GameObject> orderUIs; // 保存订单与UI对象的映射

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += StartCreateOrder;
        orderUIs = new Dictionary<RecipeSO, GameObject>();
        deliveryCounter.OnDelivery += HandleDelivery;
        InitOrders();
        completedOrderCount = 0;
        remainingTime = 60f;
        TipText.text = "订单列表   完成订单数:" + completedOrderCount;
        remainingTimeImage.fillAmount = remainingTime / 60f;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= StartCreateOrder;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying() == false) return;
        remainingTime -= Time.deltaTime;
        TipText.text = "订单列表   完成订单数:" + completedOrderCount;
        remainingTimeImage.fillAmount = remainingTime / 60f;

        if (remainingTime <= 0)
        {
            GameManager.Instance.GameOver(completedOrderCount);
        }
    }

    private void HandleDelivery(PlateKitchenObject obj)
    {
        // 获取交付的产品列表
        List<KitchenObjectSO> deliverKitchenObjectSOList = obj.GetKitchenObjectSOList();
        // 检查交付的产品是否与当前订单中的某一个一致
        foreach (var order in currentOrders)
        {
            if (AreListsEqual(order.kitchenObjectSOList, deliverKitchenObjectSOList))
            {
                // 匹配成功，处理订单完成逻辑
                Debug.Log("订单匹配成功: " + order.recipeName);
                TipText.text = "订单列表    完成订单数:" + completedOrderCount;
                remainingTime += 20f;
                AudioManager.Instance.PlaySound(AudioManager.Instance.GetAudioClip().deliverySuccess, 0.3f);
                currentOrders.Remove(order);
                if (orderUIs.ContainsKey(order))
                {
                    GameObject orderObj = orderUIs[order];
                    StartCoroutine(DestroyOrder(orderObj)); // 使用协程渐渐消失然后销毁它
                    orderUIs.Remove(order);
                }

                return;
            }
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.GetAudioClip().deliveryFailed, 0.3f);
        // 如果没有匹配的订单，处理交付失败逻辑
        Debug.Log("交付失败，没有匹配的订单");
    }

    private void StartCreateOrder()
    {
        if (GameManager.Instance.IsGamePlaying())
            StartCoroutine(CreateOrder());
    }

    private IEnumerator DestroyOrder(GameObject orderObj)
    {
        CanvasGroup canvasGroup = orderObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = orderObj.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1.0f; // 渐隐时间
        float elapsedTime = 0f;
        ++completedOrderCount; // 已完成订单数+1
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        Destroy(orderObj); // 销毁对象
    }

    private IEnumerator FadeInOrder(GameObject orderObj)
    {
        CanvasGroup canvasGroup = orderObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = orderObj.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 1.0f; // 渐显时间
        float elapsedTime = 0f;
        canvasGroup.alpha = 0f; // 初始透明度为0

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // 确保最终透明度为1
    }

    private bool AreListsEqual(List<KitchenObjectSO> list1, List<KitchenObjectSO> list2)
    {
        if (list1.Count != list2.Count)
            return false;
        list1.Sort();
        list2.Sort();
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }

    private void InitOrders()
    {
        int ordersToAdd = maxOrderCount - currentOrders.Count; // 仅添加缺少的订单数量
        for (int i = 0; i < ordersToAdd; i++)
        {
            int orderIndex = GetUniqueRecipe();
            RecipeSO recipe = recipes[orderIndex];
            GameObject orderObj = Instantiate(orderPrefab, orderParent); // 创建订单
            foreach (var item in recipe.kitchenObjectSOList)
            {
                GameObject itemObj = Instantiate(itemPrefab, orderObj.transform); // 创建订单中的项
                itemObj.transform.GetChild(0).GetComponent<Image>().sprite = item.sprite; // 设置项的图片
            }

            currentOrders.Add(recipe); // 将新订单添加到当前订单列表
            orderUIs.Add(recipe, orderObj); // 保存订单与UI对象的映射
            StartCoroutine(FadeInOrder(orderObj)); // 使用协程渐显
        }
    }

    private IEnumerator CreateOrder()
    {
        yield return new WaitForSeconds(orderInterval);
        if (currentOrders.Count >= maxOrderCount)
        {
            yield return StartCoroutine(CreateOrder());
            yield break;
        }

        InitOrders();
        yield return StartCoroutine(CreateOrder());
    }

    private int GetUniqueRecipe()
    {
        int orderIndex;
        RecipeSO recipe;
        do
        {
            orderIndex = UnityEngine.Random.Range(0, recipes.Count);
            recipe = recipes[orderIndex];
        } while (currentOrders.Contains(recipe));

        return orderIndex;
    }

    private string GetRemainingTime() => Mathf.Ceil(remainingTime).ToString();
}