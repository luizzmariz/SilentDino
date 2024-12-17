using UnityEngine;

public class ItemInteractable : Interactable
{
    [SerializeField] public string itemDescription;
    [SerializeField] public Sprite itemImage;
    
    public override void Interact()
    {
        CanvaManager.instance.EnterItemView(this);
    }
}
