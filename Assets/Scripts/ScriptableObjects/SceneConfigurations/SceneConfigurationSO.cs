using UnityEngine;

/// <summary> This scriptable object is a collection of scenes to
/// persistently load whenever a scene change occurs </summary>
[CreateAssetMenu(fileName = "SceneConfigurationSO", menuName = "ScriptableObjects/Scene/SceneConfigurationSO", order = 51)]
public class SceneConfigurationSO : ScriptableObject {

    public SceneDataSO[] Configuration;
}
