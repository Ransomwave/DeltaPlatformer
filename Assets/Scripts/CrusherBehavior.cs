using System.Collections;
using UnityEngine;

namespace deltaformer {
    public class CrusherBehavior : MonoBehaviour {
        [Header("Timings settings")] [SerializeField]
        private float _warningDuration = 0.5f;

        [SerializeField] private float _groundWaitDuration = 1f;

        [Header("Velocity settings")] [SerializeField]
        private float _fallSpeed = 25f;

        [SerializeField] private float _returnSpeed = 5f;
        private BoxCollider2D _crusherCollider;
        private bool _isActionActive;

        // Ground layer

        private Vector3 _startPos;

        private void Start() {
            _startPos = transform.position;
            _crusherCollider = GetComponent<BoxCollider2D>();
        }


        //WIP
        public void PlayerDetected() {
            if (_isActionActive) return;

            StartCoroutine(CrushSequence());
        }

        private IEnumerator CrushSequence() {
            _isActionActive = true;
            yield return null;
        }
    }
}