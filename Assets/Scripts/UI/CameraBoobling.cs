using StarterAssets;
using UnityEngine;

public class ViewBobbingCinemachine : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public Transform playerTransform;       // Referência ao objeto do jogador (o "corpo")
    public Transform cameraTransform;       // Transform da câmera virtual do Cinemachine

    [Header("Bobbing Parameters")]
    public float walkFrequency = 1.5f;      // Frequência do bobbing ao andar
    public float walkAmplitude = 0.05f;     // Altura do bobbing ao andar
    public float sprintFrequency = 2.5f;    // Frequência do bobbing ao correr
    public float sprintAmplitude = 0.1f;    // Altura do bobbing ao correr

    [Header("Dependencies")]
    public StarterAssetsInputs playerInputs; // Script de entrada do Starter Assets

    private Vector3 originalPosition;
    private float distanceTraveled = 0f;     // Distância acumulada para o cálculo do bobbing
    private Vector3 lastPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform não está atribuído!");
            return;
        }

        // Salvar a posição inicial da câmera
        originalPosition = cameraTransform.localPosition;
        lastPosition = playerTransform.position; // Posição inicial do jogador
    }

    private void Update()
    {
        // Verificar movimentação do jogador
        Vector3 currentPosition = playerTransform.position;
        float distanceSinceLastFrame = Vector3.Distance(currentPosition, lastPosition);

        if (distanceSinceLastFrame > 0.01f && playerInputs.move != Vector2.zero)
        {
            // Ajustar bobbing com base no Sprint
            float frequency = playerInputs.sprint ? sprintFrequency : walkFrequency;
            float amplitude = playerInputs.sprint ? sprintAmplitude : walkAmplitude;

            // Calcular deslocamento de bobbing
            float verticalOffset = Mathf.Sin(distanceTraveled * frequency) * amplitude;
            float lateralOffset = Mathf.Cos(distanceTraveled * frequency) * amplitude * 0.5f;

            // Aplicar o bobbing na posição local da câmera
            cameraTransform.localPosition = originalPosition + new Vector3(lateralOffset, verticalOffset, 0);

            // Atualizar distância percorrida
            distanceTraveled += distanceSinceLastFrame;
        }
        else
        {
            // Retornar gradualmente à posição original se o jogador parar
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalPosition, Time.deltaTime * 5f);
        }

        lastPosition = currentPosition; // Atualizar última posição
    }
}
