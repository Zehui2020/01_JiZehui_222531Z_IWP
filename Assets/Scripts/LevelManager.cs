using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private Animator fadeTransition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeIn()
    {
        //fadeTransition.SetBool("fade", false);
    }

    public void FadeOut()
    {
        //fadeTransition.SetBool("fade", true);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadNewScene(sceneName));
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenuInstantly());
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        if (sceneName == "MainMenu")
            yield return new WaitForSeconds(2f);

        AudioManager.Instance.FadeAllSound(false, 0.5f, 0f);
        FadeOut();

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadMainMenuInstantly()
    {
        AudioManager.Instance.FadeAllSound(false, 0.5f, 0f);
        FadeOut();

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadSceneAsync("MainMenu");
    }
}