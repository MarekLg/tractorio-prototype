using UnityEngine;
using System.Collections.Generic;

public class StackBridge : MonoBehaviour
{
    public Plank masterPlank;

    private void OnCollisionEnter(Collision collision)
    {
        if (masterPlank != null)
        {
            masterPlank.TryMerge(collision.collider);
        }
    }
}

public class Plank : MonoBehaviour
{
    [Header("Einstellungen")]
    public int maxCapacity = 9;
    public float spacing = 0.005f;

    [Header("Status (Read Only)")]
    public List<Transform> stackParts = new List<Transform>();
    public Transform myAnchor;
    public bool isMaster = true;

    private Rigidbody rb;
    private BoxCollider myCollider;
    private Vector3 singlePlankSize;

    void Start()
    {
        CalculateDimensions();
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<BoxCollider>();

        if (stackParts.Count == 0)
        {
            stackParts.Add(this.transform);
        }
    }

    private void CalculateDimensions()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();
        
        if (myCollider != null)
        {
            singlePlankSize = Vector3.Scale(myCollider.size, transform.localScale);
        }
        else
        {
            var rend = GetComponentInChildren<Renderer>();
            if (rend) singlePlankSize = rend.bounds.size;
        }

        float[] s = { singlePlankSize.x, singlePlankSize.y, singlePlankSize.z };
        System.Array.Sort(s);
        singlePlankSize.y = s[0];
        singlePlankSize.x = s[1];
        singlePlankSize.z = s[2];
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryMerge(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryMerge(other);
    }

    public void TryMerge(Collider other)
    {
        if (!isMaster) return;

        Plank otherPlank = other.GetComponentInParent<Plank>();

        if (otherPlank == null)
        {
            StackBridge bridge = other.GetComponentInParent<StackBridge>();
            if (bridge != null) otherPlank = bridge.masterPlank;
        }

        if (otherPlank != null && otherPlank != this && otherPlank.isMaster)
        {
            if (this.stackParts.Count >= otherPlank.stackParts.Count && 
                (this.stackParts.Count + otherPlank.stackParts.Count <= maxCapacity))
            {
                MergeLogic(otherPlank);
            }
        }
    }

    public void MergeLogic(Plank otherMaster)
    {
        if (myAnchor == null)
        {
            CreateAnchor();
        }

        List<Transform> incomingParts = new List<Transform>(otherMaster.stackParts);
        
        if (otherMaster.myAnchor != null) Destroy(otherMaster.myAnchor.gameObject);

        foreach (Transform part in incomingParts)
        {
            if (part == null) continue;

            Plank script = part.GetComponent<Plank>();
            if (script != null)
            {
                script.isMaster = false;
                script.rb = null;
            }

            Destroy(part.GetComponent<Rigidbody>());
            
            Collider c = part.GetComponent<Collider>();
            if (c) c.enabled = false;

            part.SetParent(myAnchor);
            
            part.localScale = this.transform.localScale; 
            part.localRotation = Quaternion.identity;

            if (!stackParts.Contains(part)) stackParts.Add(part);
        }

        otherMaster.stackParts.Clear();

        ArrangeStack();
        UpdateAnchorHitbox();
    }

    private void CreateAnchor()
    {
        GameObject anchorGO = new GameObject("Stack_Anchor_" + name);
        anchorGO.transform.position = this.transform.position;
        anchorGO.transform.rotation = this.transform.rotation;
        anchorGO.transform.localScale = Vector3.one;

        myAnchor = anchorGO.transform;

        if (rb) Destroy(rb);
        if (myCollider) myCollider.enabled = false;

        this.transform.SetParent(myAnchor);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;

        Rigidbody anchorRb = anchorGO.AddComponent<Rigidbody>();
        anchorRb.mass = 10f;

        BoxCollider anchorCol = anchorGO.AddComponent<BoxCollider>();
        
        StackBridge bridge = anchorGO.AddComponent<StackBridge>();
        bridge.masterPlank = this;
    }

    private void ArrangeStack()
    {
        float w = singlePlankSize.x + spacing;
        float h = singlePlankSize.y;

        for (int i = 0; i < stackParts.Count; i++)
        {
            Transform part = stackParts[i];

            int layer = i / 3;     
            int posInLayer = i % 3; 

            float yPos = layer * h;
            
            float linearOffset = posInLayer * -w;

            Vector3 targetPos;
            Quaternion targetRot;

            if (layer % 2 == 0) 
            {
                targetPos = new Vector3(linearOffset, yPos, 0);
                targetRot = Quaternion.identity;
            }
            else 
                {
                    float centerShift = -w; 

                float xPos = centerShift;

                float zPos = w + linearOffset; 

                targetPos = new Vector3(xPos, yPos, zPos);
                targetRot = Quaternion.Euler(0, 90, 0);
            }

            part.localPosition = targetPos;
            part.localRotation = targetRot;
        }
    }

    private void UpdateAnchorHitbox()
    {
        if (myAnchor == null) return;

        BoxCollider anchorCol = myAnchor.GetComponent<BoxCollider>();
        Rigidbody anchorRb = myAnchor.GetComponent<Rigidbody>();

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;

        foreach (Transform child in stackParts)
        {
            if (child == null) continue;

            Vector3 center = child.localPosition;
            
            bool isRotated = (Mathf.Abs(child.localEulerAngles.y - 90) < 1f);
            
            Vector3 size = isRotated 
                ? new Vector3(singlePlankSize.z, singlePlankSize.y, singlePlankSize.x) 
                : singlePlankSize;

            Bounds childBounds = new Bounds(center, size);

            if (!hasBounds)
            {
                bounds = childBounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(childBounds);
            }
        }

        if (hasBounds)
        {
            anchorCol.center = bounds.center;
            anchorCol.size = bounds.size;
        }

        if (anchorRb) anchorRb.mass = stackParts.Count * 2f;
    }
}