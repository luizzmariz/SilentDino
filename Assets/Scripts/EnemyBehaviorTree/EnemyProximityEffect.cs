using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Ou HDRP, dependendo do seu pipeline

public class EnemyProximityEffect : MonoBehaviour
{
    [Header("Refer�ncias de Cena")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float lerpSpeed = 3f; // controle da suavidade

    [Header("Overlay UI")]
    [SerializeField] private CanvasGroup enemyOverlay; // imagem com CanvasGroup

    [Header("P�s-Processamento")]
    [SerializeField] private Volume postProcessVolume; // arraste seu Volume no Inspector

    // Refer�ncias internas para efeitos:
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    // Valores m�ximos que voc� quer atingir
    [Header("Intensidade M�xima")]
    [Range(0f, 1f)] public float maxChromaticAberration = 1f;
    [Range(0f, 1f)] public float maxGrainIntensity = 0.5f;
    [Range(0f, 1f)] public float maxOverlayAlpha = 1f;

    private void Start()
    {
        // Tentar obter as refer�ncias ao ChromaticAberration e FilmGrain do volume
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out filmGrain);
        }

        // Certifique-se de iniciar sem efeito
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
    }

    private void Update()
    {
        // Calcula a dist�ncia
        float distance = Vector3.Distance(player.position, enemy.position);

        // Normaliza no range [0,1], onde 0 = longe, 1 = muito pr�ximo
        // Aqui, se o inimigo estiver a maxDistance ou mais, o efeito ser� 0.
        // Se estiver a 0 de dist�ncia, o efeito ser� 1.
        float proximityValue = 1 - Mathf.Clamp01(distance / maxDistance);

        // Aplica lerp suave para cada par�metro

        // Overlay Alpha
        if (enemyOverlay != null)
        {
            float currentAlpha = enemyOverlay.alpha;
            float targetAlpha = proximityValue * maxOverlayAlpha;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * lerpSpeed);
            enemyOverlay.alpha = newAlpha;
        }

        // Chromatic Aberration
        if (chromaticAberration != null)
        {
            float currentCA = chromaticAberration.intensity.value;
            float targetCA = proximityValue * maxChromaticAberration;
            float newCA = Mathf.Lerp(currentCA, targetCA, Time.deltaTime * lerpSpeed);
            chromaticAberration.intensity.Override(newCA);
        }

        // Grain
        if (filmGrain != null)
        {
            float currentGrain = filmGrain.intensity.value;
            float targetGrain = proximityValue * maxGrainIntensity;
            float newGrain = Mathf.Lerp(currentGrain, targetGrain, Time.deltaTime * lerpSpeed);
            filmGrain.intensity.Override(newGrain);
        }
    }

    // Chamando manualmente se quiser "for�ar" um estado (por exemplo, algum trigger especial)
    public void SetEffectsManually(float normalizedValue)
    {
        // normalizedValue deve ser entre 0 e 1, 0 = sem efeito, 1 = m�ximo
        if (enemyOverlay != null)
        {
            float newAlpha = normalizedValue * maxOverlayAlpha;
            enemyOverlay.alpha = newAlpha;
        }

        if (chromaticAberration != null)
        {
            float newCA = normalizedValue * maxChromaticAberration;
            chromaticAberration.intensity.Override(newCA);
        }

        if (filmGrain != null)
        {
            float newGrain = normalizedValue * maxGrainIntensity;
            filmGrain.intensity.Override(newGrain);
        }
    }

    // Exemplo de fun��o que poderia ser chamada via trigger para "sumir" com o efeito
    public void ClearEffectsSmoothly()
    {
        StartCoroutine(ClearEffectsCoroutine());
    }

    private System.Collections.IEnumerator ClearEffectsCoroutine()
    {
        float duration = 1f; // tempo para limpar
        float timeElapsed = 0f;

        // Captura valores atuais
        float startAlpha = enemyOverlay != null ? enemyOverlay.alpha : 0f;
        float startCA = chromaticAberration != null ? chromaticAberration.intensity.value : 0f;
        float startGrain = filmGrain != null ? filmGrain.intensity.value : 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            if (enemyOverlay != null)
            {
                enemyOverlay.alpha = Mathf.Lerp(startAlpha, 0f, t);
            }

            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.Override(Mathf.Lerp(startCA, 0f, t));
            }

            if (filmGrain != null)
            {
                filmGrain.intensity.Override(Mathf.Lerp(startGrain, 0f, t));
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ao final, zera tudo
        if (enemyOverlay != null) enemyOverlay.alpha = 0f;
        if (chromaticAberration != null) chromaticAberration.intensity.Override(0f);
        if (filmGrain != null) filmGrain.intensity.Override(0f);
    }
}
