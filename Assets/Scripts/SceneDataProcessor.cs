using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataProcessor : MonoBehaviour
{
    [SerializeField] AudioEvent _musicEventChannel;

    public void ProcessSceneData(SceneDataSO scene){
        
        //Can add as many data handlers as you want here        
        if (scene is SceneDataInGameSO) {

            SceneDataInGameSO sceneData = scene as SceneDataInGameSO;

            //TODO: for the moment audio fading will only take place when
            //we load a scene that wasn't already loaded AND contains audio file
            HandleAudioTransition(sceneData.SceneMusic);
        }
        else Debug.LogWarning("Scene is not the type specified");
    }

    private void HandleAudioTransition(AudioClip sceneMusic) {
        if (_musicEventChannel == null || sceneMusic == null) return;
        _musicEventChannel.Raise(sceneMusic);
    }
}
