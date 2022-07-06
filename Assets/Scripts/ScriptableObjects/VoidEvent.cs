using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEvent", menuName = "ScriptableObjects/Events/VoidEvent", order = 51)]
public class VoidEvent : ScriptableObject
{
    public UnityAction onVoidRequest;

    public void Raise()
    {
        if (onVoidRequest == null) { Debug.LogWarning("onVoidRequest is null"); return; }
        onVoidRequest.Invoke();
    }
}
