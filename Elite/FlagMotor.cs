using UnityEngine;

namespace NOVAKIN.Mod.Elite
{
    public class FlagMotor : MonoBehaviour
    {
        protected Flag flag;

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }

        protected virtual void Awake()
        {
            flag = GetComponent<Flag>();
        }

        protected virtual void FixedUpdate()
        {
            Move();
        }

        protected virtual void Move()
        {
            if (flag != null)
            {
                Vector3 moveVec = flag.CurrentVelocity + (Vector3.up * -20.0f * Time.fixedDeltaTime);

                bool isGrounded = flag.Move(moveVec);

                if (isGrounded == true || flag.carrier != null)
                    flag.Move(Vector3.zero);
            }
        }
    }
}
