using UnityEngine;

public class DeadzoneCameraHandler : MonoBehaviour {
    [SerializeField] private Transform _target;

    [SerializeField] private float _horizontalLimit = 2f;
    [SerializeField] private float _verticalLimit = 1.5f;

    [SerializeField] private float _smoothTime = 0.15f;

    private Vector3 _velocity;

    private void LateUpdate() {
        if (_target == null) {
            return;
        }

        var desired = transform.position;
        var targetPos = _target.position;

        // horizontal dead zone
        if (targetPos.x < desired.x - _horizontalLimit) {
            desired.x = targetPos.x + _horizontalLimit;
        } else if (targetPos.x > desired.x + _horizontalLimit) {
            desired.x = targetPos.x - _horizontalLimit;
        }

        // vertical dead zone
        if (targetPos.y < desired.y - _verticalLimit) {
            desired.y = targetPos.y + _verticalLimit;
        } else if (targetPos.y > desired.y + _verticalLimit) {
            desired.y = targetPos.y - _verticalLimit;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(_horizontalLimit * 2f, _verticalLimit * 2f, 0f));
    }
}
