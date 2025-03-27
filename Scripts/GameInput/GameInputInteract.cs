using System;
using UnityEngine;

public class GameInputInteract : MonoBehaviour
{
    public event Action OnInteract; //互动事件
    public KeyCode InteractKey = KeyCode.E; //互动按键
    internal void InputPlayerInteractions()
    {
        if (Input.GetKeyDown(InteractKey))
            OnInteract?.Invoke();
    }
}