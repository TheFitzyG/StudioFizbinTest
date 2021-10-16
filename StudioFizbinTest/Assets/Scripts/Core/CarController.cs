using TFG_SP;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour
    {

        [SerializeField] private float _topSpeed;
        [SerializeField] private float _steeringStrength;
        [SerializeField] private MinMaxFloat _angularDrag;
        [SerializeField] private float _driftSpeedMin;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _breakingPower;

        [SerializeField] private Transform _frontAxel;
        [SerializeField] private Transform _rearAxel;


        private Rigidbody2D rigidbody2D;
        private CarControls controls;//= new CarControls();

        private bool Accelerate => controls.Car.Acceleration.ReadValue<float>() > 0.1f;
        private bool Brake => controls.Car.Acceleration.ReadValue<float>() < -0.1f;

        private float Steering => controls.Car.Steering.ReadValue<float>();


        private bool hasJustBreaked = false;

        // Start is called before the first frame update
        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();

            controls = new CarControls();
            controls.Enable();

        }

        // Update is called once per frame
        void Update()
        {

            if (rigidbody2D.velocity.magnitude > _topSpeed)
            {
                rigidbody2D.velocity = rigidbody2D.velocity.normalized * _topSpeed;
            }


            if (Brake)
            {


                //if (rigidbody2D.velocity.magnitude > _driftSpeedMin)
                //{
                //    if (Steering > 0.1f || Steering < -0.1f)
                //    {
                //        newForceThisFrame += (Vector2)((transform.right * -Steering) * _steeringStrength);// + (Vector2) (transform.right * _rearWheelFriction);
                //    }
                //}

                rigidbody2D.centerOfMass = _frontAxel.localPosition;

                if (rigidbody2D.velocity.magnitude > _driftSpeedMin)
                {
                    rigidbody2D.angularDrag = _angularDrag.Min; //= Mathf.Lerp(rigidbody2D.angularDrag, _angularDrag.Min, 30f * Time.deltaTime);
                }
                else
                {
                    rigidbody2D.angularDrag = Mathf.Lerp(rigidbody2D.angularDrag, _angularDrag.Max, 5f * Time.deltaTime);
                }

                rigidbody2D.AddForceAtPosition(-((rigidbody2D.velocity.normalized).normalized * _breakingPower) * Time.deltaTime, _frontAxel.position, ForceMode2D.Force);

                //if (!hasJustBreaked)
                //{
                //    hasJustBreaked = true;

                //    rigidbody2D.AddForceAtPosition(
                //        (-(rigidbody2D.velocity * (Vector2)transform.right) * _breakingPower * 2), 
                //        _rearAxel.position, ForceMode2D.Impulse);
                //}

                //rigidbody2D.AddForceAtPosition(((rigidbody2D.velocity.normalized * (Vector2) transform.right).normalized * _breakingPower )/ 2, _rearAxel.position, ForceMode2D.Force);
                return;
            }

            rigidbody2D.angularDrag = Mathf.Lerp(rigidbody2D.angularDrag, _angularDrag.Max, 2f * Time.deltaTime);

            rigidbody2D.centerOfMass = Vector2.Lerp(rigidbody2D.centerOfMass, Vector2.zero, 2f * Time.deltaTime);

            Vector2 newForceThisFrame = Vector2.zero;
            if (Accelerate)
            {

                hasJustBreaked = false;

                if (rigidbody2D.velocity.magnitude < _topSpeed)
                {
                    newForceThisFrame += (Vector2) (transform.up * _acceleration);
                }

                if (Steering != 0 && rigidbody2D.velocity.magnitude > 2f)
                {
                    newForceThisFrame += (Vector2) (transform.right * Steering) * _steeringStrength; // * (rigidbody2D.velocity.magnitude/ _topSpeed);
                }

                
                newForceThisFrame *= Time.deltaTime;

                rigidbody2D.AddForceAtPosition(newForceThisFrame, _frontAxel.position, ForceMode2D.Force);
                return;
            }

        }
    }
}
