using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TransformEvent", menuName = "ScriptableObjects/Events/TransformEvent", order = 51)]
public class TransformEvent : ScriptableObject
{
    public UnityAction<Transform> onTransformRequest;

    public void Raise(Transform transform)
    {
        if (onTransformRequest == null) { Debug.LogWarning("No one listening to TransformRequest"); return; }
        onTransformRequest.Invoke(transform);
    }
}
