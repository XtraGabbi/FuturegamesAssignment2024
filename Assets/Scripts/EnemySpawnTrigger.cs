using UnityEngine;

namespace Mechadroids {
    public class EnemySpawnTrigger : MonoBehaviour {
        private bool hasSpawned = false;

        private void OnTriggerEnter(Collider other) {
            if(hasSpawned || !other.CompareTag("Player")) return;

            Debug.Log("TriggerSpawn");


            Entrypoint entrypoint = FindFirstObjectByType<Entrypoint>();
            entrypoint?.SpawnEnemies();





            hasSpawned = true;
        }
    }
}
