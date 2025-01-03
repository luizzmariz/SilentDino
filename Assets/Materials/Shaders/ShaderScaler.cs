using UnityEngine;

public class ShaderUnscaledTimeUpdater : MonoBehaviour
{
    [SerializeField] private Material targetMaterial; // Material associado ao objeto
    private float customTime; // Tempo acumulado manualmente

    private void Update()
    {
        // Atualiza o tempo customizado com base no tempo não escalado
        customTime += Time.unscaledDeltaTime;

        // Passa o tempo customizado para o material
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat("_CustomTime", customTime);
        }
    }
}
