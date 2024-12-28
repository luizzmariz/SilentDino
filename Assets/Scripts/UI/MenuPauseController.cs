using UnityEngine;

public class MenuPauseController : MonoBehaviour
{
    public CanvasGroup blackBGCanvasGroup; // Fundo preto
    public CanvasGroup menuCanvasGroup;    // Texto/UI do menu
    public float fadeInSpeed = 4f;   // Velocidade para aparecer
    public float fadeOutSpeed = 4f;  // Velocidade para sumir

    private bool isMenuOpen = false;
    private float targetAlphaBlack = 0f; // Alpha alvo do fundo preto
    private float targetAlphaMenu = 0f;  // Alpha alvo do menu

    private enum FadeStage
    {
        None,
        FadeInBlack,
        FadeInMenu,
        FadeOutMenu,
        FadeOutBlack
    }

    private FadeStage currentStage = FadeStage.None;

    void Start()
    {
        Time.timeScale = 1;
        // Menu e fundo começam invisíveis
        blackBGCanvasGroup.alpha = 0f;
        blackBGCanvasGroup.interactable = false;
        blackBGCanvasGroup.blocksRaycasts = false;

        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isMenuOpen && currentStage == FadeStage.None)
            {
                AbrirMenu();
            }
            else if (isMenuOpen && currentStage == FadeStage.None)
            {
                FecharMenu();
            }
        }

        AtualizarFade();
    }

    private void AtualizarFade()
    {
        switch (currentStage)
        {
            case FadeStage.FadeInBlack:
                blackBGCanvasGroup.alpha = Mathf.Lerp(blackBGCanvasGroup.alpha, targetAlphaBlack, Time.unscaledDeltaTime * fadeInSpeed);
                if (Mathf.Abs(blackBGCanvasGroup.alpha - targetAlphaBlack) < 0.01f)
                {
                    blackBGCanvasGroup.alpha = targetAlphaBlack;
                    // Fundo preto totalmente visível, agora fade in do menu
                    currentStage = FadeStage.FadeInMenu;
                    targetAlphaMenu = 1f;
                }
                break;

            case FadeStage.FadeInMenu:
                menuCanvasGroup.alpha = Mathf.Lerp(menuCanvasGroup.alpha, targetAlphaMenu, Time.unscaledDeltaTime * fadeInSpeed);
                if (Mathf.Abs(menuCanvasGroup.alpha - targetAlphaMenu) < 0.01f)
                {
                    menuCanvasGroup.alpha = targetAlphaMenu;
                    // Menu totalmente visível
                    menuCanvasGroup.interactable = true;
                    menuCanvasGroup.blocksRaycasts = true;
                    // Aqui pausamos o jogo ao terminar todo o fade in
                    Time.timeScale = 0;
                    isMenuOpen = true;
                    currentStage = FadeStage.None;
                }
                break;

            case FadeStage.FadeOutMenu:
                menuCanvasGroup.alpha = Mathf.Lerp(menuCanvasGroup.alpha, targetAlphaMenu, Time.unscaledDeltaTime * fadeOutSpeed);
                if (Mathf.Abs(menuCanvasGroup.alpha - targetAlphaMenu) < 0.01f)
                {
                    menuCanvasGroup.alpha = targetAlphaMenu;
                    menuCanvasGroup.interactable = false;
                    menuCanvasGroup.blocksRaycasts = false;
                    // Menu sumiu, agora fade out do preto
                    currentStage = FadeStage.FadeOutBlack;
                    targetAlphaBlack = 0f;
                }
                break;

            case FadeStage.FadeOutBlack:
                blackBGCanvasGroup.alpha = Mathf.Lerp(blackBGCanvasGroup.alpha, targetAlphaBlack, Time.unscaledDeltaTime * fadeOutSpeed);
                if (Mathf.Abs(blackBGCanvasGroup.alpha - targetAlphaBlack) < 0.01f)
                {
                    blackBGCanvasGroup.alpha = targetAlphaBlack;
                    blackBGCanvasGroup.interactable = false;
                    blackBGCanvasGroup.blocksRaycasts = false;
                    // Fundo sumiu, despausa o jogo
                    Time.timeScale = 1;
                    isMenuOpen = false;
                    currentStage = FadeStage.None;
                }
                break;
        }
    }

    private void AbrirMenu()
    {
        // Primeiro: Fade In do fundo preto
        currentStage = FadeStage.FadeInBlack;
        targetAlphaBlack = 1f;
        blackBGCanvasGroup.interactable = true; // Pode não ser necessário, mas pode deixar
        blackBGCanvasGroup.blocksRaycasts = true;
    }

    private void FecharMenu()
    {
        // Primeiro: Fade Out do menu (texto/UI)
        currentStage = FadeStage.FadeOutMenu;
        targetAlphaMenu = 0f;
        // Após sumir o menu, some o preto e retome o jogo
    }
}
