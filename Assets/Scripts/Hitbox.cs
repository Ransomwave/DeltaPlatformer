using System;
using UnityEngine;

namespace deltaformer {
    public class Hitbox : MonoBehaviour {
        public bool PlayerDetected;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag.Contains("Player") && !PlayerDetected) {
                PlayerDetected = true;
                OnEnter?.Invoke();
            }
        }

        public static event Action OnEnter;
    }
}