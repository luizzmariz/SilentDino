using System;
using System.Collections;
using UnityEngine;

public class UnliterTextures : MonoBehaviour
{
    [Header("Pastas de Materiais (Dentro de 'Resources')")]
    [Tooltip("Pasta dentro de Resources onde estão os materiais Overlay (ex.: 'Overlay')")]
    public string overlayFolderPath = "Overlay";

    [Tooltip("Pasta dentro de Resources onde estão os materiais Unlit (ex.: 'Unlit')")]
    public string unlitFolderPath = "Unlit";

    [Header("Objeto Pai (com filhos que possuem MeshRenderers)")]
    [Tooltip("Arraste aqui o GameObject cujo(s) filho(s) terão materiais substituídos.")]
    public GameObject rootObject;

    void Start()
    {
        // Executa a troca inicial para Unlit
        ChangeToUnlit();
    }

    /// <summary>
    /// Troca todos os materiais "XYZ - Overlay" para "XYZ - Unlit" dentro do GameObject raiz.
    /// </summary>
    public void ChangeToUnlit()
    {
        if (rootObject == null)
        {
            Debug.LogWarning("Root Object não atribuído!");
            return;
        }

        MeshRenderer[] meshRenderers = rootObject.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer rend in meshRenderers)
        {
            Material[] mats = rend.sharedMaterials;
            bool hasChanges = false;

            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = mats[i];
                if (mat == null) continue;

                // Verifica se o nome do material termina com " - Overlay"
                if (mat.name.EndsWith(" - Overlay"))
                {
                    string baseName = mat.name.Replace(" - Overlay", "");
                    string unlitName = baseName + " - Unlit";

                    // Carrega o material Unlit da pasta especificada
                    Material unlitMat = Resources.Load<Material>(unlitFolderPath + "/" + unlitName);
                    if (unlitMat != null)
                    {
                        mats[i] = unlitMat;
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.LogWarning("Material Unlit não encontrado: " + unlitFolderPath + "/" + unlitName);
                    }
                }
            }

            if (hasChanges)
            {
                rend.sharedMaterials = mats;
            }
        }
    }

    /// <summary>
    /// Troca todos os materiais "XYZ - Unlit" para "XYZ - Overlay" dentro do GameObject raiz.
    /// </summary>
    public void ChangeToOverlay()
    {
        if (rootObject == null)
        {
            Debug.LogWarning("Root Object não atribuído!");
            return;
        }

        MeshRenderer[] meshRenderers = rootObject.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer rend in meshRenderers)
        {
            Material[] mats = rend.sharedMaterials;
            bool hasChanges = false;

            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = mats[i];
                if (mat == null) continue;

                // Verifica se o nome do material termina com " - Unlit"
                if (mat.name.EndsWith(" - Unlit"))
                {
                    string baseName = mat.name.Replace(" - Unlit", "");
                    string overlayName = baseName + " - Overlay";

                    // Carrega o material Overlay da pasta especificada
                    Material overlayMat = Resources.Load<Material>(overlayFolderPath + "/" + overlayName);
                    if (overlayMat != null)
                    {
                        mats[i] = overlayMat;
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.LogWarning("Material Overlay não encontrado: " + overlayFolderPath + "/" + overlayName);
                    }
                }
            }

            if (hasChanges)
            {
                rend.sharedMaterials = mats;
            }
        }
    }
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
