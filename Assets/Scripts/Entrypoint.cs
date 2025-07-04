using Mechadroids.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Mechadroids {
    /// <summary>
    /// The main class that performs different system initialization. This class can be refactored to handle larger and more expansive systems, since
    /// in this state it breaks the single responsibility principle. System initialization and systems state should be handled separately.
    /// </summary>
    public class Entrypoint : MonoBehaviour {
        public Transform playerStartPosition;
        public CinemachineCamera followCamera;
        public Transform aiParentTransform;

        private PlayerPrefabs playerPrefabs;
        private InputHandler inputHandler;
        private PlayerEntityHandler playerEntityHandler;
        private AISettings aISettings;
        private AIEntitiesHandler aiEntitiesHandler;
        private bool initialized;
        private DebugMenuHandler debugMenuHandler;
        private UIPrefabs uiPrefabs;




        public void Initialize() {
            // Load resources if they are present in a /Resource folder anywhere in the project
            playerPrefabs = Resources.Load<PlayerPrefabs>("PlayerPrefabs");
            aISettings = Resources.Load<AISettings>("AISettings");
            uiPrefabs = Resources.Load<UIPrefabs>("UIPrefabs");

            // Initialize systems
            inputHandler = new InputHandler();
            inputHandler.Initialize();




            // this define symbol, if removed from Project Settings, makes sure that in a release build this code will be stripped
#if GAME_DEBUG
            debugMenuHandler = new DebugMenuHandler(uiPrefabs, inputHandler);
            debugMenuHandler.Initialize();
#endif
            playerEntityHandler = new PlayerEntityHandler(playerPrefabs, inputHandler, playerStartPosition, followCamera, debugMenuHandler);
            playerEntityHandler.Initialize();

           //Moved down to method
            //aiEntitiesHandler = new AIEntitiesHandler(aISettings, aiParentTransform, playerEntityHandler.PlayerReference.transform);
            //aiEntitiesHandler.Initialize();

            // it is very important to control the initialization state to avoid running tick functions with data that is not yet initialized
            initialized = true;
        }


        public void SpawnEnemies() {
            aiEntitiesHandler = new AIEntitiesHandler(aISettings, aiParentTransform, playerEntityHandler.PlayerReference.transform);
            aiEntitiesHandler.Initialize();
        }




        public void Update() {
            if(!initialized) {
                return;
            }
            playerEntityHandler.Tick();
            if(aiEntitiesHandler != null) {
                aiEntitiesHandler.Tick();
            }
            debugMenuHandler?.Tick();
        }

        public void FixedUpdate() {
            if(!initialized) {
                return;
            }
            playerEntityHandler.PhysicsTick();
            if(aiEntitiesHandler != null) {
                aiEntitiesHandler.PhysicsTick();
            }
        }

        public void OnDestroy() {
            if(!initialized) {
                return;
            }
            inputHandler.Dispose();
            playerEntityHandler.Dispose();
            if(aiEntitiesHandler != null) {
                aiEntitiesHandler.Dispose();
            }
            debugMenuHandler?.Dispose();
        }
    }
}
