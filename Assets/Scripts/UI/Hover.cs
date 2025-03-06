using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI buttonText; // Refer�ncia ao TextMeshPro

    public Color hoverTextColor = Color.red; // Cor do texto no hover
    public Color normalTextColor = Color.white; // Cor padr�o do texto
    public bool autoFocusOnAwake = false; // Booleano para pegar foco automaticamente

    void OnEnable()
    {
        if (autoFocusOnAwake)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    void Start()
    {
        // Encontra automaticamente o TextMeshProUGUI filho do bot�o
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("Nenhum componente TextMeshProUGUI encontrado como filho deste bot�o!");
        }
        else
        {
            buttonText.color = normalTextColor; // Define a cor inicial do texto
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverTextColor; // Muda o texto para vermelho no hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = normalTextColor; // Retorna � cor normal do texto
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverTextColor; // Muda o texto para vermelho quando recebe foco
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = normalTextColor; // Retorna � cor normal quando perde foco
        }
    }
}
