using UnityEngine;
using FMODUnity;

public class EnemyFootstepsFMOD : MonoBehaviour
{
    [Header("FMOD Footstep Sounds")]
    public EventReference walkClose;   // Som de passos ao andar próximo ao jogador
    public EventReference sprintClose; // Som de passos ao correr próximo ao jogador
    public EventReference walkFar;    // Som de passos ao andar longe do jogador
    public EventReference sprintFar;  // Som de passos ao correr longe do jogador
    public float footstepDistance = 1f; // Distância percorrida necessária para emitir som de passos

    [Header("Player Detection")]
    public Transform player;           // Transform do jogador
    public float closeDistance = 5f;   // Distância para considerar "próximo"
    public float detectionRadius = 10f;// Raio para ajustar estado de corrida/caminhada

    private Vector3 lastPosition;
    private float accumulatedDistance = 0f;
    private bool isPlayerDetected;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }

        lastPosition = transform.position; // Define a posição inicial
    }

    void Update()
    {
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerDetected = distanceToPlayer <= detectionRadius;

        // Calcula a distância percorrida desde a última atualização
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        accumulatedDistance += distanceMoved;

        if (accumulatedDistance >= footstepDistance)
        {
            PlayFootstepSound(distanceToPlayer);
            accumulatedDistance = 0f; // Reseta a distância acumulada
        }

        lastPosition = transform.position; // Atualiza a posição anterior
    }

    private void PlayFootstepSound(float distanceToPlayer)
    {
        // Determina o som com base na proximidade do jogador e no estado do inimigo.
        EventReference selectedSound;

        if (isPlayerDetected)
        {
            selectedSound = distanceToPlayer <= closeDistance ? sprintClose : sprintFar;
        }
        else
        {
            selectedSound = distanceToPlayer <= closeDistance ? walkClose : walkFar;
        }

        RuntimeManager.PlayOneShot(selectedSound, transform.position);
    }
}
