using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Box Einstellungen")]
    public int maxPlankCapacity = 20;
    
    [Header("Currently Holding")]
    public int currentPlankCount = 0;

    public void InitBox(int startAmount)
    {
        currentPlankCount = startAmount;
        UpdateVisuals();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Plank incomingPlank = collision.collider.GetComponent<Plank>();
        
        if (incomingPlank != null && currentPlankCount < maxPlankCapacity)
        {
            currentPlankCount++;
            UpdateVisuals();
            
            Destroy(incomingPlank.gameObject);
        }
    }

    private void UpdateVisuals()
    {
        Debug.Log($"Box enthält jetzt: {currentPlankCount} / {maxPlankCapacity} Planks.");
    }
}