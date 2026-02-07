using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public sealed class LogSelector : MonoBehaviour
{
    private readonly List<Log> _logs = new ();

    public IReadOnlyList<Log> Logs => _logs;
    
    private void OnValidate()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var log = other.GetComponentInParent<Log>();
        if (log)
        {
            _logs.Add(log);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var log = other.GetComponentInParent<Log>();
        if (log)
        {
            _logs.Remove(log);
        }
    }
}