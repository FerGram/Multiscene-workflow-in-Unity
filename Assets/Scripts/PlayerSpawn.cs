using System.Collections;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] TransformEvent _spawnTransformEvent;
    [Space]
    [SerializeField] VoidEvent _respawnRequestEvent;
    [SerializeField] VoidEvent _mainSceneLoadedEvent;
    // [SerializeField] PlayerSpawnPositionSO _spawnPosition;

    private Ball _player;
    private float _currentTime = 0;
    private bool _isMainSceneLoaded = false;

    private const float MAX_SPAWN_TIMEOUT = 3; //In seconds

    private void OnEnable() {
        _respawnRequestEvent.onVoidRequest += SpawnPlayer;
        _mainSceneLoadedEvent.onVoidRequest += () => _isMainSceneLoaded = true;

        //This is only for the first call and we don't want the extra yield return null on
        //the coroutine
        if (SearchForPlayer()){
            _spawnTransformEvent.onTransformRequest.Invoke(_player.transform);
            _isMainSceneLoaded = false;
        }
    }

    private void OnDisable() {
        _respawnRequestEvent.onVoidRequest -= SpawnPlayer;
        _mainSceneLoadedEvent.onVoidRequest -= () => _isMainSceneLoaded = true;
    }

    private void SpawnPlayer(){
        
        if (SearchForPlayer()){
            StartCoroutine(SpawnRoutine());
        }
    }

    private bool SearchForPlayer(){
        if (_player == null) _player = FindObjectOfType<Ball>();
        if (_player == null) {
            Debug.LogWarning("Could not spawn Player because no player was found");
            return false;
        }
        else return true;
    }

    IEnumerator SpawnRoutine(){
        while(!_isMainSceneLoaded && _currentTime < MAX_SPAWN_TIMEOUT){
            _currentTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
        if (_isMainSceneLoaded) {
            _spawnTransformEvent.onTransformRequest.Invoke(_player.transform);
            _isMainSceneLoaded = false;
        }

        _currentTime = 0;
    }
}
