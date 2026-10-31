using System;
using UnityEngine;

public class EndDoorHandler : MonoBehaviour {
    public event Action OnEndDoorTouched = delegate { };

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Contains("Player")) {
            OnEndDoorTouched?.Invoke();
        }
    }
}
