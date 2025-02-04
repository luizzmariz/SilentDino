using System.Collections;
using UnityEngine;

public class ComicAnimator : MonoBehaviour
{
    public RectTransform comicPage; // A página completa dos quadrinhos (um único RectTransform)
    public Vector3[] positions; // Posições-alvo para movimentar a página
    public Vector3[] scales; // Escalas-alvo para fazer o zoom nos quadrinhos
    public float[] waitTimes; // Tempo para transição entre as posições
    public float moveSpeed = 2.0f; // Não utilizado diretamente, mas mantido para controle geral se necessário

    private bool isAnimating = false;

    public void StartAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            StartCoroutine(AnimateComic());
        }
    }

    private IEnumerator AnimateComic()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            // Move e ajusta o zoom na página para a posição e escala desejadas
            yield return StartCoroutine(MoveAndZoom(positions[i], scales[i], waitTimes[i]));
        }

        isAnimating = false;

        // Caso precise notificar outro script (como `SceneChanger`), você pode chamar aqui.
        Debug.Log("Comic animation finished.");
    }

    private IEnumerator MoveAndZoom(Vector3 targetPosition, Vector3 targetScale, float duration)
    {
        Vector3 startPosition = comicPage.anchoredPosition;
        Vector3 startScale = comicPage.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Suaviza o movimento da posição e o ajuste de escala baseado no tempo
            float t = elapsedTime / duration;
            comicPage.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            comicPage.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // Garante que a posição e a escala finais sejam exatamente as definidas
        comicPage.anchoredPosition = targetPosition;
        comicPage.localScale = targetScale;
    }
}
