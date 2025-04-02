using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Dictionary<string, AudioClip> mAudioClip;
    [SerializeField] private AssetLabelReference audioClipPrefabLabel;
    [SerializeField] private AudioClipData audioClipCatalogue;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] List<AudioClip> audioClipPlayeds;

    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource bgmSource;
    public bool preLoad = false;
    [SerializeField] bool isMusicOn = true;
    [SerializeField] bool isSoundOn = true;
  //  [SerializeField] UserData userData;



    void Start()
    {
       // UserDataManager.Instance.OnDataLoaded += SetUserData;
        //if (preLoad)
            LoadAudioClipPrefab();

        isMusicOn = true;
        isSoundOn = true;
    }

    public void SetUserData()
    {
       // userData = UserDataManager.Instance.UserData;
       // userData.sound.OnStatusChange += SoundStatus;
        SoundStatus();
    }


    public void LoadAudioClipPrefab()
    {
        audioClipCatalogue.Clear();
        Addressables.LoadResourceLocationsAsync(audioClipPrefabLabel.labelString, null).Completed +=
            OnLoadDataLocationSuccess;
    }

    private void OnLoadDataLocationSuccess(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.IsDone)
        {
            StartCoroutine(FillData(handle.Result));
        }
    }

    IEnumerator FillData(IList<IResourceLocation> resourceLocations)
    {
        foreach (var item in resourceLocations)
        {
            var tmp = Addressables.LoadAssetAsync<AudioClip>(item);
            yield return tmp;
            Debug.Log(item.ToString());
            audioClipCatalogue.Add(tmp.Result, item);
            audioClips.Add(tmp.Result);
            Debug.Log(audioClipCatalogue.audioClipAssets.Count);
            Debug.Log(audioClips.Count);
        }
    }

    public void SoundStatus()
    {
       // isMusicOn = userData.sound.MusicStatus;
      //  isSoundOn = userData.sound.SoundStatus;
        if (!isMusicOn)
        {
            if (bgmSource.isPlaying)
                bgmSource.Pause();
        }
        else
        {
            if (bgmSource.isPlaying)
                bgmSource.UnPause();
            else
                bgmSource.Play();
        }
    }


    public void PlaySound(SoundName audioClipName, float volumne)
    {
/*        if (!isSoundOn) return;
        else
        {*/
            var audioClip = audioClips.Find(x => x.name == audioClipName.ToString());
            if (audioClip)
            {
                sfxSource.PlayOneShot(audioClip, volumne);
            }
            else
            {
                StartCoroutine(DelayPlay(audioClipName, volumne, true));
            }

    }

    public void StopSfx()
    {
        sfxSource.Stop();
    }

    public void PlayMusic(SoundName audioClipName, float volumne)
    {
        var audioClip = audioClips.Find(x => x.name == audioClipName.ToString());
        if (audioClip)
        {
            bgmSource.clip = audioClip;
            bgmSource.volume = volumne;
            if (isMusicOn)
                bgmSource.Play();
        }
        else
        {
            StartCoroutine(DelayPlay(audioClipName, volumne, false));
        }
    }


    private IEnumerator DelayPlay(SoundName audioClipName, float volumne, bool isSfx) 
    {
        var tmp = Addressables.LoadAssetAsync<AudioClip>(audioClipCatalogue.GetItem(audioClipName.ToString()).path);
        yield return tmp;

        if (isSfx)
            sfxSource.PlayOneShot(tmp.Result, volumne);
        else
        {
            bgmSource.clip = tmp.Result;
            bgmSource.volume = volumne;
            if (isMusicOn)
                bgmSource.Play();
        }
    }
}


public enum SoundName
{
    Explosion_1,
    Explosion_2,
    Gun_Shoot_1,
    Gun_Shoot_2,
    Gun_Shoot_3,
    Missile_Shoot_1,
    Blood_Splat_1
}