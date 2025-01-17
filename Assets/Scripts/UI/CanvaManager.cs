using System.Collections;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.UI;
using System.Linq;

public class CanvaManager : MonoBehaviour
{
    public static CanvaManager instance = null;

    [Header("UI Elements")]
    public GameObject itemPanel;
    public TMP_Text panelText;
    public Image panelImage;

    [Header("Player Controllers")]
    public FirstPersonController playerController;
    public UIController uiController;

    private int panelTextIndex;
    private bool inTransition;
    private ItemInteractable itemInView;

    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        itemPanel.SetActive(false);
        inTransition = false;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnterItemView(ItemInteractable item)
    {
        DisablePlayerControls();
        StartCoroutine(EnterItemViewRoutine(item));
    }

    private IEnumerator EnterItemViewRoutine(ItemInteractable item)
    {
        inTransition = true;

        // Trigger Fade In Animation
        FadeScreen.instance.FadeInScreen();
        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded);

        // Pause the game after Fade In
        Time.timeScale = 0;
        Debug.Log("Jogo pausado.");

        UpdatePanelContent(item);
        itemPanel.SetActive(true);
        EnableUIControls();

        // Trigger Fade Out Animation
        FadeScreen.instance.FadeOutScreen();
        yield return new WaitUntil(() => !FadeScreen.instance.screenIsFaded);

        inTransition = false;
    }

    public void LeaveItemView()
    {
        if (!inTransition)
        {
            DisableUIControls();
            StartCoroutine(LeaveItemViewRoutine());
        }
    }

    private IEnumerator LeaveItemViewRoutine()
    {
        inTransition = true;

        // Trigger Fade In Animation
        FadeScreen.instance.FadeInScreen();
        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded);

        // Close Panel and Resume Game
        itemPanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Jogo retomado.");

        itemInView?.InteractionEnd();
        EnablePlayerControls();

        // Trigger Fade Out Animation
        FadeScreen.instance.FadeOutScreen();
        yield return new WaitUntil(() => !FadeScreen.instance.screenIsFaded);

        inTransition = false;
    }

    private void UpdatePanelContent(ItemInteractable item)
    {
        itemInView = item;

        if (item.itemInteractText != null && item.itemInteractText.Any())
        {
            panelText.text = item.itemInteractText[0];
            panelTextIndex = 0;
        }
        else
        {
            panelText.text = string.Empty;
        }

        panelImage.sprite = item.itemImage;
    }

    private void DisablePlayerControls()
    {
        playerController.DisableDefaultPlayerActions();
    }

    private void EnablePlayerControls()
    {
        playerController.EnableDefaultPlayerActions();
    }

    private void EnableUIControls()
    {
        uiController.EnableUIActions();
    }

    private void DisableUIControls()
    {
        uiController.DisableUIActions();
    }

    public void Navigate(Vector2 navigation)
    {
        if (!inTransition && itemPanel.activeSelf)
        {
            if (navigation == Vector2.left && panelTextIndex > 0)
            {
                panelTextIndex--;
                panelText.text = itemInView.itemInteractText[panelTextIndex];
            }
            else if (navigation == Vector2.right && panelTextIndex < itemInView.itemInteractText.Count() - 1)
            {
                panelTextIndex++;
                panelText.text = itemInView.itemInteractText[panelTextIndex];
            }
        }
    }

    public void Submit()
    {
        if (!inTransition && itemPanel.activeSelf)
        {
            if (panelTextIndex == itemInView.itemInteractText.Count() - 1)
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

    public void Cancel()
    {
        if (!inTransition && itemPanel.activeSelf)
        {
            LeaveItemView();
        }
    }
}
