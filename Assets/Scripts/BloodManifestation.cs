using UnityEngine;
using System.Collections;

public class BloodTransition : MonoBehaviour
{
    public Material[] targetMaterials; // Lista de materiais
    public float manifestSpeed = 1.0f;
    public float demanifestSpeed = 1.0f;

    private Coroutine transitionRoutine;

    private void Start()
    {
        StartDemanifestation();
    }
    public void StartManifestation()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionBlood(1.0f, manifestSpeed));
    }

    public void StartDemanifestation()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionBlood(0.0f, demanifestSpeed));
    }

    private IEnumerator TransitionBlood(float targetValue, float speed)
    {
        if (targetMaterials.Length == 0) yield break;

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
    }
}
