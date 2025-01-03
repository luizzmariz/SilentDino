using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CinemachineScreenShake : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera; // Refer�ncia � c�mera virtual do Cinemachine

    [Header("Shake Parameters")]
    public float shakeDuration = 0.5f;  // Dura��o total do screenshake
    public float shakeAmplitude = 2.0f; // Intensidade do movimento
    public float shakeFrequency = 2.0f; // Frequ�ncia do movimento

    [Header("Noise Effect Settings")]
    public float noiseGrainIntensity = 2.0f; // Intensidade do "gr�o" no efeito de ru�do
    public Color noiseColor = Color.red; // Cor do ru�do durante o tremor

    private CinemachineBasicMultiChannelPerlin noiseComponent; // Componente de ru�do
    private float shakeTimer = 0f;

    private Vector3 initialCameraPosition;
    private bool isShaking = false;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera n�o foi atribu�da!");
            return;
        }

        // Obter o componente de ru�do da c�mera
        noiseComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noiseComponent == null)
        {
            Debug.LogError("A Cinemachine Virtual Camera precisa de um componente 'CinemachineBasicMultiChannelPerlin'!");
        }

        // Obter a posi��o inicial da c�mera
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
        // Configurar os valores personalizados ou usar os padr�es
        float duration = customDuration > 0 ? customDuration : shakeDuration;
        float amplitude = customAmplitude > 0 ? customAmplitude : shakeAmplitude;
        float frequency = customFrequency > 0 ? customFrequency : shakeFrequency;

        // Aplicar os valores ao componente de ru�do
        if (noiseComponent != null)
        {
            noiseComponent.m_AmplitudeGain = amplitude;
            noiseComponent.m_FrequencyGain = frequency;
        }

        // Configurar o tempo de shake
        shakeTimer = duration;
        isShaking = true;

        // Aplicar o efeito de ru�do personalizado
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

        // Restaurar a posi��o inicial da c�mera
        virtualCamera.transform.position = initialCameraPosition;

        // Reverter o efeito de ru�do personalizado
        ResetNoiseEffect();

        // Permitir o movimento do jogador
        EnablePlayerMovement();

        isShaking = false;
    }

    private void ApplyNoiseEffect()
    {
        // L�gica para aplicar ru�do mais "grosso" e avermelhado
        // Por exemplo, ajustar material ou shader espec�fico
        Debug.Log("Aplicando efeito de ru�do: intensidade " + noiseGrainIntensity + " e cor " + noiseColor);
    }

    private void ResetNoiseEffect()
    {
        // L�gica para reverter as altera��es no ru�do
        Debug.Log("Revertendo efeito de ru�do para estado original.");
    }

    private void DisablePlayerMovement()
    {
        // Implementar l�gica para desativar o movimento do jogador
        // Exemplo:
        // PlayerController.instance.enabled = false;
    }

    private void EnablePlayerMovement()
    {
        // Implementar l�gica para reativar o movimento do jogador
        // Exemplo:
        // PlayerController.instance.enabled = true;
    }
}
