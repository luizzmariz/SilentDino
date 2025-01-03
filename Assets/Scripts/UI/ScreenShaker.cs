using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CinemachineScreenShake : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera; // Referência à câmera virtual do Cinemachine

    [Header("Shake Parameters")]
    public float shakeDuration = 0.5f;  // Duração total do screenshake
    public float shakeAmplitude = 2.0f; // Intensidade do movimento
    public float shakeFrequency = 2.0f; // Frequência do movimento

    [Header("Noise Effect Settings")]
    public float noiseGrainIntensity = 2.0f; // Intensidade do "grão" no efeito de ruído
    public Color noiseColor = Color.red; // Cor do ruído durante o tremor

    private CinemachineBasicMultiChannelPerlin noiseComponent; // Componente de ruído
    private float shakeTimer = 0f;

    private Vector3 initialCameraPosition;
    private bool isShaking = false;

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

        // Obter a posição inicial da câmera
        initialCameraPosition = virtualCamera.transform.position;
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

    public void callShake()
    {
        TriggerShake();
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
        isShaking = true;

        // Aplicar o efeito de ruído personalizado
        ApplyNoiseEffect();

        // Impedir o movimento do jogador
        DisablePlayerMovement();
    }

    private void StopShake()
    {
        if (noiseComponent != null)
        {
            noiseComponent.m_AmplitudeGain = 0f;
            noiseComponent.m_FrequencyGain = 0f;
        }

        // Restaurar a posição inicial da câmera
        virtualCamera.transform.position = initialCameraPosition;

        // Reverter o efeito de ruído personalizado
        ResetNoiseEffect();

        // Permitir o movimento do jogador
        EnablePlayerMovement();

        isShaking = false;
    }

    private void ApplyNoiseEffect()
    {
        // Lógica para aplicar ruído mais "grosso" e avermelhado
        // Por exemplo, ajustar material ou shader específico
        Debug.Log("Aplicando efeito de ruído: intensidade " + noiseGrainIntensity + " e cor " + noiseColor);
    }

    private void ResetNoiseEffect()
    {
        // Lógica para reverter as alterações no ruído
        Debug.Log("Revertendo efeito de ruído para estado original.");
    }

    private void DisablePlayerMovement()
    {
        // Implementar lógica para desativar o movimento do jogador
        // Exemplo:
        // PlayerController.instance.enabled = false;
    }

    private void EnablePlayerMovement()
    {
        // Implementar lógica para reativar o movimento do jogador
        // Exemplo:
        // PlayerController.instance.enabled = true;
    }
}
