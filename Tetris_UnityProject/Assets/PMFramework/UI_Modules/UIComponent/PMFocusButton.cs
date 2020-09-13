using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


[RequireComponent(typeof(Animator))]
public class PMFocusButton : PMUIComponent, IPointerEnterHandler, IPointerExitHandler
{
    Animator _focusAnimator;
    [SerializeField]
    RuntimeAnimatorController _animatorController;
    [SerializeField]
    AudioClip _audioClipEnter, _audioClipExit;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;


    private void Awake()
    {
        if (_focusAnimator == null) _focusAnimator = GetComponent<Animator>();
        if (_animatorController == null)
        {
            Debug.LogError(string.Format("AnimatorController值不能为空"));
        }
        _focusAnimator.runtimeAnimatorController = _animatorController;

    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) return;
        _focusAnimator.SetBool("IsIn", true);
        PlayAudio(_audioClipEnter);
        OnEnter?.Invoke();
    }



    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable) return;
        _focusAnimator.SetBool("IsIn", false);
        PlayAudio(_audioClipExit);
        OnExit?.Invoke();
    }


    protected virtual void OnDisable()
    {
        OnPointerExit(null);
    }



    
}
