using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Mutant")]
public class MutantData : EnemyData
{
    public float punchRange;
    public float swipeRange;
    public float jumpAttackRange;

    public int punchesTillSwipe;

    public float roarCooldown;
    public float roarRadius;
    public float roarSpeedModifier;
    public float roarBuffDuration;

    public float jumpAttackCooldown;
    public float jumpAttackCamShakeIntensity;
    public float jumpAttackCamShakeFrequency;
    public float jumpAttackCamShakeDuration;
}
