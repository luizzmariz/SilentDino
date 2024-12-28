using UnityEngine;

public class FreezeScale : MonoBehaviour
{
    public Vector3 escalaFixa = Vector3.one;

    void LateUpdate()
    {
        transform.localScale = escalaFixa;
    }
}
