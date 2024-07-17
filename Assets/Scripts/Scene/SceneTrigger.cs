using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public bool isClicked = false;

    private void Start()
    {
        isClicked = false;
    }

    public void ChangeScene(string nextScene)
    {
        if (!isClicked)
        {
            isClicked = true;
            LevelManager.Instance.LoadScene(nextScene);
        }
    }

    public void EnableCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
