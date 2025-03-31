using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "AudioClipData", order = 0)]
public class AudioClipData : ScriptableObject
{
    [SerializeField] List<AudioClipAsset> audioClipAssets;
    public void Add(AudioClip item, IResourceLocation location)
    {
        audioClipAssets.Add(new AudioClipAsset(item, location));
        //PopupCatalogueItem abc
    }
    public AudioClipAsset GetItem(string name)
    {
        foreach (var item in audioClipAssets)
        {
            if (item.key.Equals(name))
                return item;
        }
        return null;
    }
    public void Clear()
    {
        audioClipAssets.Clear();
    }

}

[Serializable]
public class AudioClipAsset
{
    public string key;
    public AssetReference path;

    public AudioClipAsset(AudioClip item, IResourceLocation location)
    {
#if UNITY_EDITOR
        Debug.Log(item.name);
        this.key = item.name;


        var guid = AssetDatabase.GUIDFromAssetPath(location.InternalId);
        this.path = new AssetReference(guid.ToString());
#endif
    }
    public AudioClipAsset(int id, IResourceLocation location)
    {
#if UNITY_EDITOR
        this.key = id.ToString();
        var guid = AssetDatabase.GUIDFromAssetPath(location.InternalId);
        this.path = new AssetReference(guid.ToString());
#endif
    }
}