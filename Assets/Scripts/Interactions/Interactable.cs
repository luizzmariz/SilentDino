using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] public string interactableName;

    public virtual void Interact(){}
}
