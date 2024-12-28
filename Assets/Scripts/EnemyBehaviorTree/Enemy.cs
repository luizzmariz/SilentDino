using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class EnemyTrigger : MonoBehaviour
{
    [Header("Event triggered when Player enters")]
    public UnityEvent onPlayerEnter;

    // Refer�ncia para o BehaviorGraphAgent do inimigo
    private BehaviorGraphAgent enemyBehaviorGraph;

    private void Awake()
    {
        // Garante que pegamos o BehaviorGraphAgent anexado ao mesmo GameObject
        enemyBehaviorGraph = GetComponent<BehaviorGraphAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");

            // 1) Marca no Blackboard que encontramos o player
            enemyBehaviorGraph.SetVariableValue("Player", true);

            // 2) Armazena a refer�ncia do Player no Blackboard
            enemyBehaviorGraph.SetVariableValue("PlayerObject", other.gameObject);

            // 3) Invoca qualquer evento adicional (�udio, anima��o etc.)
            onPlayerEnter.Invoke();
            enemyBehaviorGraph.Restart();
            // Importante: *N�o chamamos* Restart() aqui, 
            // deixamos a Behavior Tree lidar com a mudan�a naturalmente
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the trigger area!");

            // 1) Marca no Blackboard que n�o temos mais player no range
            enemyBehaviorGraph.SetVariableValue("Player", false);


            enemyBehaviorGraph.Restart();
            // Novamente, *n�o* chamamos Restart() 
            // para evitar reiniciar a �rvore a todo momento
        }
    }
}
