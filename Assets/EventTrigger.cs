using UnityEngine;
using UnityEngine.Events;

public class AreaTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string targetTag = "Player"; // Tag que o objeto precisa ter para ativar o evento

    [Header("Events")]
    public UnityEvent onPlayerEnter; // Evento a ser chamado quando o jogador entrar
    public UnityEvent onPlayerExit;  // Evento a ser chamado quando o jogador sair

    private void OnTriggerEnter(Collider other)
    {
        // Verificar se o objeto que entrou possui a tag especificada
        if (other.CompareTag(targetTag))
        {
            onPlayerEnter?.Invoke(); // Chamar o evento
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verificar se o objeto que saiu possui a tag especificada
        if (other.CompareTag(targetTag))
        {
            onPlayerExit?.Invoke(); // Chamar o evento
        }
    }
}
