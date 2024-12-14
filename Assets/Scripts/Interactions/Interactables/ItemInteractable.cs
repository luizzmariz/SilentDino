using UnityEngine;

public class ItemInteractable : Interactable
{
    [SerializeField] public string itemDescription;
    
    public override void Interact()
    {
        Debug.Log(itemDescription);

        Destroy(gameObject);
    }
}
