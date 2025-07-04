using UnityEngine;

namespace Mechadroids {
    public class EnemyChaseState : IEntityState {
        private readonly EnemyEntityHandler entityHandler;
        private readonly EnemyReference enemyReference;
        private readonly Transform playerTransform;
        private readonly EnemySettings enemySettings;

        public EnemyChaseState(EnemyEntityHandler entityHandler, EnemyReference enemyReference, Transform playerTransform, EnemySettings enemySettings) {
            this.entityHandler = entityHandler;
            this.enemyReference = enemyReference;
            this.playerTransform = playerTransform;
            this.enemySettings = enemySettings;
        }

        public void Enter() {
            // Optionally: play chase animation
            Debug.Log("Chasing player");
        }

        public void LogicUpdate() {
            if(playerTransform == null) return;

            // Check if player is still in range, otherwise return to patrol
            float distance = Vector3.Distance(enemyReference.transform.position, playerTransform.position);
            if(distance > enemySettings.enemy.detectionRadius) {
                entityHandler.SwitchToPatrolState();
                return;
            }

            // Move toward player
            Vector3 direction = (playerTransform.position - enemyReference.transform.position).normalized;
            direction.y = 0f; // Keep on flat ground if needed

            enemyReference.transform.position += direction * enemySettings.enemy.patrolSpeed * Time.deltaTime;
            RotateTowards(direction);
        }

        public void PhysicsUpdate() {
            // Usually not needed unless you use Rigidbody
        }

        public void Exit() {
            // Cleanup or stop animation
        }

        private void RotateTowards(Vector3 direction) {
            if(direction.magnitude == 0) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyReference.transform.rotation = Quaternion.Slerp(
                enemyReference.transform.rotation,
                targetRotation,
                enemySettings.enemy.patrolRotationSpeed * Time.deltaTime
            );
        }
    }
}
