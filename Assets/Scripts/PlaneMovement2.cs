using UnityEngine;
using TMPro;

public class PlaneMovement2 : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _speed;
    [SerializeField] private Rigidbody _planeRigidbody;
    [SerializeField] private Rigidbody _planeBodyRigidbody;
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _target;
    [SerializeField] private float _horizontalDisplacement;
    [SerializeField] private float _horizontalTime;
    [SerializeField] private float _horizontalVelocity;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _horizontalAccelerationTime;
    [SerializeField] private float _rotationalVelocity;
    [SerializeField] private float _rotationalDragUp;
    [SerializeField] private float _rotationalDragDown;
    [SerializeField] private float _rotationalTime;
    private float _timer;
    private Vector3 _direction;
    private float _originMagnitude;
    private Vector3 _originNormalized;
    private float _directionMagnitude;
    private Vector3 _directionNormalized;
    private float tilt;

    private void Update()
    {
        _timer = Time.deltaTime;

        Accelerate();
        Rotate();

        _speed.text = $"Speed: {_planeRigidbody.velocity.z}";
    }

    private void Accelerate()
    {

        GetVelocity();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _planeRigidbody.velocity = transform.forward * Mathf.Lerp(_planeRigidbody.velocity.z, _maxSpeed, _timer / _horizontalAccelerationTime);
        }
        else
        {
            _planeRigidbody.velocity = transform.forward * Mathf.Lerp(_planeRigidbody.velocity.z, _horizontalVelocity, _timer / _horizontalAccelerationTime);
        }

        _planeBodyRigidbody.velocity = _planeRigidbody.velocity;
        _planeBodyRigidbody.transform.localPosition = Vector3.zero;

    }

    private void Rotate()
    {

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _target.position += Vector3.up * 0.1f;
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            _target.position -= Vector3.up * 0.1f;
        }

        if (TiltLimit())
        {
            _planeBodyRigidbody.angularVelocity = GetRotationAxis() * _rotationalVelocity * _planeBodyRigidbody.transform.right;
        }
        else
        {
            _planeBodyRigidbody.angularVelocity = Vector3.zero;
        }

    }

    private bool TiltLimit()
    {
        _direction = GetDirection(origin: _origin.forward, target: _target.position);
        _originMagnitude = GetMagnitude(vector: _origin.forward);
        _originNormalized = GetNormalized(vector: _origin.forward, magnitude: _originMagnitude);
        _directionMagnitude = GetMagnitude(vector: _target.position);
        _directionNormalized = GetNormalized(vector: _target.position, magnitude: _directionMagnitude);

        tilt = Vector3.Dot(_originNormalized, _directionNormalized);

        return tilt >= 0.8f;
    }

    private void GetVelocity()
    {
        _horizontalVelocity = GetDrag() * _horizontalDisplacement / _horizontalTime;
    }

    private Vector3 GetDirection(Vector3 origin, Vector3 target)
    {
        return target - origin;
    }

    private float GetMagnitude(Vector3 vector)
    {
        return vector.magnitude;
    }

    private Vector3 GetNormalized(Vector3 vector, float magnitude)
    {
        return vector / magnitude;
    }

    private float GetRotationAxis()
    {

        _direction = GetDirection(origin: _planeBodyRigidbody.position, target: _target.position);
        _originMagnitude = GetMagnitude(vector: _planeBodyRigidbody.transform.forward);
        _originNormalized = GetNormalized(vector: _planeBodyRigidbody.transform.forward, magnitude: _originMagnitude);
        _directionMagnitude = GetMagnitude(vector: _target.position);
        _directionNormalized = GetNormalized(vector: _target.position, magnitude: _directionMagnitude);

        return Vector3.Cross(_originNormalized, _directionNormalized).x;
    }

    private float GetDrag()
    {
        if (_planeBodyRigidbody.rotation.x > 0)
        {
            return _rotationalDragDown;
        }
        else if (_planeBodyRigidbody.rotation.x < 0)
        {
            return _rotationalDragUp;
        }
        else
        {
            return 1f;
        }
    }

}
