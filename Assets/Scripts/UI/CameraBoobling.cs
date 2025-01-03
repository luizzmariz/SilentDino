using StarterAssets;
using UnityEngine;
using FMODUnity; // Importar FMOD Unity Integration
using FMOD.Studio;

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

    [Header("FMOD Footstep Sounds")]
    public EventReference walkSound;        // Som de passos ao andar
    public EventReference sprintSound;      // Som de passos ao correr
    public EventReference walkOnStoneSound; // Som de passos ao andar em pedras
    public EventReference sprintOnStoneSound; // Som de passos ao correr em pedras
    public float footstepInterval = 0.5f;   // Intervalo entre sons de passos

    [Header("Environment Settings")]
    public bool isOnStone; // Determina se o jogador está em pedras

    [Header("Dependencies")]
    public StarterAssetsInputs playerInputs; // Script de entrada do Starter Assets

    private Vector3 originalPosition;
    private float distanceTraveled = 0f;     // Distância acumulada para o cálculo do bobbing
    private Vector3 lastPosition;
    private float footstepTimer;            // Temporizador para controlar o som de passos

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
        footstepTimer = footstepInterval; // Inicializar o temporizador
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
            float lateralOffset = Mathf.Cos(distanceTraveled * frequency * 2f) * amplitude * 0.3f;

            // Aplicar o bobbing na posição local da câmera
            cameraTransform.localPosition = originalPosition + new Vector3(lateralOffset, verticalOffset, 0);

            // Atualizar distância percorrida
            distanceTraveled += distanceSinceLastFrame;

            // Tocar som de passos
            PlayFootstepSound(playerInputs.sprint, isOnStone);
        }

        else
        {
            // Retornar gradualmente à posição original se o jogador parar
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalPosition, Time.deltaTime * 5f);
        }

        lastPosition = currentPosition; // Atualizar última posição
    }

    private void PlayFootstepSound(bool isSprinting, bool isOnStone)
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            // Escolher som com base na superfície e estado do jogador
            EventReference sound;

            if (isOnStone)
            {
                sound = isSprinting ? sprintOnStoneSound : walkOnStoneSound;
            }
            else
            {
                sound = isSprinting ? sprintSound : walkSound;
            }

            // Reproduzir o som
            RuntimeManager.PlayOneShot(sound, playerTransform.position);

            // Reiniciar o temporizador
            footstepTimer = footstepInterval / (isSprinting ? 1.5f : 1f); // Ajustar intervalo para corrida
        }
    }

    public void shiftStone()
    {
        if(isOnStone == true)
        {
            isOnStone = false;
        }
        else
        {
            isOnStone = true;
        }
    }
}
