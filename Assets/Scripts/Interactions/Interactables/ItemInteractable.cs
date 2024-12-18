using UnityEngine;

public class ItemInteractable : Interactable
{
    [SerializeField][TextArea(3,10)] public string[] itemInteractText;
    [SerializeField] public string itemDescription;
    [SerializeField] public Sprite itemImage;
    
    public override void Interact()
    {
        CanvaManager.instance.EnterItemView(this);
    }
}
