using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private CombatCollisionController collisionController;

    private bool canKnife = true;

    public void InitKnifeController()
    {
        collisionController = GetComponent<CombatCollisionController>();
    }

    public bool Knife()
    {
        if (canKnife)
            gameObject.SetActive(true);

        return canKnife;
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(col);
        collisionController.StartDamageCheck(PlayerController.Instance.knifeDamage);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
        collisionController.StopDamageCheck();
    }

    public void ShowCurrentWeapon()
    {
        PlayerController.Instance.ShowCurrentWeapon();
    }

    public void SetCanKnife(int flag)
    {
        if (flag == 0)
            canKnife = false;
        else
        {
            gameObject.SetActive(false);
            canKnife = true;
        }
    }
}