using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeEffect : MonoBehaviour
{
    private GameObject gameObjectToFadeIn;
    private GameObject gameObjectToFadeOut;

    // Start is called before the first frame update
    public void Fade(GameObject gameObjectToFadeIn, GameObject gameObjectToFadeOut)
    {
        if (this.gameObjectToFadeIn != null)
        {
            this.gameObjectToFadeIn = gameObjectToFadeIn;
        }
        if (this.gameObjectToFadeOut != null)
        {
            this.gameObjectToFadeOut = gameObjectToFadeOut;
        }

        StartCoroutine(Fade(gameObjectToFadeIn, 0, 1));
        StartCoroutine(Fade(gameObjectToFadeOut, 1, 0));
    }


    public void FadeIn(GameObject gameObjectToFadeIn, float fadeTime = 0.1f, string scene="", float fadeDelay = 0f)
    {
        this.gameObjectToFadeIn = gameObjectToFadeIn;

        StartCoroutine(Fade(gameObjectToFadeIn, 0, 1, fadeTime, scene, fadeDelay));
    }

    public void FadeOut(GameObject gameObjectToFadeOut, float fadeTime = 0.1f)
    {
        this.gameObjectToFadeOut = gameObjectToFadeOut;

        StartCoroutine(Fade(gameObjectToFadeOut, 1, 0, fadeTime));
    }

    private IEnumerator Fade(GameObject gameObjectToFade, float startA, float endA, float fadeTime = 0.1f, string scene="", float fadeDelay = 0f)
    {
        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        if (startA == 0)
        {
            gameObjectToFade.SetActive(true);
        }

        float time = 0f;

        while (time < fadeTime) // default to 0.1 but can specify otherwise (to 1 for scene transitions)
        {
            time += Time.deltaTime;
            float a = Mathf.Lerp(startA, endA, time / fadeTime);
            gameObjectToFade.GetComponent<CanvasGroup>().alpha = a;
            yield return null;
        }

        gameObjectToFade.GetComponent<CanvasGroup>().alpha = endA;

        
        if (endA == 0)
        {
            if (gameObjectToFade.GetComponent<Image>() != null && !gameObjectToFade.name.Contains("DialogueSystemManager"))
            {
                gameObjectToFade.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Temporary/None");
            }
            gameObjectToFade.SetActive(false);
        }

        if (!scene.Equals(""))
        {
            SceneManager.LoadScene(scene);
        }
    }
}
