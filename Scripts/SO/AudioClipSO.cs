using UnityEngine;

[CreateAssetMenu]
public class AudioClipSO : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] deliveryFailed;
    public AudioClip[] deliverySuccess;
    public AudioClip[] footsteps;
    public AudioClip[] objectDrop; //放下物品
    public AudioClip[] objectPickup; //拿起物品
    public AudioClip[] stoveSizzle; //火炉烧焦声
    public AudioClip[] trash;
    public AudioClip[] waring;
}