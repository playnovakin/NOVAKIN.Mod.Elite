using System.Collections;
using UnityEngine;
using NOVAKIN;

namespace NOVAKIN.Mod.Elite
{
    public class AbilityBoost : MovementAbility
    {
        private IEnumerator abilityRoutine;

        public override void UseAbility(Vector3 inputVec, RobotMotor robotMotor)
        {
            if (abilityRoutine != null)
                StopCoroutine(abilityRoutine);

            abilityRoutine = AbilityRoutine(inputVec, robotMotor);

            StartCoroutine(abilityRoutine);
        }

        protected override IEnumerator AbilityRoutine(Vector3 inputVec, RobotMotor robotMotor)
        {
            abilityVec = transform.forward * 24.0f;
            //abilityVec = robotMotor._MotorState.velocity * 0.66f;

            robotMotor.ConsumeAbility();
            SendEventHandler.SendPlaySoundEvent("Boost", true, transform.position);

            yield return new WaitForFixedUpdate();

            abilityVec = Vector3.zero;
        }
    }
}
