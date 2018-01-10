using System.Collections;
using UnityEngine;
using NOVAKIN;

namespace NOVAKIN.Mod.Elite
{
    public class AbilityVelocityRedirection : MovementAbility
    {
        private IEnumerator abilityRoutine;

        public override void UseAbility(Vector3 inputVec, RobotMotor robotMotor)
        {
            if (BoltNetwork.isServer || BoltNetwork.IsSinglePlayer)
            {
                if (abilityRoutine != null)
                    StopCoroutine(abilityRoutine);

                abilityRoutine = AbilityRoutine(inputVec, robotMotor);

                StartCoroutine(abilityRoutine);
            }
        }

        protected override IEnumerator AbilityRoutine(Vector3 inputVec, RobotMotor robotMotor)
        {
            float triggerTime = BoltNetwork.serverTime;
            float distanceToGround = 10;

            RaycastHit hit;

            while (BoltNetwork.serverTime <= triggerTime + 0.50f)
            {
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10))
                    distanceToGround = hit.distance - 1.05f;

                distanceToGround = Mathf.Clamp(distanceToGround, 0, 10);

                if (distanceToGround <= 0.75f || robotMotor._IsGrounded || robotMotor._MotorState.timeSinceLastJumpableNormal < 0.256f)
                {
                    abilityVec = ForceVec(inputVec, robotMotor);
                    robotMotor.ConsumeAbility();

                    SendEventHandler.SendPlaySoundEvent("VelocityRedirect01", true, transform.position);
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForFixedUpdate();

            abilityVec = Vector3.zero;
        }

        private Vector3 ForceVec(Vector3 inputVec, RobotMotor robotMotor)
        {
            Vector3 forceVec = Vector3.zero;

            if (inputVec.x > 0)
            {
                forceVec.y = -robotMotor._MotorState.velocity.y + (Mathf.Abs(robotMotor._MotorState.velocity.magnitude) * 0.60f);// (Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(rigidBody.velocity.z)) / 2;
                                                                                                                                 //forceVec.x = -robotMotor._MotorState.velocity.x + (robotMotor._MotorState.velocity.x * 0.9f);
                                                                                                                                 //forceVec.z = -robotMotor._MotorState.velocity.z + (robotMotor._MotorState.velocity.z * 0.9f);

                return forceVec;
            }

            if (inputVec.x < 0)
            {
                forceVec.y = -robotMotor._MotorState.velocity.y + (Mathf.Abs(robotMotor._MotorState.velocity.magnitude) * 0.60f);// (Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(rigidBody.velocity.z)) / 2;
                forceVec.x = -robotMotor._MotorState.velocity.x + (-robotMotor._MotorState.velocity.x * 0.7f);
                forceVec.z = -robotMotor._MotorState.velocity.z + (-robotMotor._MotorState.velocity.z * 0.7f);

                return forceVec;
            }

            forceVec = -robotMotor._MotorState.velocity;

            return forceVec;
        }
    }
}
