using UnityEngine;

namespace deltaformer {
    public class ParallaxPersonalizado : MonoBehaviour {
        [Header("Efecto Parallax Horizontal (X)")]
        [Tooltip("0 = Va pegado a la cámara. 1 = No se mueve de lado (Efecto infinito lejanía)")]
        [Range(0f, 1f)] public float velocidadParallaxX = 0.5f;

        [Header("Efecto Parallax Vertical (Y)")]
        [Tooltip("0 = Sigue a la cámara idéntico. 1 = Se queda completamente quieto arriba/abajo")]
        [Range(0.8f, 1f)] public float friccionVerticalY = 0.95f; // Un valor alto como 0.95 hace que suba "muy poquito"

        [Header("Encuadre Inicial Vertical")]
        [Tooltip("Ajusta este valor en el editor para bajar el fondo y mostrar de la mitad para abajo al iniciar")]
        public float desfaseVerticalInicial = 4f;

        private Transform camaraTransform;
        private float posicionInicialX;
        private float longitudSprite;

        void Start() {
            camaraTransform = Camera.main.transform;
            posicionInicialX = transform.position.x;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null) {
                // Al usar .sprite.bounds leemos el tamaño real de la imagen original (5.6 unidades), 
                // ignorando por completo el Width de 50 que pusimos para el mosaico.
                longitudSprite = spriteRenderer.sprite.bounds.size.x;
            }

            Vector3 pos = transform.position;
            pos.y = camaraTransform.position.y + desfaseVerticalInicial;
            transform.position = pos;
        }


        void LateUpdate() {
            // 1. CALCULAR PARALLAX HORIZONTAL (SUAVE)
            // Esto hace que el fondo se mueva más lento que la cámara
            float movimientoParallaxX = camaraTransform.position.x * (1 - velocidadParallaxX);

            // 2. CONTROL DEL BUCLE INFINITO (AJUSTADO CON MARGEN EXTRA)
            // Calcula cuántas pantallas completas de fondo ha recorrido la cámara
            float distanciaRelativaCamara = camaraTransform.position.x * velocidadParallaxX;

            // Añadimos un pequeño margen (ej. 2 unidades de Unity) para que el salto ocurra ANTES 
            // de que la pantalla llegue al borde visible de la repetición.
            float margenSeguridad = 2.0f;

            if (distanciaRelativaCamara > posicionInicialX + longitudSprite - margenSeguridad) {
                posicionInicialX += longitudSprite;
            } else if (distanciaRelativaCamara < posicionInicialX - longitudSprite + margenSeguridad) {
                posicionInicialX -= longitudSprite;
            }

            // 3. PARALLAX VERTICAL (SUTIL)
            float distanciaY = camaraTransform.position.y * (1 - friccionVerticalY);

            // 4. APLICAR POSICIÓN FINAL
            // Combinamos la posición base del bucle con el movimiento suave del parallax
            transform.position = new Vector3(
                posicionInicialX + movimientoParallaxX,
                (camaraTransform.position.y + desfaseVerticalInicial) - distanciaY,
                transform.position.z
            );
        }

    }
}
