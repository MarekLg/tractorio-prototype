using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class Grabber : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    
    [Header("Inputs")] 
    [SerializeField] private InputAction grab;

    private Log _grabbedLog;
    private FixedJoint _joint;
    private LogSelector _logSelector;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.TransformPoint(offset), 0.1f);
    }

    private void Awake()
    {
        _logSelector = GetComponentInChildren<LogSelector>();
        
        grab.Enable();
        grab.performed += _ => ToggleGrab();
    }
    

    private Log FindGrabbableLog()
    {
        return _logSelector.Logs.FirstOrDefault(log => log.Sawed && !log.Grabbed);
    }

    private void ToggleGrab()
    {
        if (_grabbedLog)
        {
            ReleaseLog();
            return;
        }

        var log = FindGrabbableLog();
        if (log)
        {
            Grab(log);
        }
    }


    public void Grab(Log log)
    {
        _grabbedLog = log;
        log.Grabbed = true;

        _grabbedLog.transform.position = transform.TransformPoint(offset);
        _grabbedLog.transform.rotation = Quaternion.identity;
        
        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = log.Rigidbody;
    }

    private void ReleaseLog()
    {
        if (!_grabbedLog)
        {
            return;
        }

        Destroy(_joint);
        _joint = null;

        _grabbedLog.Grabbed = false;
        _grabbedLog = null;
    }
}