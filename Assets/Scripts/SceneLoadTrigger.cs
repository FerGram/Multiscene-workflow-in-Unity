using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Next scene configuration")]
    [SerializeField] SceneDataSO _thisScene;
    [SerializeField] SceneConfigurationSO _scenesConfiguration;

    [Space]
    [SerializeField] bool _respawnPlayer = false;
    [SerializeField] VoidEvent _respawnRequestEvent;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (_respawnPlayer) _respawnRequestEvent.onVoidRequest.Invoke();
            if (_thisScene != null && _scenesConfiguration != null) SceneLoader.Instance.LoadScenes(_thisScene, _scenesConfiguration);
            else if (_thisScene != null) SceneLoader.Instance.LoadScenes(_thisScene);
        }
    }

}
