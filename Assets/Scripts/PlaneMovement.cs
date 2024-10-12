using System.Threading;
using TMPro;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{

    #region GameObjects and Components
    [Header("GameObjects and Components")]
    [SerializeField] private TextMeshProUGUI _planeProperties;
    [SerializeField] private Rigidbody _planeRB;
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _target;
    #endregion

    #region Modifiable Attributes
    [Header("Modifiable Attributes")]
    [SerializeField] private float _velocity;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _accelerationTime;
    [SerializeField] private float _normalDragMultiplier;
    [SerializeField] private float _positiveDragMultiplier;
    [SerializeField] private float _negativeDragMultiplier;
    [SerializeField] private float _angularVelocity;
    #endregion

    #region Unmodifiable Attributes

    private float _timer;
    private Vector3 _planePosition;
    private Vector3 _targetPosition;
    private Vector3 _direction;
    private float _directionMagnitude;
    private Vector3 _directionNormalized;
    #endregion

    private void Start()
    {
        _planeRB = GetComponent<Rigidbody>();
        _planeRB.velocity = transform.forward * _velocity * GetDrag();
    }

    private void Update()
    {
        SetPositions();
        Accelerate();
        Rotate();
        SetText();
    }

    private void SetPositions()
    {
        _origin.position = transform.position;
        _target.position = _target.up * Mathf.Clamp(_target.position.y, transform.position.y - 25f, transform.position.y + 25f) + _target.forward * (25f + transform.position.z);
    }

    private void Accelerate()
    {
        _timer = Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            _planeRB.velocity = transform.forward * Mathf.Lerp(_planeRB.velocity.magnitude, _maxVelocity * GetDrag(), _timer / _accelerationTime);
        }
        else
        {
            _planeRB.velocity = transform.forward * Mathf.Lerp(_planeRB.velocity.magnitude, _velocity * GetDrag(), _timer / _accelerationTime);
        }
    }

    private float GetDrag()
    {
        if (_planeRB.rotation.x > 0.1f)
        {
            return _positiveDragMultiplier;
        }
        else if (_planeRB.rotation.x < -0.1f)
        {
            return _negativeDragMultiplier;
        }
        else
        {
            return _normalDragMultiplier;
        }
    }



    private void Rotate()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _target.position += Vector3.up * 0.1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _target.position -= Vector3.up * 0.1f;
        }

        _planePosition = transform.position;
        _targetPosition = _target.position;
        _direction = _targetPosition - _planePosition;
        _directionMagnitude = _direction.magnitude;
        _directionNormalized = _direction / _directionMagnitude;

        if (TiltLimit())
        {
            _planeRB.angularVelocity = Vector3.Cross(transform.forward, _directionNormalized).x * _angularVelocity * transform.right;
        }
        else
        {
            _planeRB.angularVelocity = Vector3.zero;
        }
    }

    private bool TiltLimit()
    {
        float _tilt = Vector3.Dot(_origin.forward, _directionNormalized);
        return _tilt >= 0.90f;
    }

    private void SetText()
    {

        float depthVelocity = Round(_planeRB.velocity.z);
        string depthVelocityText = GetText("Depth", "Z-Axis", "Velocity", depthVelocity, "unit/s", "\n");

        float verticalVelocity = Round(_planeRB.velocity.y);
        string verticalVelocityText = GetText("Vertical", "Y-Axis", "Velocity", verticalVelocity, "unit/s", "\n");

        float resultantVelocity = Round(_planeRB.velocity.magnitude);
        string resultantVelocityText = GetText("Resultant", "Diagonal", "Velocity", resultantVelocity, "unit/s", "\n\n");

        float depthDistance = Round(_planeRB.position.z);
        string depthDistanceText = GetText("Depth", "Z-Axis", "Distance", depthDistance, "unit", "\n");

        float verticalDistacne = Round(_planeRB.position.y);
        string verticalDistanceText = GetText("Vertical", "Y-Axis", "Distance", verticalDistacne, "unit", "\n");

        Vector3 distanceFromOrigin = _planeRB.position - Vector3.zero;
        float magnitudeFromOrigin = distanceFromOrigin.magnitude;
        float resultantDisplacement = Round(magnitudeFromOrigin);
        string resultantDisplacementText = GetText("Resultant", "Diagonal", "Displacement", resultantDisplacement, "unit");

        _planeProperties.text = depthVelocityText + verticalVelocityText + resultantVelocityText + depthDistanceText + verticalDistanceText + resultantDisplacementText;
    }

    private float Round(float value)
    {
        return Mathf.Round(value * 100) / 100;
    }

    private string GetText(string name, string axis, string concept, float value, string unit, string endLine = "")
    {
        return $"{name} ({axis}) {concept}: {value} {unit}{endLine}";
    }

}
