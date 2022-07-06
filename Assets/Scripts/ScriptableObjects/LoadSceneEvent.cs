using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LoadSceneEvent", menuName = "ScriptableObjects/Events/LoadSceneEvent", order = 51)]
public class LoadSceneEvent : ScriptableObject
{
    public UnityAction<SceneDataSO> onSceneLoadRequest;

    public void Raise(SceneDataSO scene)
    {
        if (onSceneLoadRequest == null) { Debug.LogWarning("onTransformRequest is null"); return; }
        onSceneLoadRequest.Invoke(scene);
    }
}
