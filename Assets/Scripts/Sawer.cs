using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class Sawer: MonoBehaviour
{
    [SerializeField] private float sawSeconds = 5.0f;

    [Header("Inputs")] 
    [SerializeField] private InputAction saw;

    [Header("Events")] 
    [SerializeField] private UnityEvent startSaw;
    [SerializeField] private UnityEvent stopSaw;
    [SerializeField] private UnityEvent<float> updateSawPercent;

    private LogSelector _logSelector;
    private Coroutine _sawCoroutine;
    private Grabber _grabber;


    private void Awake()
    {
        _logSelector = GetComponentInChildren<LogSelector>();
        _grabber = GetComponent<Grabber>();
        
        saw.Enable();
        saw.performed += _ => _sawCoroutine = StartCoroutine(Saw());
        saw.canceled += _ =>
        {
            if (_sawCoroutine == null) return;

            Debug.Log("Saw Cancelled");
            
            StopCoroutine(_sawCoroutine);
            _sawCoroutine = null;
        };
    }


    private Log FindSawableLog()
    {
        return _logSelector.Logs.FirstOrDefault(log => !log.Sawed);
    }

    private IEnumerator Saw()
    {
        Debug.Log("Saw Performed");

        var log = FindSawableLog();
        if (!log)
        {
            _sawCoroutine = null;
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
        
        SawLog(log);
        
        _sawCoroutine = null;
        
        Debug.Log("Saw Completed");
    }

    private void SawLog(Log log)
    {
        log.Rigidbody.useGravity = true;
        log.Rigidbody.isKinematic = false;

        log.Rigidbody.AddForce(Vector3.up * 20, ForceMode.Impulse);
        log.Rigidbody.AddTorque(Vector3.right * 10, ForceMode.Impulse);
        
        log.Saw();

        if (_grabber)
        {
            _grabber.Grab(log);
        }
    }
}