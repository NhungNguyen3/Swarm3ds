using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    [SerializeField] GameObject content;
    public virtual void Appear()
    {
/*        transform.SetAsLastSibling();
        content.gameObject.SetActive(true);*/
    }

    public virtual void Disappear()
    {
/*        content.gameObject.SetActive(false);
        SoundManager.Instance.PlaySound(SoundName.UI_Button_Click, GameConstants.UI_BTN_CLICK_VOLUMNE);*/
    }

}
