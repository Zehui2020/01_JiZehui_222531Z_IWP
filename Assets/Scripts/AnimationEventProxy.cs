using UnityEngine;
using UnityEngine.Events;

public class AnimationEventProxy : MonoBehaviour
{
    [System.Serializable]
    public class AnimationEvent : UnityEvent { }

    public AnimationEvent onAnimationEvent;

    // This method will be called by the animation event
    public void OnAnimationEvent()
    {
        onAnimationEvent.Invoke();
    }
}