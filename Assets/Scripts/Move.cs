using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveOnAwake : MonoBehaviour
{
    public Transform targetPosition;
    public float moveDuration = 1f;
    public UnityEvent onMoveComplete;

    private void Awake()
    {
        if (targetPosition != null)
        {
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetPosition.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        onMoveComplete?.Invoke();
    }
}
