using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    private AudioSource audioSource;
    private BGMManagerView _view;

    private void Start()
    {
        _view = GetComponent<BGMManagerView>();
        _view.muteButton.onClick.AddListener(() =>
        {
            _view.muteButton.gameObject.SetActive(false);
            _view.unmuteButton.gameObject.SetActive(true);
            MuteAudio(true);
        });
        _view.unmuteButton.onClick.AddListener(() =>
        {
            _view.muteButton.gameObject.SetActive(true);
            _view.unmuteButton.gameObject.SetActive(false);
            MuteAudio(false);
        });

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }

    private void MuteAudio(bool mute)
    {
        if(mute)
            audioSource.volume = 0;
        else
            audioSource.volume = 1f;
    }

    private void SwitchBGM(int index)
    {
        audioSource.clip = audioClips[index];
    }
}
