using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using FMODUnity;

public class SceneChanger : MonoBehaviour
{
    public GameObject introPanel;
    public GameObject FadePanel;

    public EventReference introTheme;

    [SerializeField] InputAction Submit;

    private bool inTransition;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        introPanel.SetActive(false);
        FadePanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName == "Stage1")
        {
            FadePanel.SetActive(true);
            //StartCoroutine(EnterCutsceneTransition());
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator EnterCutsceneTransition()
    {
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        ActivateIntroCutscenePanel();

        FadeScreen.instance.FadeOutScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == false);

        inTransition = false;

        // Inicia a animação dos quadrinhos
        GetComponent<ComicAnimator>().StartAnimation();
    }

    private void ActivateIntroCutscenePanel()
    {
        introPanel.SetActive(true);
        RuntimeManager.PlayOneShot(introTheme);
        Submit.Enable();
        Submit.performed += SkipIntro;
    }

    private void SkipIntro(InputAction.CallbackContext context)
    {
        if (!inTransition)
        {
            StopAllCoroutines();
            StartCoroutine(LeaveCutsceneTransition());
        }
    }

    public IEnumerator LeaveCutsceneTransition()
    {
        Submit.Disable();
        Submit.performed -= SkipIntro;
        inTransition = true;

        FadeScreen.instance.FadeInScreen();

        yield return new WaitUntil(() => FadeScreen.instance.screenIsFaded == true);

        inTransition = false;

        SceneManager.LoadScene("Stage1");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
