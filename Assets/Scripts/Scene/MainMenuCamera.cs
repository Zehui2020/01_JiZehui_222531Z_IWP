using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private Quaternion leftRotation;
    [SerializeField] private Quaternion rightRotation;
    [SerializeField] private float lerpSpeed = 1.0f;

    private bool isLerpingToRight = true;
    private float lerpTime = 0.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        lerpTime += Time.deltaTime * lerpSpeed;

        if (isLerpingToRight)
        {
            transform.rotation = Quaternion.Lerp(leftRotation, rightRotation, lerpTime);
            if (lerpTime >= 1.0f)
            {
                lerpTime = 0.0f;
                isLerpingToRight = false;
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(rightRotation, leftRotation, lerpTime);
            if (lerpTime >= 1.0f)
            {
                lerpTime = 0.0f;
                isLerpingToRight = true;
            }
        }
    }
}