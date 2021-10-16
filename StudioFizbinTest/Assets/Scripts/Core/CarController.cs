using TFG_SP;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarController : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float _topSpeed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _breakingPower;

        [Header("Steering")]
        [SerializeField] private float _steeringStrength;
        [SerializeField] private MinMaxFloat _angularDrag;
        [SerializeField] private float _driftSpeedMin;
        
        [Header("SubComponents")]
        [SerializeField] private Transform _frontAxel;


        private Rigidbody2D _rigidbody2D;
        private CarControls _controls;

        private bool Accelerate => _controls.Car.Acceleration.ReadValue<float>() > 0.1f;
        private bool Brake => _controls.Car.Acceleration.ReadValue<float>() < -0.1f;

        private float Steering => _controls.Car.Steering.ReadValue<float>();

        void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _controls = new CarControls();
            _controls.Enable();
        }

        void Update()
        {
            if (_rigidbody2D.velocity.magnitude > _topSpeed)
            {
                _rigidbody2D.velocity = _rigidbody2D.velocity.normalized * _topSpeed;
            }


            if (Brake)
            {
                _rigidbody2D.centerOfMass = _frontAxel.localPosition;

                if (_rigidbody2D.velocity.magnitude > _driftSpeedMin)
                {
                    _rigidbody2D.angularDrag = _angularDrag.Min; 
                }
                else
                {
                    _rigidbody2D.angularDrag = Mathf.Lerp(_rigidbody2D.angularDrag, _angularDrag.Max, 2f * Time.deltaTime);
                    _rigidbody2D.centerOfMass = Vector2.Lerp(_rigidbody2D.centerOfMass, Vector2.zero, 2f * Time.deltaTime);
                }

                _rigidbody2D.AddForceAtPosition(-((_rigidbody2D.velocity.normalized).normalized * _breakingPower) * Time.deltaTime, _frontAxel.position, ForceMode2D.Force);

                return;
            }


            _rigidbody2D.angularDrag = Mathf.Lerp(_rigidbody2D.angularDrag, _angularDrag.Max, 2f * Time.deltaTime);
            _rigidbody2D.centerOfMass = Vector2.Lerp(_rigidbody2D.centerOfMass, Vector2.zero, 2f * Time.deltaTime);


            Vector2 newForceThisFrame = Vector2.zero;
            if (Accelerate)
            {

                if (_rigidbody2D.velocity.magnitude < _topSpeed)
                {
                    newForceThisFrame += (Vector2) (transform.up * _acceleration);
                }

                if (Steering != 0 && _rigidbody2D.velocity.magnitude > 2f)
                {
                    newForceThisFrame += (Vector2) (transform.right * Steering) * _steeringStrength;
                }

                newForceThisFrame *= Time.deltaTime;

                _rigidbody2D.AddForceAtPosition(newForceThisFrame, _frontAxel.position, ForceMode2D.Force);
                return;
            }

        }
    }
}
