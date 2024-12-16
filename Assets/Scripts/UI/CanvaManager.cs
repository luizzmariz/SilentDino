using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StarterAssets;

public class CanvaManager : MonoBehaviour
{
    public static CanvaManager instance = null;

    public GameObject panel;
    public TMP_Text panelText;
    public Image panelImage;

    public FirstPersonController playerController;

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

        panel.SetActive(false);
    }

    public void EnterItemView(ItemInteractable item)
    {
        LockPlayerMovements();

        StartCoroutine(EnterItemViewTransition(item));
    }

    void LockPlayerMovements()
    {
        playerController.DisableDefaultPlayerActions();
    }

    IEnumerator EnterItemViewTransition(ItemInteractable item)
    {
        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ChangePanelContent(item);
        OpenPanel();

        FadeScreen.instance.FadeOutScreen();

        yield return new WaitForSeconds(5); 

        LeaveItemView();
    }

    void ChangePanelContent(ItemInteractable item)
    {
        panelText.text = item.itemDescription;
    }

    void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void LeaveItemView()
    {
        StartCoroutine(LeaveItemViewTransition());
    }

    IEnumerator LeaveItemViewTransition()
    {
        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ClosePanel();
        
        FadeScreen.instance.FadeOutScreen();

        yield return new WaitUntil(() => FadeScreen.instance.fadeOcurring == false);

        UnlockPlayerMovements();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }

    void UnlockPlayerMovements()
    {
        playerController.EnableDefaultPlayerActions();
    }
}
