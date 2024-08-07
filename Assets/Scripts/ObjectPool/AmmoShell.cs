using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using static Sound;

public class AmmoShell : PooledObject
{
    private Rigidbody shellRB;
    private AudioSource audioSource;
    [SerializeField] private Sound.SoundName ammoShellSound;
    [SerializeField] private LayerMask groundLayer;
    private bool isAudioPlayed = false;

    public override void Init()
    {
        shellRB = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetupShell(Vector3 spawnPos, float ejectForce, float ejectUpwardForce, Vector3 torque, float lifetime)
    {
        transform.position = spawnPos;
        transform.forward = PlayerController.Instance.transform.forward;
        gameObject.SetActive(true);

        shellRB.AddForce(transform.up * ejectForce + transform.right * ejectUpwardForce, ForceMode.Impulse);
        shellRB.velocity = PlayerController.Instance.GetVelocity();
        shellRB.AddTorque(torque, ForceMode.Impulse);

        Deactivate(lifetime);
    }

    public void Deactivate(float lifetime)
    {
        StartCoroutine(DeactivateRoutine(lifetime));
    }

    private IEnumerator DeactivateRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Release();
        isAudioPlayed = false;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utility.Instance.CheckLayer(collision.gameObject, groundLayer))
            return;

        if (isAudioPlayed)
            return;

        AudioManager.Instance.PlayOneShot(audioSource, ammoShellSound);
        isAudioPlayed = true;
    }
}