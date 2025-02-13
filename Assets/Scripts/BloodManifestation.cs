using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BloodTransition : MonoBehaviour
{
    public Material[] targetMaterials;
    public float manifestSpeed = 1.0f;
    public float demanifestSpeed = 1.0f;

    [Header("Auto Fade Settings")]
    [Tooltip("Time in seconds before auto-fade starts after manifestation completes")]
    public float autoFadeDelay = 0f;

    [Header("Events")]
    public UnityEvent OnManifestationStart;
    public UnityEvent OnManifestationComplete;
    public UnityEvent OnDemanifestationStart;
    public UnityEvent OnDemanifestationComplete;

    private Coroutine transitionRoutine;

    private void Start()
    {
    }

    public void StartManifestation()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionBlood(1.0f, manifestSpeed, true));
    }

    public void StartDemanifestation()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionBlood(0.0f, demanifestSpeed, false));
    }

    private IEnumerator TransitionBlood(float targetValue, float speed, bool isManifesting)
    {
        if (targetMaterials.Length == 0) yield break;

        // Trigger start event
        if (isManifesting)
            OnManifestationStart.Invoke();
        else
            OnDemanifestationStart.Invoke();

        float blend = targetMaterials[0].GetFloat("_Blend");

        while (!Mathf.Approximately(blend, targetValue))
        {
            blend = Mathf.MoveTowards(blend, targetValue, Time.deltaTime * speed);

            foreach (Material mat in targetMaterials)
            {
                mat.SetFloat("_Blend", blend);
            }

            yield return null;
        }

        // Trigger complete event
        if (isManifesting)
        {
            OnManifestationComplete.Invoke();

            // Start auto-fade after delay if configured
            if (autoFadeDelay > 0)
            {
                yield return new WaitForSeconds(autoFadeDelay);
                StartDemanifestation();
            }
        }
        else
        {
            OnDemanifestationComplete.Invoke();
        }
    }
}