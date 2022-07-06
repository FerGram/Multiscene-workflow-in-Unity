using UnityEngine;

public class InitializationLoader : MonoBehaviour
{
    [Header("Initial configuration")]
    [SerializeField] SceneConfigurationSO _sceneConfiguration;

    [Header("Extra scenes")] 
    [SerializeField] SceneDataSO[] _extraScenesToLoad;

    private void OnEnable() {

        //If only one scene in the array, load that scene with the configuration
        //otherwise, load the first scene with configuration and the remaining
        //load them individually
        if (_extraScenesToLoad.Length == 1) SceneLoader.Instance.LoadScenes(_extraScenesToLoad[0], _sceneConfiguration);
        else {
            for(int i = 0; i < _extraScenesToLoad.Length; i++){
                if (i == 0) SceneLoader.Instance.LoadScenes(_extraScenesToLoad[i], _sceneConfiguration);
                else SceneLoader.Instance.LoadScenes(_extraScenesToLoad[i]);
            }
            
        }
    }
}
