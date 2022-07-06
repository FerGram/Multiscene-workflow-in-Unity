using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance {get{
        if (_instance == null) {
            _instance = FindObjectOfType<SceneLoader>();
            if (_instance == null) {
                GameObject container = new GameObject("SceneLoader");
                _instance = container.AddComponent<SceneLoader>();
            }
        }
        return _instance;
    }}
    private VoidEvent _mainSceneLoadedEvent;
    private const string MAIN_SCENE_LOADED_PATH = "MainSceneLoadedEvent";

    private static SceneLoader _instance;

    private List<string> _loadingScenes = new List<string>();
    private List<AsyncOperation> _loadingScenesOperations = new List<AsyncOperation>();
    private bool _loadingalreadyRequested = false;

    private SceneDataProcessor _processor;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else if (_instance != this) {
            Destroy(gameObject);
        }

        _mainSceneLoadedEvent = Resources.Load<VoidEvent>(MAIN_SCENE_LOADED_PATH);
    }

    public void UnloadScene(string scene){
        if (scene != null)
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(scene)).completed += (AsyncOperation op) => {
                    Debug.Log("Unloaded scene");
                };
    }

    //Both parameters are default to null because you can load only a scene without configuration
    //and only configuration without scene
    public void LoadScenes(SceneDataSO nextScene = null, SceneConfigurationSO sceneConfiguration = null) {

        //TODO: Show loading screen

        //Prevent double scene loading
        if (_loadingalreadyRequested) return;
        _loadingalreadyRequested = true;

        //Not all scenes given may need to be loaded, that's why we filter
        List<SceneDataSO> scenesToLoad = new List<SceneDataSO>{nextScene};
        AddConfiguration(ref scenesToLoad, sceneConfiguration);
        AddAdjacentScenes(ref scenesToLoad, nextScene);

        UnloadNonPersistentScenes(ref scenesToLoad);
        StartCoroutine(LoadRemainingScenes(scenesToLoad));
        
        StartCoroutine(WaitForScenesToLoad(scenesToLoad, nextScene));
    }

    private void AddAdjacentScenes(ref List<SceneDataSO> scenesToLoad, SceneDataSO scene){
        if (scene is SceneDataInGameSO){
            SceneDataInGameSO sceneInGame = scene as SceneDataInGameSO;
            if (sceneInGame.AdjacentScenes != null){
                foreach(var adjacent in sceneInGame.AdjacentScenes){
                    if (!scenesToLoad.Contains(adjacent) && adjacent != null) scenesToLoad.Add(adjacent as SceneDataSO);
                }
            }
        }
        else scenesToLoad.Add(scene);
    }
    private void AddConfiguration(ref List<SceneDataSO> scenesToLoad, SceneConfigurationSO sceneConfiguration){
        
        if (sceneConfiguration == null) return;
        foreach(var configScene in sceneConfiguration.Configuration){
            if (!scenesToLoad.Contains(configScene) && configScene != null) scenesToLoad.Add(configScene);
        }
    }

    ///<summary> Unload all scenes that are not in the scenes list </summary>
    private void UnloadNonPersistentScenes(ref List<SceneDataSO> scenesToLoad) {

        //Go through all loaded scenes and for each of them, check if they are in the 
        //scenesToLoad list (those who are, don't need to be unloaded)
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            bool isPersistent = false;
            foreach(var loadedScene in scenesToLoad){
                if (SceneManager.GetSceneAt(i).name == loadedScene.SceneName) {
                    isPersistent = true;
                }
            }
            if (!isPersistent) {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)).completed += (AsyncOperation op) => {
                    Debug.Log("Unloaded scene");
                };
            }
        }
    }
    ///<summary> Load all the scenes that are not already loaded </summary>
    IEnumerator LoadRemainingScenes(List<SceneDataSO> scenesToLoad){

        //Ensure the first scene is always loaded first
        if (!SceneManager.GetSceneByName(scenesToLoad[0].SceneName).isLoaded &&
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName(scenesToLoad[0].SceneName) &&
                !_loadingScenes.Contains(scenesToLoad[0].SceneName)){

                bool isFirstSceneLoaded = false;
                LoadScene(scenesToLoad[0]).completed += (_) => {
                    isFirstSceneLoaded = true;
                    _mainSceneLoadedEvent.onVoidRequest.Invoke();
                };

                while (!isFirstSceneLoaded) yield return null;
        }

        foreach(var scene in scenesToLoad){
            if (!SceneManager.GetSceneByName(scene.SceneName).isLoaded &&
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName(scene.SceneName) &&
                !_loadingScenes.Contains(scene.SceneName)) {

                LoadScene(scene);
            }
        }
    }


    private void SendSceneDataForProcessing(SceneDataSO scene){
        if (_processor == null) _processor = FindObjectOfType<SceneDataProcessor>();
        if (_processor == null) return;
        _processor.ProcessSceneData(scene);
    }
    private void SendSceneDataForProcessing(List<SceneDataSO> scenes){

        foreach(var scene in scenes){
            SendSceneDataForProcessing(scene);
        }
    }

    private AsyncOperation LoadScene(SceneDataSO scene){
        AsyncOperation op = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
        _loadingScenes.Add(scene.SceneName);
        _loadingScenesOperations.Add(op);
        return op;
    }

    IEnumerator WaitForScenesToLoad(List<SceneDataSO> scenes, SceneDataSO newActiveScene) {
        int i = 0;
        while (_loadingScenesOperations.Count > 0) {
            if (_loadingScenesOperations[i].isDone){
                _loadingScenesOperations.RemoveAt(i);
                _loadingScenes.RemoveAt(i);
            }
            if (_loadingScenesOperations.Count != 0) i = (i+1) % _loadingScenesOperations.Count;
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newActiveScene.SceneName));
        Debug.Log(SceneManager.GetActiveScene().name);

        //Wait for all scenes to load and then send scene data for processing
        // SendSceneDataForProcessing(scenes);
        SendSceneDataForProcessing(newActiveScene);
        _loadingalreadyRequested = false;
    }
}
