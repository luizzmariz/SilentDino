using System.Collections;
using UnityEngine;

public class UnliterTextures : MonoBehaviour
{
    [Tooltip("Materiais que possuem o shader BloodOverlay e que iremos trocar para Unlit e vice-versa.")]
    public Material[] materialsToUnlit;

    [Tooltip("Shader Unlit (URP) a ser aplicado ao trocar.")]
    public Shader unlitURPShader;

    [Tooltip("Shader customizado BloodOverlay.")]
    public Shader customShader;

    // Array para armazenar todas as propriedades de cada material BloodOverlay
    private BloodOverlayProperties[] bloodPropertiesArray;

    // Chamado antes do Start. Vamos extrair as propriedades iniciais do BloodOverlay.
    private void Awake()
    {
        if (materialsToUnlit == null || materialsToUnlit.Length == 0)
            return;

        bloodPropertiesArray = new BloodOverlayProperties[materialsToUnlit.Length];

        // Para cada material, extraímos as propriedades do shader BloodOverlay
        for (int i = 0; i < materialsToUnlit.Length; i++)
        {
            Material mat = materialsToUnlit[i];
            if (mat != null && mat.shader == customShader)
            {
                bloodPropertiesArray[i] = ExtractBloodOverlayProperties(mat);
            }
            else
            {
                // Se o material não estiver com o shader custom no início,
                // apenas inicializa com valores padrão (ou você pode extrair de qualquer forma).
                bloodPropertiesArray[i] = new BloodOverlayProperties();
            }
        }
    }

    private void Start()
    {
        // Exemplo de fluxo:
        // 1) Garante que todos os materiais estejam no shader custom e com valores corretos
        ChangeToCustomShader();
        // 2) Em seguida, muda para o shader Unlit
        ChangeToUnlit();
    }

    /// <summary>
    /// Troca o shader para Unlit em todos os materiais do array,
    /// salvando antes os valores atualizados do BloodOverlay (caso tenham mudado).
    /// </summary>
    public void ChangeToUnlit()
    {
        if (unlitURPShader == null)
        {
            unlitURPShader = Shader.Find("Universal Render Pipeline/Unlit");
            if (unlitURPShader == null)
            {
                Debug.LogWarning("Não foi possível encontrar o shader Unlit URP!");
                return;
            }
        }

        for (int i = 0; i < materialsToUnlit.Length; i++)
        {
            Material mat = materialsToUnlit[i];
            if (mat == null) continue;

            // Se ainda estiver usando o shader BloodOverlay, re-extrai as propriedades
            // para capturar alterações que ocorreram em runtime (por ex. _Blend alterado).
            if (mat.shader == customShader)
            {
                bloodPropertiesArray[i] = ExtractBloodOverlayProperties(mat);
            }

            // Agora troca o shader do material para Unlit (sem criar nova instância).
            mat.shader = unlitURPShader;

            // Se quiser copiar cor/texture para manter visual:
            // if (mat.HasProperty("_MainTex")) {
            //     // _BaseMap é a prop do Unlit URP (ou "_MainTex", depende da variante)
            //     mat.SetTexture("_BaseMap", mat.GetTexture("_MainTex"));
            // }
            // if (mat.HasProperty("_MainColor")) {
            //     // Normalmente o Unlit URP usa "_BaseColor" ou "_Color"
            //     mat.SetColor("_BaseColor", mat.GetColor("_MainColor"));
            // }
        }
    }

    /// <summary>
    /// Restaura o shader BloodOverlay em todos os materiais,
    /// reaplicando as propriedades salvas (como _Blend, _Darkness etc.).
    /// </summary>
    public void ChangeToCustomShader()
    {
        if (customShader == null)
        {
            Debug.LogWarning("Shader customizado BloodOverlay não foi atribuído!");
            return;
        }

        for (int i = 0; i < materialsToUnlit.Length; i++)
        {
            Material mat = materialsToUnlit[i];
            if (mat == null) continue;

            // Se não estiver com o shader BloodOverlay, troca.
            if (mat.shader != customShader)
            {
                mat.shader = customShader;
            }

            // Restaura todas as propriedades salvas no array
            ApplyBloodOverlayProperties(mat, bloodPropertiesArray[i]);
        }
    }

    // ==========================================
    // Abaixo estão as funções auxiliares
    // ==========================================

    /// <summary>
    /// Extrai todas as propriedades relevantes do shader BloodOverlay e retorna em um struct.
    /// </summary>
    private BloodOverlayProperties ExtractBloodOverlayProperties(Material mat)
    {
        BloodOverlayProperties props = new BloodOverlayProperties();

        if (mat.HasProperty("_MainTex"))
            props.mainTex = mat.GetTexture("_MainTex");
        if (mat.HasProperty("_BloodTex"))
            props.bloodTex = mat.GetTexture("_BloodTex");
        if (mat.HasProperty("_DissolveTex"))
            props.dissolveTex = mat.GetTexture("_DissolveTex");

        if (mat.HasProperty("_Blend"))
            props.blend = mat.GetFloat("_Blend");
        if (mat.HasProperty("_Darkness"))
            props.darkness = mat.GetFloat("_Darkness");
        if (mat.HasProperty("_EnableFog"))
            props.enableFog = mat.GetFloat("_EnableFog");

        if (mat.HasProperty("_MainTexScale"))
            props.mainTexScale = mat.GetVector("_MainTexScale");
        if (mat.HasProperty("_BloodScale"))
            props.bloodScale = mat.GetVector("_BloodScale");
        if (mat.HasProperty("_DissolveScale"))
            props.dissolveScale = mat.GetFloat("_DissolveScale");

        if (mat.HasProperty("_DissolveThreshold"))
            props.dissolveThreshold = mat.GetFloat("_DissolveThreshold");
        if (mat.HasProperty("_DissolveSoftness"))
            props.dissolveSoftness = mat.GetFloat("_DissolveSoftness");
        if (mat.HasProperty("_UseLocalUV"))
            props.useLocalUV = mat.GetFloat("_UseLocalUV");

        if (mat.HasProperty("_MainColor"))
            props.mainColor = mat.GetColor("_MainColor");
        if (mat.HasProperty("_BloodColor"))
            props.bloodColor = mat.GetColor("_BloodColor");

        return props;
    }

    /// <summary>
    /// Aplica todos os valores do struct ao material com shader BloodOverlay.
    /// </summary>
    private void ApplyBloodOverlayProperties(Material mat, BloodOverlayProperties props)
    {
        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", props.mainTex);
        if (mat.HasProperty("_BloodTex"))
            mat.SetTexture("_BloodTex", props.bloodTex);
        if (mat.HasProperty("_DissolveTex"))
            mat.SetTexture("_DissolveTex", props.dissolveTex);

        if (mat.HasProperty("_Blend"))
            mat.SetFloat("_Blend", props.blend);
        if (mat.HasProperty("_Darkness"))
            mat.SetFloat("_Darkness", props.darkness);
        if (mat.HasProperty("_EnableFog"))
            mat.SetFloat("_EnableFog", props.enableFog);

        if (mat.HasProperty("_MainTexScale"))
            mat.SetVector("_MainTexScale", props.mainTexScale);
        if (mat.HasProperty("_BloodScale"))
            mat.SetVector("_BloodScale", props.bloodScale);
        if (mat.HasProperty("_DissolveScale"))
            mat.SetFloat("_DissolveScale", props.dissolveScale);

        if (mat.HasProperty("_DissolveThreshold"))
            mat.SetFloat("_DissolveThreshold", props.dissolveThreshold);
        if (mat.HasProperty("_DissolveSoftness"))
            mat.SetFloat("_DissolveSoftness", props.dissolveSoftness);
        if (mat.HasProperty("_UseLocalUV"))
            mat.SetFloat("_UseLocalUV", props.useLocalUV);

        if (mat.HasProperty("_MainColor"))
            mat.SetColor("_MainColor", props.mainColor);
        if (mat.HasProperty("_BloodColor"))
            mat.SetColor("_BloodColor", props.bloodColor);
    }

    // Métodos de controle de fog permanecem inalterados
    public void StartFog()
    {
        AdjustFogDensity(2.01f, 1.0f, 0.03f, 3.0f);
    }

    public void AdjustFogDensity(float targetDensity, float duration, float revertDensity, float revertDuration)
    {
        StartCoroutine(FogDensityRoutine(targetDensity, duration, revertDensity, revertDuration));
    }

    public IEnumerator FogDensityRoutine(float targetDensity, float duration, float revertDensity, float revertDuration)
    {
        float originalDensity = RenderSettings.fogDensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            RenderSettings.fogDensity = Mathf.Lerp(originalDensity, targetDensity, Mathf.SmoothStep(0f, 1f, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogDensity = targetDensity;
        yield return new WaitForSeconds(duration);

        elapsedTime = 0f;
        while (elapsedTime < revertDuration)
        {
            float t = elapsedTime / revertDuration;
            RenderSettings.fogDensity = Mathf.Lerp(targetDensity, revertDensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogDensity = revertDensity;
    }
}

/// <summary>
/// Estrutura para armazenar todas as propriedades do shader BloodOverlay,
/// para que possamos salvá-las e restaurá-las quando mudarmos para Unlit e depois voltarmos.
/// Ajuste se houver mais propriedades no seu shader.
/// </summary>
[System.Serializable]
public struct BloodOverlayProperties
{
    public Texture mainTex;
    public Texture bloodTex;
    public Texture dissolveTex;

    public float blend;
    public float darkness;
    public float enableFog;

    public Vector4 mainTexScale;
    public Vector4 bloodScale;
    public float dissolveScale;

    public float dissolveThreshold;
    public float dissolveSoftness;
    public float useLocalUV;

    public Color mainColor;
    public Color bloodColor;
}
