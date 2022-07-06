using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] TransformEvent _spawnTransformEvent;
    // [SerializeField] PlayerSpawnPositionSO _spawnPosition;

    private void OnEnable() {
        _spawnTransformEvent.onTransformRequest += SpawnPlayerAtPosition;
    }

    private void OnDisable() {
        _spawnTransformEvent.onTransformRequest -= SpawnPlayerAtPosition;
    }

    private void SpawnPlayerAtPosition(Transform player){
        player.position = transform.position;
        //Could also reset rigidbody's velocity
    }
}
