using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    bool inTransition;

    public GameObject introPanel;
    public GameObject introSequence;

    public GameObject blackScreen;
    public GameObject fadePanel;

    [SerializeField] InputAction Submit;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Menu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        introPanel.SetActive(false);
        fadePanel.SetActive(false);
    }
    // Fun��o para carregar uma cena pelo nome
    public void LoadScene(string sceneName)
    {
        if(sceneName == "Stage1")
        {
            fadePanel.SetActive(true);
            StartCoroutine(EnterCutsceneTransition());

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    IEnumerator EnterCutsceneTransition()
    {
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ActivateIntroCutscenePanel();

        FadeScreen.instance.FadeOutScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == false);

        fadePanel.SetActive(false);
        inTransition = false;

        StartCoroutine(StartIntroAnimation());
    }

    void ActivateIntroCutscenePanel()
    {
        introPanel.SetActive(true);

        Submit.Enable();
        Submit.performed += context => SkipIntro(context);
    }

    IEnumerator StartIntroAnimation()
    {
        float Ycord = introSequence.GetComponent<RectTransform>().anchoredPosition.y;

        while(Ycord < 990f)
        {
            Ycord+=0.25f;
            introSequence.GetComponent<RectTransform>().anchoredPosition = new Vector3(introSequence.GetComponent<RectTransform>().anchoredPosition.x, Ycord);

            yield return new WaitForNextFrameUnit();
        }

        if(!inTransition)
        {
            StartCoroutine(LeaveCutsceneTransition());
        }
    }

    // Fun��o para sair pular a introdução
    void SkipIntro(InputAction.CallbackContext context)
    {
        if(!inTransition)
        {
            StopAllCoroutines();

            StartCoroutine(LeaveCutsceneTransition());
        }        
    }

    // Fun��o para sair da cutscene de introdução
    IEnumerator LeaveCutsceneTransition()
    {
        fadePanel.SetActive(true);

        Submit.Disable();
        Submit.performed -= context => SkipIntro(context);
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        fadePanel.SetActive(false);

        blackScreen.SetActive(true);

        inTransition = false;


        SceneManager.LoadScene("Stage1");
    }

    // Fun��o para sair do jogo
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
