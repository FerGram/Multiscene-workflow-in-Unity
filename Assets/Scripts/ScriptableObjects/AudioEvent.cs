using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "ScriptableObjects/Events/AudioEvent", order = 51)]
public class AudioEvent : ScriptableObject
{
    public UnityAction<AudioClip> onAudioClipRequest;

    public void Raise(AudioClip audio)
    {
        if (onAudioClipRequest == null) { Debug.LogWarning("onAudioClipRequest is null"); return; }
        onAudioClipRequest.Invoke(audio);
    }
}
