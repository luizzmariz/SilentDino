using UnityEngine;
using Cinemachine;

public class CinemachineScreenShake : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera; // Referência à câmera virtual do Cinemachine

    [Header("Shake Parameters")]
    public float shakeDuration = 0.5f;  // Duração total do screenshake
    public float shakeAmplitude = 2.0f; // Intensidade do movimento
    public float shakeFrequency = 2.0f; // Frequência do movimento

    private CinemachineBasicMultiChannelPerlin noiseComponent; // Componente de ruído
    private float shakeTimer = 0f;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera não foi atribuída!");
            return;
        }

        // Obter o componente de ruído da câmera
        noiseComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noiseComponent == null)
        {
            Debug.LogError("A Cinemachine Virtual Camera precisa de um componente 'CinemachineBasicMultiChannelPerlin'!");
        }
    }

    private void Update()
    {
        // Atualizar o timer do screenshake
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0)
            {
                StopShake();
            }
        }
    }

    public void TriggerShake(float customDuration = -1f, float customAmplitude = -1f, float customFrequency = -1f)
    {
        // Configurar os valores personalizados ou usar os padrões
        float duration = customDuration > 0 ? customDuration : shakeDuration;
        float amplitude = customAmplitude > 0 ? customAmplitude : shakeAmplitude;
        float frequency = customFrequency > 0 ? customFrequency : shakeFrequency;

        // Aplicar os valores ao componente de ruído
        if (noiseComponent != null)
        {
            noiseComponent.m_AmplitudeGain = amplitude;
            noiseComponent.m_FrequencyGain = frequency;
        }

        // Configurar o tempo de shake
        shakeTimer = duration;
    }

    private void StopShake()
    {
        if (noiseComponent != null)
        {
            noiseComponent.m_AmplitudeGain = 0f;
            noiseComponent.m_FrequencyGain = 0f;
        }
    }
}
