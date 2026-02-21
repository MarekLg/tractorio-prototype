using UnityEngine;
using System.Collections.Generic;

public class Plank : MonoBehaviour
{
    [Header("Einstellungen")]
    [Tooltip("Box Prefab")]
    public GameObject boxPrefab; 
    
    [Tooltip("Min Planks for Box")]
    public int planksNeededForBox = 4;

    [Header("Status")]
    public bool isMaster = true;
    public List<Plank> groupedPlanks = new List<Plank>();

    private bool hasConverted = false;

    void Start()
    {
        if (!groupedPlanks.Contains(this))
        {
            groupedPlanks.Add(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryMerge(collision.collider.GetComponent<Plank>());
    }

    public void TryMerge(Plank otherPlank)
    {
        if (otherPlank == null || otherPlank == this || hasConverted || otherPlank.hasConverted) return;

        if (this.isMaster && otherPlank.isMaster)
        {
            foreach (Plank p in otherPlank.groupedPlanks)
            {
                if (!this.groupedPlanks.Contains(p))
                {
                    this.groupedPlanks.Add(p);
                    p.isMaster = false; 
                }
            }
            
            otherPlank.groupedPlanks.Clear();

            CheckForBoxSpawn();
        }
    }

    private void CheckForBoxSpawn()
    {
        if (groupedPlanks.Count >= planksNeededForBox)
        {
            hasConverted = true;

            if (boxPrefab != null)
            {
                Vector3 centerPos = Vector3.zero;
                for (int i = 0; i < planksNeededForBox; i++)
                {
                    centerPos += groupedPlanks[i].transform.position;
                }
                centerPos /= (float)planksNeededForBox;

                GameObject newBox = Instantiate(boxPrefab, centerPos, Quaternion.identity);
                Box boxScript = newBox.GetComponent<Box>();
                
                if (boxScript != null)
                {
                    boxScript.InitBox(planksNeededForBox);
                }
            }
            else
            {
                Debug.LogWarning("Box Prefab fehlt im Plank Skript!");
            }

            for (int i = 0; i < planksNeededForBox; i++)
            {
                if (groupedPlanks[i] != null)
                {
                    groupedPlanks[i].hasConverted = true;
                    Destroy(groupedPlanks[i].gameObject);
                }
            }
        }
    }
}