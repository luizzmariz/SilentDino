using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Refer�ncia ao objeto que ser� o alvo
    public Transform target;

    void Update()
    {
        // Verifica se o alvo foi definido
        if (target != null)
        {
            // Faz o objeto olhar para o alvo
            transform.LookAt(target);

            // Opcional: Alinhar apenas no eixo Y (evita inclina��es no X/Z)
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            Debug.LogWarning("Target n�o definido para " + gameObject.name);
        }
    }
}
