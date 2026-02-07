using UnityEngine;
using System.Collections;

public class Sawmill : MonoBehaviour
{
    [Header("Sägeblatt Einstellungen")]
    public Transform[] blades;
    public float maxRotationSpeed = 1000f;
    public float rampUpDuration = 2.0f;
    public float rampDownDuration = 3.0f;
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Setup")]
    public string treeTag = "Tree";
    public GameObject plankPrefab;
    public Transform outputPoint;
    
    public Transform logStartPoint; 
    public Transform logEndPoint; 
    
    [Header("Produktion")]
    public int planksPerLog = 5;
    public float timeBetweenPlanks = 0.5f;

    private float currentSpeed = 0f;
    private bool isProcessing = false;

    void Update()
    {
        if (currentSpeed > 0.1f)
        {
            foreach (Transform blade in blades)
            {
                if (blade != null)
                    blade.Rotate(rotationAxis * currentSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag(treeTag))
        {
            StartCoroutine(ProcessLogRoutine(other.gameObject));
        }
    }

    private IEnumerator ProcessLogRoutine(GameObject logObject)
    {
        isProcessing = true;

        Rigidbody logRb = logObject.GetComponent<Rigidbody>();
        Collider logCol = logObject.GetComponent<Collider>();
        if (logRb) logRb.isKinematic = true; 
        if (logCol) logCol.enabled = false; 

        if (logObject != null)
        {
            logObject.transform.position = logStartPoint.position;
            logObject.transform.rotation = logStartPoint.rotation;
        }

        float timer = 0f;
        while (timer < rampUpDuration)
        {
            timer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(0f, maxRotationSpeed, timer / rampUpDuration);
            yield return null;
        }
        currentSpeed = maxRotationSpeed;

        float totalProcessTime = planksPerLog * timeBetweenPlanks;
        float processTimer = 0f;
        int planksSpawned = 0;

        Vector3 fixedStartPos = logStartPoint.position;
        Vector3 fixedEndPos = logEndPoint.position;

        while (processTimer < totalProcessTime)
        {
            processTimer += Time.deltaTime;
            
            if (logObject != null)
            {
                float progress = processTimer / totalProcessTime;
                logObject.transform.position = Vector3.Lerp(fixedStartPos, fixedEndPos, progress);
            }

            if (processTimer >= (planksSpawned + 1) * timeBetweenPlanks)
            {
                SpawnPlank();
                planksSpawned++;
            }

            yield return null; 
        }

        if (logObject != null) Destroy(logObject); 

        timer = 0f;
        while (timer < rampDownDuration)
        {
            timer += Time.deltaTime;
            currentSpeed = Mathf.Lerp(maxRotationSpeed, 0f, timer / rampDownDuration);
            yield return null;
        }
        currentSpeed = 0f;
        isProcessing = false;
    }

    private void SpawnPlank()
    {
        if (plankPrefab && outputPoint)
        {
            GameObject newPlank = Instantiate(plankPrefab, outputPoint.position, outputPoint.rotation);
            Rigidbody rb = newPlank.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomForce = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
                rb.AddForce((outputPoint.forward * 5f) + randomForce, ForceMode.Impulse);
            }
        }
    }
}