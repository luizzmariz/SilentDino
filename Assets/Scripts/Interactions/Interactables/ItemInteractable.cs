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

    // Backward-compatible fields (keep your existing Inspector setup)
    [Header("Legacy Event (Single Event)")]
    public EventTriggerTime whenActivateEvent;
    public UnityEvent interactionEvent;

    // New system for multiple events
    [System.Serializable]
    public class EventEntry
    {
        public EventTriggerTime triggerTime;
        public UnityEvent eventToTrigger;
    }

    [Header("Multiple Events")]
    [SerializeField] private EventEntry[] eventEntries;

    public override void Interact()
    {
        // Check for condition at the start (e.g., key item)
        bool conditionMet = hasCondition && inventarioController.BuscarItem(stringConditionName);

        // Trigger legacy event at the beginning if configured
        if (whenActivateEvent == EventTriggerTime.begginingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }

        // Trigger new events at the beginning
        TriggerEvents(EventTriggerTime.begginingOfInteraction, conditionMet);

        CanvaManager.instance.EnterItemView(this);
    }

    public void InteractionEnd()
    {
        bool conditionMet = hasCondition && inventarioController.BuscarItem(stringConditionName);

        // Legacy event handling for ending/conditionMet
        if (whenActivateEvent == EventTriggerTime.endingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
            GiveItemIfNeeded();
        }
        else if (whenActivateEvent == EventTriggerTime.conditionMet && conditionMet && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }

        // Trigger new events for ending/conditionMet
        TriggerEvents(EventTriggerTime.endingOfInteraction, conditionMet);
        TriggerEvents(EventTriggerTime.conditionMet, conditionMet);
    }

    private void TriggerEvents(EventTriggerTime triggerTime, bool conditionMet)
    {
        if (eventEntries == null) return;

        foreach (var entry in eventEntries)
        {
            if (entry.triggerTime == triggerTime)
            {
                // For conditionMet, only trigger if the condition is actually met
                if (triggerTime == EventTriggerTime.conditionMet && !conditionMet)
                {
                    continue;
                }
                entry.eventToTrigger?.Invoke();
            }
        }
    }

    private void GiveItemIfNeeded()
    {
        if (hasItem && !hasGiven)
        {
            hasGiven = true;
            inventarioController.AdicionarItem(item);
        }
    }
}