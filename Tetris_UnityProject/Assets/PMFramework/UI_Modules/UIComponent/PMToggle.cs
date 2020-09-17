using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PMToggle : PMUIComponent, IPointerClickHandler
{



    [SerializeField]
    GameObject _openImage;
    [SerializeField]
    GameObject _closeImage;
    bool _isOn;
    [SerializeField]
    AudioClip _audioClip;

    public UnityEventOnValueChanged OnValueChanged;


    public bool IsOn
    {
        get
        {
            return _isOn;
        }
        set
        {
            if (_isOn == value) return;
            _isOn = value;
            _openImage.SetActive(_isOn);
            _closeImage.SetActive(!_isOn);
            OnValueChanged?.Invoke(_isOn);
        }
    }




    private void Awake()
    {
        _openImage.SetActive(_isOn);
        _closeImage.SetActive(!_isOn);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;
        PlayAudio(_audioClip);
        IsOn = !IsOn;
    }



    [Serializable]
    public class UnityEventOnValueChanged : UnityEvent<bool> { }
}
