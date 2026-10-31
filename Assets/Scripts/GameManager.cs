using System.Collections;
using deltaformer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private const float FadeTime = 1f;

    [Header("Player Settings")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _spawnPoint;

    [Header("Level")]
    [SerializeField] private EndDoorHandler _endDoorHandler;
    [SerializeField] private int _currentLevel;
    [SerializeField] private bool _isLastLevel;

    [Header("UI")]
    [SerializeField] private CanvasGroup _fadeOverlay;

    // Player stats
    private static int _bananaCount = 0;

    // Scenes
    private const string MainMenuScene = "MainMenu";
    private const string GameOverScene = "GameOver";

    // the constant naming convention in Unity is PascalCase (no prefixed underscore) or `k_`
    // `k_` looks really ugly, so PascalCase it is

    private bool _isTransitioning;

    private void Awake() {
        Assert.IsNotNull(_playerController, "PlayerController is not assigned in the GameManager.");
        Assert.IsNotNull(_spawnPoint, "SpawnPoint is not assigned in the GameManager.");
        Assert.IsNotNull(_endDoorHandler, "EndDoorHandler is not assigned in the GameManager.");
        Assert.IsNotNull(_fadeOverlay, "FadeOverlay is not assigned in the GameManager.");

        _endDoorHandler.OnEndDoorTouched += OnEndDoorTouched;
        _playerController.OnPlayerDeath += OnPlayerDeath;
    }

    private void Start() {
        _fadeOverlay.alpha = 1f;
        StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha) {
        var startAlpha = _fadeOverlay.alpha;
        for (var t = 0f; t < FadeTime; t += Time.unscaledDeltaTime) {
            _fadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / FadeTime);
            yield return null;
        }
        _fadeOverlay.alpha = targetAlpha;
    }

    public void IncrementBananaCount() {
        _bananaCount++;
    }

    private IEnumerator LoadLevelRoutine(string levelName) {
        _isTransitioning = true;
        yield return Fade(1f);
        SceneManager.LoadScene(levelName);
    }

    private IEnumerator RespawnRoutine() {
        _isTransitioning = true;
        yield return Fade(1f);
        _playerController.Respawn(_spawnPoint.position);
        yield return Fade(0f);
        _isTransitioning = false;
    }

    private void LoadLevel(int levelIndex) {
        if (_isTransitioning) {
            return;
        }

        var levelName = $"Level{levelIndex}";
        Debug.Log($"Loading level: {levelName}");
        StartCoroutine(LoadLevelRoutine(levelName));
    }

    private void OnEndDoorTouched() {
        if (_isLastLevel) {
            Debug.Log("Game completed!");
            // Implement game completion logic here (e.g., show a victory screen)
        } else {
            LoadLevel(_currentLevel + 1);
        }
    }

    private void OnPlayerDeath() {
        if (_isTransitioning) {
            return;
        }

        StartCoroutine(RespawnRoutine());
    }
}
