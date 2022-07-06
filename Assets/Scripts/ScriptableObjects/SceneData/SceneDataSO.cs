using UnityEngine;

//Note that you don't put the "CreateAssetMenu" attribute, because this is the base class 
//for all SceneData objects
public class SceneDataSO : ScriptableObject {
    
    public string SceneName;
    public string SceneDescription;
    
}
