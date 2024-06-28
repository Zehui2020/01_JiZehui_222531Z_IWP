using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void InitInteractable();
    virtual void OnInteract() { }
    virtual void OnEnterRange() { }
    virtual void OnExitRange() { }
    void SetCost(int newCost);
}