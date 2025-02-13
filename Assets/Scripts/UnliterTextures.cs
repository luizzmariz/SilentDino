using System;
using System.Collections;
using UnityEngine;

public class UnliterTextures : MonoBehaviour
{
    public Material[] materialsToUnlit = null;
    public Shader unlitURPShader;
    public Shader customShader;

    void Start()
    {
        ChangeToUnlit();
    }

    public void ChangeToUnlit()
    {
        if (unlitURPShader == null)
        {
            unlitURPShader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        if (unlitURPShader != null)
        {
            foreach (Material mat in materialsToUnlit)
            {
                if (mat != null)
                {
                    mat.shader = unlitURPShader;
                }
            }
        }
        else
        {
            Debug.LogWarning("Unlit URP shader not found!");
        }
    }

    public void ChangeToCustomShader()
    {
        if (customShader != null)
        {
            foreach (Material mat in materialsToUnlit)
            {
                if (mat != null)
                {
                    mat.shader = customShader;
                }
            }
        }
        else
        {
            Debug.LogWarning("Custom shader is not assigned!");
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
