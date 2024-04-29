using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; set; }

    private Animator animator;

    private int currentState;
    private float transitionDelay;

    // Idles
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int CrouchIdle = Animator.StringToHash("CrouchIdle");
    // Movement
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int CrouchWalk = Animator.StringToHash("CrouchWalk");
    public readonly int Sprint = Animator.StringToHash("Sprint");
    public readonly int Jump = Animator.StringToHash("Jump");
    public readonly int Falling = Animator.StringToHash("Falling");

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(int state, float transitionDuration, float delayDuration, int layer)
    {
        if (state == currentState || Time.time < transitionDelay)
            return;

        transitionDelay = Time.time + delayDuration;

        animator?.CrossFade(state, transitionDuration, layer);
        currentState = state;
    }

    public AnimationClip GetAnimationClip(int animation)
    {
        foreach (AnimationClip clip in animator?.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == animation)
                return clip;
        }

        return null;
    }
}