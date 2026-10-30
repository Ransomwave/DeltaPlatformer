using System.Collections;
using UnityEngine;

namespace deltaformer {
    public class VanishingPlatform : MonoBehaviour {
        [SerializeField] private float timeToDestroy = 1f;
        private Coroutine _corDestroy;

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.tag.Contains("Player")) _corDestroy = StartCoroutine(DestroyPlatformAfterTime());
        }

        private void OnCollisionExit2D(Collision2D other) {
            if (other.gameObject.tag.Contains("Player") && _corDestroy != null) StopCoroutine(_corDestroy);
        }

        private IEnumerator DestroyPlatformAfterTime() {
            yield return new WaitForSeconds(timeToDestroy);
            Destroy(gameObject);
        }
    }
}