using System.Collections;
using UnityEngine;

public class ComicAnimator : MonoBehaviour
{
    public RectTransform comicPage; // A p�gina completa dos quadrinhos (um �nico RectTransform)
    public Vector3[] positions; // Posi��es-alvo para movimentar a p�gina
    public Vector3[] scales; // Escalas-alvo para fazer o zoom nos quadrinhos
    public float[] waitTimes; // Tempo para transi��o entre as posi��es
    public float moveSpeed = 2.0f; // N�o utilizado diretamente, mas mantido para controle geral se necess�rio

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
            // Move e ajusta o zoom na p�gina para a posi��o e escala desejadas
            yield return StartCoroutine(MoveAndZoom(positions[i], scales[i], waitTimes[i]));
        }

        isAnimating = false;

        // Caso precise notificar outro script (como `SceneChanger`), voc� pode chamar aqui.
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

            // Suaviza o movimento da posi��o e o ajuste de escala baseado no tempo
            float t = elapsedTime / duration;
            comicPage.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            comicPage.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // Garante que a posi��o e a escala finais sejam exatamente as definidas
        comicPage.anchoredPosition = targetPosition;
        comicPage.localScale = targetScale;
    }
}
