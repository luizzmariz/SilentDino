using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using FMODUnity; // Biblioteca FMOD

public class EnemyProximityEffect : MonoBehaviour
{
    [Header("Referências de Cena")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float gameOverDistance = 10f; // Distância para acionar o Game Over
    [SerializeField] private float lerpSpeed = 3f;

    [Header("Overlay UI")]
    [SerializeField] private CanvasGroup enemyOverlay;
    [SerializeField] private Material overlayMaterial;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverText;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Pós-Processamento")]
    [SerializeField] private Volume postProcessVolume;

    [Header("FMOD")]
    [SerializeField] private EventReference gameOverSound; // Referência pública para o som

    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    [Header("Intensidade Máxima")]
    [Range(0f, 1f)] public float maxChromaticAberration = 1f;
    [Range(0f, 1f)] public float maxGrainIntensity = 0.5f;
    [Range(0f, 1f)] public float maxOverlayAlpha = 1f;

    private bool gameOverTriggered = false;

    private void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out filmGrain);
        }

        if (enemyOverlay != null)
        {
            enemyOverlay.alpha = 0f;
        }
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.Override(0f);
        }
        if (filmGrain != null)
        {
            filmGrain.intensity.Override(0f);
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOverTriggered) return;

        float distance = Vector3.Distance(player.position, enemy.position);

        // Atualiza o efeito visual com base na distância
        float proximityValue = 1 - Mathf.Clamp01(distance / maxDistance);

        if (enemyOverlay != null)
        {
            float currentAlpha = enemyOverlay.alpha;
            float targetAlpha = proximityValue * maxOverlayAlpha;
            enemyOverlay.alpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * lerpSpeed);
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.Override(Mathf.Lerp(
                chromaticAberration.intensity.value,
                proximityValue * maxChromaticAberration,
                Time.deltaTime * lerpSpeed
            ));
        }

        if (filmGrain != null)
        {
            filmGrain.intensity.Override(Mathf.Lerp(
                filmGrain.intensity.value,
                proximityValue * maxGrainIntensity,
                Time.deltaTime * lerpSpeed
            ));
        }

        // Passa o tempo não escalado para o material do shader
        if (overlayMaterial != null)
        {
            overlayMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
        }

        // Aciona o Game Over se a distância for menor ou igual à distância limite
        if (distance <= gameOverDistance)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        gameOverTriggered = true;

        // Pausa o tempo no jogo
        Time.timeScale = 0f;

        if (enemyOverlay != null)
        {
            enemyOverlay.alpha = 1f;

            // Adiciona o material no momento do Game Over
            if (overlayMaterial != null)
            {
                enemyOverlay.GetComponent<Image>().material = overlayMaterial;
            }
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Você Sucumbiu";
        }

        // Espaço para implementar o som do FMOD

        RuntimeManager.PlayOneShot(gameOverSound);

        StartCoroutine(GameOverSequence());
    }

    private System.Collections.IEnumerator GameOverSequence()
    {
        // Pausa visual (mesmo com o jogo pausado)
        yield return new WaitForSecondsRealtime(11.5f);

        // Restaura o tempo ao normal
        Time.timeScale = 1f;

        // Carrega a cena do menu
        SceneManager.LoadScene(menuSceneName);
    }
}
