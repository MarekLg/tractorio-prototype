using System;
using UnityEngine;

public sealed class CameraController: MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothing = 0.2f;

    private Vector3 velocity;

    private void Update()
    {
        var targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
        transform.LookAt(target);
    }
}