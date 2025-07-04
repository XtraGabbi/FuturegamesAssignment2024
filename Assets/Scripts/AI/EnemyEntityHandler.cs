using UnityEngine;

namespace Mechadroids {

    /// <summary>
    /// Class that handles the states for each enemy behaviour. Each enemy will have its own entity handler
    /// </summary>
    public class EnemyEntityHandler : IEntityHandler {
        private readonly EnemySettings enemySettings;
        private readonly Transform parentHolder;
        private EnemyReference enemyReference;
        private Transform playerTransform;

        private int currentPatrolIndex;

        public IEntityState EntityState { get; set; }

        public EnemyEntityHandler(EnemySettings enemySettings, Transform parentHolder) {
            this.enemySettings = enemySettings;
            this.parentHolder = parentHolder;
        }

        public void Initialize() {
            // FIX: Removed playerTransform from the Instantiate call — enemy should NOT be a child of the player
            enemyReference = Object.Instantiate(enemySettings.enemy.enemyReferencePrefab, parentHolder);

            enemyReference.transform.position = enemySettings.routeSettings.routePoints.Length > 0
                ? enemySettings.routeSettings.routePoints[0]
                : parentHolder.position;

            // Initialize the default state (Idle State)
            EntityState = new EnemyIdleState(this, enemyReference);
            EntityState.Enter();

            SetNextPatrolDestination();
        }

        public void Tick() {
            if(IsPlayerInDetectionRange()) {
                if(!(EntityState is EnemyChaseState)) {
                    EntityState.Exit();
                    EntityState = new EnemyChaseState(this, enemyReference, playerTransform, enemySettings);
                    EntityState.Enter();
                }

                EntityState.LogicUpdate(); // Chase logic
                return;
            }


            MoveTowardsPatrolPoint();

            EntityState.HandleInput();
            EntityState.LogicUpdate();
        }

        public void PhysicsTick() {
            EntityState.PhysicsUpdate();
        }

        public void LateTick() {
            // Implement if needed
        }

        public void Dispose() {
            if(enemyReference != null) {
                Object.Destroy(enemyReference.gameObject);
                enemyReference = null;
            }
        }


        public void SwitchToPatrolState() {
            EntityState.Exit();
            EntityState = new EnemyIdleState(this, enemyReference); // Or create a PatrolState if you have it
            EntityState.Enter();
        }



        // Patrolling logic

        private void SetNextPatrolDestination() {
            if(enemySettings.routeSettings.routePoints.Length == 0) return;
            currentPatrolIndex %= enemySettings.routeSettings.routePoints.Length;
        }

        private void MoveTowardsPatrolPoint() {
            if(enemySettings.routeSettings.routePoints.Length == 0) return;

            Vector3 targetPoint = enemySettings.routeSettings.routePoints[currentPatrolIndex];
            targetPoint.y = enemyReference.transform.position.y;
            Vector3 direction = (targetPoint - enemyReference.transform.position).normalized;

            // Move towards the target point
            enemyReference.transform.position += direction * enemySettings.enemy.patrolSpeed * Time.deltaTime;

            // Rotate towards the target point
            RotateTowards(direction);

            // Check if the enemy has reached the patrol point
            if(Vector3.Distance(enemyReference.transform.position, targetPoint) <= 0.1f) {
                currentPatrolIndex = (currentPatrolIndex + 1) % enemySettings.routeSettings.routePoints.Length;
                SetNextPatrolDestination();
            }
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

        private bool IsPlayerInDetectionRange() {


            if(playerTransform == null) return false;
            float distance = Vector3.Distance(enemyReference.transform.position, playerTransform.position);
            return distance <= enemySettings.enemy.detectionRadius;
        }

        public void SetPlayerTransform(Transform player) {
            playerTransform = player;
        }
    }
}
