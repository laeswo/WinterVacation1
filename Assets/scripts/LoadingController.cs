using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image progress;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    private void Start()
    {
        StartCoroutine(SceneProcess());
    }

    IEnumerator SceneProcess()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(nextScene);
        load.allowSceneActivation = false;

        float timer = 0f;
        while (!load.isDone)
        {
            yield return null;

            if (load.progress < 0.9f)
            {
                progress.fillAmount = load.progress;
            } else
            {
                timer += Time.unscaledDeltaTime;
                progress.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progress.fillAmount >= 1f)
                {
                    load.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
