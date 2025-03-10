using System.Collections;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoEndEvent : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI text;
    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("O vídeo terminou!");
        EventoFinal();
    }

    IEnumerator timeToMenu(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Menu");
    }

    void EventoFinal()
    {
        text.gameObject.SetActive(true);
        StartCoroutine(timeToMenu(5.0f));
        Debug.Log("Evento acionado após o fim do vídeo.");
    }
    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
