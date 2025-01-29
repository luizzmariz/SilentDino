using System.Collections;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.UI;
using System.Linq;

public class CanvaManager : MonoBehaviour
{
    public static CanvaManager instance = null;

    public GameObject itemPanel;
    public TMP_Text panelText;
    int panelTextIndex;
    public Image panelImage;
    bool inTransition;
    ItemInteractable itemInView;

    public FirstPersonController playerController;
    public UIController _UIController;


    void Start()
    {
        // Define o cursor como confinado dentro da janela
        Cursor.lockState = CursorLockMode.Locked;

        // Torna o cursor vis�vel (se quiser esconder, use Cursor.visible = false)
        Cursor.visible = false;
    }

    void Awake()
    {
        if(instance == null) 
        {
			instance = this;
		} 
        else if(instance != this) 
        {
			Destroy(gameObject);
		}

        itemPanel.SetActive(false);
        _UIController = GetComponent<UIController>();
        inTransition = false;
    }

    public void EnterItemView(ItemInteractable item)
    {
        DisablePlayerMovementsController();

        StartCoroutine(EnterItemViewTransition(item));
    }

    void DisablePlayerMovementsController()
    {
        playerController.DisableDefaultPlayerActions();
    }

    IEnumerator EnterItemViewTransition(ItemInteractable item)
    {
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ChangePanelContent(item);
        OpenPanel();
        EnablePlayerUIController();

        FadeScreen.instance.FadeOutScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == false);

        inTransition = false;
    }

    void ChangePanelContent(ItemInteractable item)
    {
        itemInView = item;
        if(item.itemInteractText.Count() > 0)
        {
            panelText.text = item.itemInteractText[0];
            panelTextIndex = 0;
        }
        panelImage.sprite = item.itemImage;
    }

    void OpenPanel()
    {
        itemPanel.SetActive(true);
    }

    void EnablePlayerUIController()
    {
        _UIController.EnableUIActions();
    }

    public void LeaveItemView()
    {
        DisablePlayerUIController();

        StartCoroutine(LeaveItemViewTransition());
    }

    void DisablePlayerUIController()
    {
        _UIController.DisableUIActions();
    }

    IEnumerator LeaveItemViewTransition()
    {
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ClosePanel();
        itemInView.InteractionEnd();
        
        FadeScreen.instance.FadeOutScreen();

        yield return new WaitUntil(() => FadeScreen.instance.fadeOcurring == false);

        EnablePlayerMovementsController();

        inTransition = false;
    }

    void ClosePanel()
    {
        itemPanel.SetActive(false);
    }

    void EnablePlayerMovementsController()
    {
        playerController.EnableDefaultPlayerActions();
    }

    public void Navigate(Vector2 navigations)
    {
        if(!inTransition)
        {
            if(itemPanel.activeSelf)
            {
                if(navigations == Vector2.left)
                {
                    if(panelTextIndex-1 >= 0)
                    {
                        panelTextIndex--;
                        panelText.text = itemInView.itemInteractText[panelTextIndex];
                    }
                }
                else if(navigations == Vector2.right)
                {
                    if(panelTextIndex+1 < itemInView.itemInteractText.Count())
                    {
                        panelTextIndex++;
                        panelText.text = itemInView.itemInteractText[panelTextIndex];
                    }
                }
            }
        }
        
    }

    public void Submit()
    {
        if(!inTransition)
        {
            if(itemPanel.activeSelf)
            {
                if(panelTextIndex == itemInView.itemInteractText.Count()-1)
                {
                    LeaveItemView();
                }
                else
                {
                    panelTextIndex++;
                    panelText.text = itemInView.itemInteractText[panelTextIndex];
                }
            }
        }
    }

    public void Cancel()
    {
        if(!inTransition)
        {
            if(itemPanel.activeSelf)
            {
                LeaveItemView();
            }
        }
    }
}
