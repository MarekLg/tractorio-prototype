using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public sealed class Sawer: MonoBehaviour
{
    [SerializeField] private float sawSeconds = 5.0f;

    [Header("Inputs")] 
    [SerializeField] private InputAction saw;

    [Header("Events")] 
    [SerializeField] private UnityEvent startSaw;
    [SerializeField] private UnityEvent stopSaw;
    [SerializeField] private UnityEvent<float> updateSawPercent;

    private Log selectedLog;
    private Coroutine sawCoroutine;

    private void OnValidate()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        
        saw.Enable();
    }


    private void Awake()
    {
        saw.performed += _ => sawCoroutine = StartCoroutine(Saw());
        saw.canceled += _ =>
        {
            if (sawCoroutine == null) return;

            Debug.Log("Saw Cancelled");
            
            StopCoroutine(sawCoroutine);
            sawCoroutine = null;
        };
    }


    private void OnTriggerEnter(Collider other)
    {
        var log = other.GetComponentInParent<Log>();
        if (log && !log.Sawed)
        {
            selectedLog = log;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var log = other.GetComponentInParent<Log>();
        if (log == selectedLog)
        {
            selectedLog = null;
        }
    }


    private IEnumerator Saw()
    {
        Debug.Log("Saw Performed");

        if (!selectedLog)
        {
            sawCoroutine = null;
            yield break;
        }
        
        Debug.Log("Saw Started");
        startSaw.Invoke();
        
        var timer = sawSeconds;

        while (timer > 0)
        {
            yield return null;
            
            updateSawPercent.Invoke(1 - timer / sawSeconds);
            
            timer -= Time.deltaTime;
        }
        
        stopSaw.Invoke();
        
        SawLog(selectedLog);
        selectedLog.Saw();
        selectedLog = null;
        
        sawCoroutine = null;
        
        Debug.Log("Saw Completed");
    }

    private static void SawLog(Log log)
    {
        var rigidbody = log.GetComponent<Rigidbody>();
        
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        
        rigidbody.AddForce(Vector3.up * 20, ForceMode.Impulse);
        rigidbody.AddTorque(Vector3.right * 10, ForceMode.Impulse);
        
        // TODO: initiate Grab
    }
}