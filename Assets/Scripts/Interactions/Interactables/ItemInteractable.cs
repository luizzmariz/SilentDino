using UnityEngine;
using UnityEngine.Events;

public class ItemInteractable : Interactable
{
    [SerializeField][TextArea(3,10)] public string[] itemInteractText;
    [SerializeField] public string itemDescription;
    [SerializeField] public Sprite itemImage;

    public enum EventTriggerTime
    {
        begginingOfInteraction,
        endingOfInteraction,
        never
    }

    public EventTriggerTime whenActivateEvent;
    public UnityEvent interactionEvent;
    
    public override void Interact()
    {
        if(whenActivateEvent == EventTriggerTime.begginingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }
        CanvaManager.instance.EnterItemView(this);
    }

    public void InteractionEnd()
    {
        if(whenActivateEvent == EventTriggerTime.endingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }
    }
}
