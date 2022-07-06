using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _sfxAudioSource;
    [Space]
    [SerializeField] AudioSource _music1AudioSource;
    [SerializeField] AudioSource _music2AudioSource;
    [SerializeField] AudioMixer _musicMixer;
    [Header("Configuration")]
    [SerializeField] AudioEvent _sfxEventChannel;
    [SerializeField] AudioEvent _musicEventChannel;
    [Space]
    [SerializeField] bool _disableSfx;
    [SerializeField] bool _disableMusic;

    private bool _track1IsPlaying = false;
    private bool _isFading = false;

    private AudioMixerGroup[] group;

    private const string MUSIC_MASTER_VOLUME = "VolumeMaster";
    private const string MUSIC_VOLUME_1 = "Volume1";
    private const string MUSIC_VOLUME_2 = "Volume2";

    private const string SFX_MASTER_VOLUME = "VolumeMaster";


    private void Awake() {
        group = _musicMixer.FindMatchingGroups("Track");

        if (_sfxEventChannel != null) _sfxEventChannel.onAudioClipRequest += OnSfxRequest;
        if (_musicEventChannel != null) _musicEventChannel.onAudioClipRequest += OnMusicRequest;
    }

    private void Start() {
        if (_disableMusic) {
            _musicMixer.SetFloat(MUSIC_MASTER_VOLUME, -80);
        }
        if (_disableSfx) {
            _sfxAudioSource.volume = -80;
        }
    }

    private void OnSfxRequest(AudioClip audio) {
        if (_disableSfx) return;
        _sfxAudioSource.PlayOneShot(audio);
    }

    private void OnMusicRequest(AudioClip audio) {
        if (_disableMusic) return;
        if ((_track1IsPlaying && _music1AudioSource.clip == audio) || (!_track1IsPlaying && _music2AudioSource.clip == audio)) return;
        SetNewClip(audio);
        FadeMusic();
    }

    private void SetNewClip(AudioClip clip){
        
        if(_track1IsPlaying){
            _music2AudioSource.clip = clip;
            _music2AudioSource.Play();
        }
        else {
            _music1AudioSource.clip = clip;
            _music1AudioSource.Play();
        }
    }

    private void FadeMusic(){

        if (!_isFading && group != null){

            _isFading = true;

            if (_track1IsPlaying){ 

                StartCoroutine(StartFade(group[0], MUSIC_VOLUME_1, 1.5f, 0f));
                StartCoroutine(StartFade(group[1], MUSIC_VOLUME_2, 1.5f, 1f));
                _track1IsPlaying = false;
            }
            else{
                StartCoroutine(StartFade(group[0], MUSIC_VOLUME_1, 1.5f, 1f));
                StartCoroutine(StartFade(group[1], MUSIC_VOLUME_2, 1.5f, 0f));
                _track1IsPlaying = true;
            }
        }
    }

    private IEnumerator StartFade(AudioMixerGroup audioMixerGroup, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixerGroup.audioMixer.GetFloat(exposedParam, out currentVol);

        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixerGroup.audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        _isFading = false;
        yield break;
    }

    private void OnDisable() {
        if (_sfxEventChannel != null) _sfxEventChannel.onAudioClipRequest -= OnSfxRequest;
        if (_musicEventChannel != null) _musicEventChannel.onAudioClipRequest -= OnMusicRequest;
    }
}
