using Mechadroids.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Mechadroids {
    public class PlayerEntityHandler : IEntityHandler {
        private readonly PlayerPrefabs playerPrefabs;
        private readonly InputHandler inputHandler;
        private readonly Transform playerStartPosition;
        private readonly CinemachineCamera followCamera;
        private readonly DebugMenuHandler debugMenuHandler;

        private PlayerReference playerReference;
        private HitIndicator hitIndicatorInstance;

        // Public getter so other systems can get the live player reference
        public PlayerReference PlayerReference => playerReference;

        public IEntityState EntityState { get; set; }

        public PlayerEntityHandler(PlayerPrefabs playerPrefabs,
            InputHandler inputHandler,
            Transform playerStartPosition,
            CinemachineCamera followCamera,
            DebugMenuHandler debugMenuHandler) {
            this.playerPrefabs = playerPrefabs;
            this.inputHandler = inputHandler;
            this.playerStartPosition = playerStartPosition;
            this.followCamera = followCamera;
            this.debugMenuHandler = debugMenuHandler;
        }

        public void Initialize() {
            inputHandler.SetCursorState(false, CursorLockMode.Locked);

            playerReference = Object.Instantiate(playerPrefabs.playerReferencePrefab);
            playerReference.transform.position = playerStartPosition.position;
            followCamera.Follow = playerReference.barrel.transform;
            followCamera.LookAt = playerReference.barrel.transform;

            hitIndicatorInstance = Object.Instantiate(playerPrefabs.hitIndicatorPrefab);
            hitIndicatorInstance.gameObject.SetActive(false);

            EntityState = new PlayerActiveState(this, inputHandler, playerReference, hitIndicatorInstance);
            EntityState.Enter();

#if GAME_DEBUG
            InitializeDebugMenu();
#endif
        }

        private void InitializeDebugMenu() {
            debugMenuHandler.AddUIElement(UIElementType.Single, "MoveSpeed", new float[] { playerReference.playerSettings.moveSpeed }, (newValue) => {
                playerReference.playerSettings.moveSpeed = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "RotationSpeed", new float[] { playerReference.playerSettings.rotationSpeed }, (newValue) => {
                playerReference.playerSettings.rotationSpeed = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "Acceleration", new float[] { playerReference.playerSettings.acceleration }, (newValue) => {
                playerReference.playerSettings.acceleration = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "Deceleration", new float[] { playerReference.playerSettings.deceleration }, (newValue) => {
                playerReference.playerSettings.deceleration = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "MaxSlopeAngle", new float[] { playerReference.playerSettings.maxSlopeAngle }, (newValue) => {
                playerReference.playerSettings.maxSlopeAngle = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "TurretRotationSpeed", new float[] { playerReference.playerSettings.turretRotationSpeed }, (newValue) => {
                playerReference.playerSettings.turretRotationSpeed = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "BarrelRotationSpeed", new float[] { playerReference.playerSettings.barrelRotationSpeed }, (newValue) => {
                playerReference.playerSettings.barrelRotationSpeed = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "MaxBarrelElevation", new float[] { playerReference.playerSettings.maxBarrelElevation }, (newValue) => {
                playerReference.playerSettings.maxBarrelElevation = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "MinBarrelElevation", new float[] { playerReference.playerSettings.minBarrelElevation }, (newValue) => {
                playerReference.playerSettings.minBarrelElevation = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "MinTurretAngle", new float[] { playerReference.playerSettings.minTurretAngle }, (newValue) => {
                playerReference.playerSettings.minTurretAngle = newValue[0];
            });

            debugMenuHandler.AddUIElement(UIElementType.Single, "MaxTurretAngle", new float[] { playerReference.playerSettings.maxTurretAngle }, (newValue) => {
                playerReference.playerSettings.maxTurretAngle = newValue[0];
            });
        }

        public void Tick() {
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
            inputHandler.Dispose();
            if(hitIndicatorInstance != null) {
                Object.Destroy(hitIndicatorInstance.gameObject);
                hitIndicatorInstance = null;
            }
        }
    }
}
