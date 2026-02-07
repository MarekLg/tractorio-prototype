using System;
using UnityEngine;

public sealed class Log : MonoBehaviour
{
    public bool Sawed { get; private set; }
    public bool Grabbed { get; set; }
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Saw()
    {
        Sawed = true;
    }
}