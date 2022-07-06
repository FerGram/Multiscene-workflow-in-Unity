using UnityEngine;

[CreateAssetMenu(fileName = "SceneDataInGameSO", menuName = "ScriptableObjects/Scene/SceneDataInGameSO", order = 51)]
public class SceneDataInGameSO : SceneDataSO
{
    [Space]
    public AudioClip SceneMusic;
    public SceneDataInGameSO[] AdjacentScenes;
}
