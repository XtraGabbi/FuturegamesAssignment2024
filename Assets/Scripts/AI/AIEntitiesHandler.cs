using System.Collections.Generic;
using UnityEngine;

namespace Mechadroids {
    // Keeps the state for all AI activity
    public class AIEntitiesHandler {
        private readonly AISettings aiSettings;
        private readonly Transform parentHolder;
        private readonly Transform playerTransform;

        private Dictionary<int, EnemyEntityHandler> EnemyEntityHandlers { get; } = new();

        public AIEntitiesHandler(AISettings aiSettings, Transform parentHolder, Transform playerTransform) {
            this.aiSettings = aiSettings;
            this.parentHolder = parentHolder;
            this.playerTransform = playerTransform;
        }

        public void Initialize() {
            int globalIndex = 0;
            foreach(EnemyGroup enemy in aiSettings.enemiesToSpawn) {
                for(int i = 0; i < enemy.enemyCount; i++) {
                    EnemyEntityHandler enemyEntityHandler = new(enemy.enemySettings, parentHolder);
                    enemyEntityHandler.SetPlayerTransform(playerTransform); // Pass player reference
                    enemyEntityHandler.Initialize();
                    EnemyEntityHandlers.TryAdd(globalIndex, enemyEntityHandler);
                    globalIndex++;
                }
            }
        }

        public void Tick() {
            foreach(KeyValuePair<int, EnemyEntityHandler> enemyEntityHandler in EnemyEntityHandlers) {
                enemyEntityHandler.Value.Tick();
            }
        }

        public void PhysicsTick() {
            foreach(KeyValuePair<int, EnemyEntityHandler> enemyEntityHandler in EnemyEntityHandlers) {
                enemyEntityHandler.Value.PhysicsTick();
            }
        }

        public void Dispose() {
            foreach(KeyValuePair<int, EnemyEntityHandler> enemyEntityHandler in EnemyEntityHandlers) {
                enemyEntityHandler.Value.Dispose();
            }
            EnemyEntityHandlers.Clear();
        }
    }
}
