using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public enum TeleportType
    {
        Indoor,
        Outdoor
    }
    public TeleportType teleportType;

    [SerializeField] private Transform teleportLocation;

    public void Teleport()
    {
        PlayerController.Instance.transform.position = teleportLocation.position;
        PlayerController.Instance.transform.forward = teleportLocation.forward;

        switch (teleportType) 
        {
            case TeleportType.Indoor:
                AudioManager.Instance.Stop(Sound.SoundName.OutdoorAmbience);
                AudioManager.Instance.Play(Sound.SoundName.IndoorAmbience);
                break;
            case TeleportType.Outdoor:
                AudioManager.Instance.Play(Sound.SoundName.OutdoorAmbience);
                AudioManager.Instance.Stop(Sound.SoundName.IndoorAmbience);
                break;
        }
    }
}