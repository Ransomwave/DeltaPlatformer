using UnityEngine;

public class BackgroundHandler : MonoBehaviour {
    private Transform cam;

    void Start() {
        cam = Camera.main.transform;
    }

    // late update so this updates after the camera has moved
    void LateUpdate() {
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);
    }
}
