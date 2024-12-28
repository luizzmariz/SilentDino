using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemInteractable : Interactable
{
    [SerializeField]
    [TextArea(3, 10)]
    public string[] itemInteractText;
    [SerializeField]
    public string itemDescription;
    [SerializeField]
    public Sprite itemImage;


    [Header("Secção de Item para Coleta")]
    [SerializeField] public ItemInfo item;
    [SerializeField] public bool hasItem = false;
    [SerializeField] public InventarioController inventarioController;
    [SerializeField] private bool hasGiven = false;



    [Header("Secção de Item Chave Fechadura")]
    public bool hasCondition = false;
    public string stringConditionName = "";
    public string foundItemText;


    public enum EventTriggerTime
    {
        begginingOfInteraction,
        endingOfInteraction,
        never,
        conditionMet
    }

    public EventTriggerTime whenActivateEvent;
    public UnityEvent interactionEvent;

    public override void Interact()
    {
        if (hasCondition == true && inventarioController.BuscarItem(stringConditionName) == true)
        {
            Debug.Log("ENTERED");
            itemInteractText = itemInteractText.Concat(new string[] { foundItemText }).ToArray();


            if (whenActivateEvent == EventTriggerTime.conditionMet && interactionEvent != null)
            {
                Debug.Log("ENTERED");
                interactionEvent.Invoke();
            }
        }

        else if (whenActivateEvent == EventTriggerTime.begginingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }
        CanvaManager.instance.EnterItemView(this);
    }

    public void InteractionEnd()
    {
        if (whenActivateEvent == EventTriggerTime.endingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
            if (hasItem && !hasGiven)
            {
                hasGiven = true;
                inventarioController.AdicionarItem(item);
            }
        }
    }
}
