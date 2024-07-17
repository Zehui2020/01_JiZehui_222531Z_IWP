using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        StartCoroutine(IdleRoutine());
        LevelManager.Instance.FadeIn();
    }

    private IEnumerator IdleRoutine()
    {
        float timer = 0;
        int interval = Random.Range(10, 25);

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                animator.SetTrigger("trigger");
                StartCoroutine(IdleRoutine());
                timer = 0;
                yield break;
            }

            yield return null;
        }
    }
}