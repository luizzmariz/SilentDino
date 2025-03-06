using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Header("Legacy Event (Single Event)")]
    public EventTriggerTime whenActivateEvent;
    public UnityEvent interactionEvent;

    [System.Serializable]
    public class EventEntry
    {
        public EventTriggerTime triggerTime;
        public UnityEvent eventToTrigger;
    }

    [Header("Multiple Events")]
    [SerializeField] private EventEntry[] eventEntries;

    [Header("Escolha do Jogador")]
    [SerializeField] private bool enableChoice = false;
    [SerializeField] private GameObject choiceUI;
    [SerializeField] private Button optionOneButton;
    [SerializeField] private Button optionTwoButton;
    [SerializeField] private UnityEvent optionOneEvent;
    [SerializeField] private UnityEvent optionTwoEvent;
    [SerializeField] private string defaultOptionText = "Escolha uma opção";

    [SerializeField] private bool choiceShown = false;

    public override void Interact()
    {
        bool conditionMet = hasCondition && inventarioController.BuscarItem(stringConditionName);

        if (whenActivateEvent == EventTriggerTime.begginingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }

        TriggerEvents(EventTriggerTime.begginingOfInteraction, conditionMet);
        CanvaManager.instance.EnterItemView(this);
    }

    public void InteractionEnd()
    {
        bool conditionMet = hasCondition && inventarioController.BuscarItem(stringConditionName);

        if (whenActivateEvent == EventTriggerTime.endingOfInteraction && interactionEvent != null)
        {
            interactionEvent.Invoke();
            GiveItemIfNeeded();
        }
        else if (whenActivateEvent == EventTriggerTime.conditionMet && conditionMet && interactionEvent != null)
        {
            interactionEvent.Invoke();
        }

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

    public void ShowChoice()
    {
        if (!enableChoice || choiceShown) return;

        choiceShown = true;
        if (choiceUI != null)
        {
            choiceUI.SetActive(true);

            if (optionOneButton != null && optionTwoButton != null)
            {
                optionOneButton.onClick.RemoveAllListeners();
                optionTwoButton.onClick.RemoveAllListeners();

                optionOneButton.onClick.AddListener(() => SelectOption(true));
                optionTwoButton.onClick.AddListener(() => SelectOption(false));
            }
            else
            {
                Debug.LogWarning("Os botões de escolha não estão atribuídos! Usando o texto padrão: " + defaultOptionText);
            }
        }
    }

    public void RevertChoice()
    {
        choiceShown = false;
    }

    private void SelectOption(bool isOptionOne)
    {
        choiceUI.SetActive(false);

        if (isOptionOne)
        {
            optionOneEvent.Invoke();
        }
        else
        {
            optionTwoEvent.Invoke();
        }
    }
}
