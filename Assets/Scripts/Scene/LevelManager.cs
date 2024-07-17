using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private Animator fadeTransition;
    private Coroutine changeSceneRoutine;

    void Awake()
    {
        Instance = this;
    }

    public void FadeIn()
    {
        fadeTransition.SetBool("fade", false);
    }

    public void FadeOut()
    {
        fadeTransition.SetBool("fade", true);
    }

    public void LoadScene(string sceneName)
    {
        if (changeSceneRoutine == null)
            changeSceneRoutine = StartCoroutine(LoadNewScene(sceneName));
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        if (sceneName == "MainMenu")
            yield return new WaitForSeconds(2f);

        AudioManager.Instance.FadeAllSound(false, 0.5f, 0f);
        FadeOut();

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitForSeconds(0.5f);

        changeSceneRoutine = null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}