using UnityEngine;

namespace Mechadroids {
    public class PlayerActiveState : IEntityState {
        private readonly InputHandler inputHandler;
        private readonly PlayerReference playerReference;
        private readonly HitIndicator hitIndicatorInstance;

        private float currentSpeed;
        private float turretAngle = 0f;
        private float barrelAngle = 0f;

        public PlayerActiveState(
            IEntityHandler entityHandler,
            InputHandler inputHandler,
            PlayerReference playerReference,
            HitIndicator hitIndicatorInstance) {
            this.inputHandler = inputHandler;
            this.playerReference = playerReference;
            this.hitIndicatorInstance = hitIndicatorInstance;
        }




        // Added in after
        private void HandleMovement() {
            if(inputHandler.MovementInput.y != 0) {
                currentSpeed += inputHandler.MovementInput.y * playerReference.playerSettings.acceleration * Time.deltaTime;
            }
            else {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, playerReference.playerSettings.deceleration * Time.deltaTime);
            }

            currentSpeed = EntityHelper.HandleSlope(playerReference.tankBody, playerReference.playerSettings.maxSlopeAngle, currentSpeed);

            currentSpeed = Mathf.Clamp(currentSpeed, -playerReference.playerSettings.moveSpeed, playerReference.playerSettings.moveSpeed);
            playerReference.tankBody.Translate(Vector3.forward * (currentSpeed * Time.deltaTime));

            float rotationAmount = inputHandler.MovementInput.x * playerReference.playerSettings.rotationSpeed * Time.deltaTime;
            playerReference.tankBody.Rotate(Vector3.up, rotationAmount);
        }

        private void HandleTurretAiming() {
            Vector2 mouseInput = inputHandler.MouseDelta;

            // Update turret horizontal angle
            turretAngle += mouseInput.x * playerReference.playerSettings.turretRotationSpeed * Time.deltaTime;
            turretAngle = Mathf.Clamp(turretAngle, playerReference.playerSettings.minTurretAngle, playerReference.playerSettings.maxTurretAngle);

            // Update barrel elevation angle
            barrelAngle -= mouseInput.y * playerReference.playerSettings.barrelRotationSpeed * Time.deltaTime; // Inverted because moving mouse up should raise the barrel
            barrelAngle = Mathf.Clamp(barrelAngle, playerReference.playerSettings.minBarrelElevation, playerReference.playerSettings.maxBarrelElevation);

            // Apply turret rotation relative to tank body
            Quaternion turretRotation = playerReference.tankBody.rotation * Quaternion.Euler(0f, turretAngle, 0f);
            playerReference.turretBase.rotation = turretRotation;

            // Apply barrel rotation
            Quaternion barrelRotation = Quaternion.Euler(barrelAngle, 0f, 0f);
            playerReference.barrel.localRotation = barrelRotation;
        }

        private void UpdateHitIndicator() {
            var ray = new Ray(playerReference.barrelEnd.position, playerReference.barrelEnd.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, playerReference.aimLayerMask)) {
                hitIndicatorInstance.gameObject.SetActive(true);
                hitIndicatorInstance.transform.position = hitInfo.point + hitInfo.normal * 0.01f;
                hitIndicatorInstance.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
            }
            else {
                if(hitIndicatorInstance != null) {
                    hitIndicatorInstance.gameObject.SetActive(false);
                }
            }
        }




        public void Enter() {
            // Any initialization when entering the state
        }

        public void LogicUpdate() {
            //handle player active state functionality
            HandleMovement();
            HandleTurretAiming();
            UpdateHitIndicator();


        }

        public void PhysicsUpdate() {
            // Implement physics update if needed
        }

        public void Exit() {
            // Clean up when exiting the state
        }
    }
}
