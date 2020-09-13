using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMUIComponent : MonoBehaviour
{
    [SerializeField]
    private bool _interactable = true;
    
    protected AudioSource _audioSource;
    /// <summary>
    /// 是否可交互
    /// </summary>
    public bool Interactable { get => _interactable; set => _interactable = value; }


    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="audioClip">指定音频文件</param>
    protected virtual void PlayAudio(AudioClip audioClip)
    {
        if (audioClip == null) return;
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
        if (_audioSource.isPlaying) _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

}
