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
            debugMenuHandler.AddUIElement(UIElementType.Single, "MoveSpeed", new float [] { playerReference.playerSettings.moveSpeed }, (newValue) => {
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
            if (hitIndicatorInstance != null) {
                Object.Destroy(hitIndicatorInstance.gameObject);
                hitIndicatorInstance = null;
            }
        }
    }

    // code that handles the player functionality. Should to the correct state

        // private void HandleMovement() {
        //     if(inputHandler.MovementInput.y != 0) {
        //         currentSpeed += inputHandler.MovementInput.y * playerReference.playerSettings.acceleration * Time.deltaTime;
        //     }
        //     else {
        //         currentSpeed = Mathf.MoveTowards(currentSpeed, 0, playerReference.playerSettings.deceleration * Time.deltaTime);
        //     }
        //
        //     currentSpeed = EntityHelper.HandleSlope(playerReference.tankBody, playerReference.playerSettings.maxSlopeAngle, currentSpeed);
        //
        //     currentSpeed = Mathf.Clamp(currentSpeed, -playerReference.playerSettings.moveSpeed, playerReference.playerSettings.moveSpeed);
        //     playerReference.tankBody.Translate(Vector3.forward * (currentSpeed * Time.deltaTime));
        //
        //     float rotationAmount = inputHandler.MovementInput.x * playerReference.playerSettings.rotationSpeed * Time.deltaTime;
        //     playerReference.tankBody.Rotate(Vector3.up, rotationAmount);
        // }
        //
        // private void HandleTurretAiming() {
        //     Vector2 mouseInput = inputHandler.MouseDelta;
        //
        //     // Update turret horizontal angle
        //     turretAngle += mouseInput.x * playerReference.playerSettings.turretRotationSpeed * Time.deltaTime;
        //     turretAngle = Mathf.Clamp(turretAngle, playerReference.playerSettings.minTurretAngle, playerReference.playerSettings.maxTurretAngle);
        //
        //     // Update barrel elevation angle
        //     barrelAngle -= mouseInput.y * playerReference.playerSettings.barrelRotationSpeed * Time.deltaTime; // Inverted because moving mouse up should raise the barrel
        //     barrelAngle = Mathf.Clamp(barrelAngle, playerReference.playerSettings.minBarrelElevation, playerReference.playerSettings.maxBarrelElevation);
        //
        //     // Apply turret rotation relative to tank body
        //     Quaternion turretRotation = playerReference.tankBody.rotation * Quaternion.Euler(0f, turretAngle, 0f);
        //     playerReference.turretBase.rotation = turretRotation;
        //
        //     // Apply barrel rotation
        //     Quaternion barrelRotation = Quaternion.Euler(barrelAngle, 0f, 0f);
        //     playerReference.barrel.localRotation = barrelRotation;
        // }
        //
        // private void UpdateHitIndicator() {
        //     var ray = new Ray(playerReference.barrelEnd.position, playerReference.barrelEnd.forward);
        //     if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, playerReference.aimLayerMask)) {
        //         hitIndicatorInstance.gameObject.SetActive(true);
        //         hitIndicatorInstance.transform.position = hitInfo.point + hitInfo.normal * 0.01f;
        //         hitIndicatorInstance.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        //     }
        //     else {
        //         if(hitIndicatorInstance != null) {
        //             hitIndicatorInstance.gameObject.SetActive(false);
        //         }
        //     }
        // }
}
