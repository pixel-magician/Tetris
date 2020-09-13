using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PMButton : PMUIComponent, IPointerClickHandler
{
    [SerializeField]
    AudioClip _audioClip;
    public UnityEvent OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;
        PlayAudio(_audioClip);
        OnClick?.Invoke();
    }
}
