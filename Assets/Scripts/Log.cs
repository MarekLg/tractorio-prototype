using UnityEngine;

public sealed class Log : MonoBehaviour
{
    public bool Sawed { get; private set; } = false;


    public void Saw()
    {
        Sawed = true;
    }
}