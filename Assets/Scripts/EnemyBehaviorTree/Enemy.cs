using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTrigger : MonoBehaviour
{

    [Header("Event triggered when Player enters")]
    public UnityEvent onPlayerEnter;

    public BehaviorGraphAgent enemyBehaviorGraph;

    private void Start()
    {
        enemyBehaviorGraph = GetComponent<BehaviorGraphAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            enemyBehaviorGraph.SetVariableValue("Player", true);
            enemyBehaviorGraph.Restart();
            onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the trigger!");
            enemyBehaviorGraph.SetVariableValue("Player", false);
            enemyBehaviorGraph.Restart();
        }
    }
}
