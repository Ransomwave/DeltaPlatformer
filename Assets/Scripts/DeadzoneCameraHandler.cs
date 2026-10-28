using UnityEngine;

public class DeadzoneCameraHandler : MonoBehaviour {
    public Transform target;

    public float horizontalLimit = 2f;
    public float verticalLimit = 1.5f;

    public float smoothTime = 0.15f;

    private Vector3 velocity;

    private void LateUpdate() {
        if (target == null) {
            return;
        }

        var desired = transform.position;
        var targetPos = target.position;

        // horizontal dead zone
        if (targetPos.x < desired.x - horizontalLimit) {
            desired.x = targetPos.x + horizontalLimit;
        } else if (targetPos.x > desired.x + horizontalLimit) {
            desired.x = targetPos.x - horizontalLimit;
        }

        // vertical dead zone
        if (targetPos.y < desired.y - verticalLimit) {
            desired.y = targetPos.y + verticalLimit;
        } else if (targetPos.y > desired.y + verticalLimit) {
            desired.y = targetPos.y - verticalLimit;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(horizontalLimit * 2f, verticalLimit * 2f, 0f));
    }
}
