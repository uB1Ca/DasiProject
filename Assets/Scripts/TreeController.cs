using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("Health")]
    public int treeMaxHealth = 40;
    private int currentHealth;

    [Header("Cutting")]
    public float cutAnimationDuration = 2.0f;

    [Header("Dropping Logs")]
    public GameObject logPrefab;
    public Transform logDropPoint;
    public int logsPerHit = 2;
    public float maxLogDropRadius = 2.0f; // Maximum radius for log drops.

    [Header("Regrowth")]
    public GameObject treePrefab;
    public float regrowthTime = 5.0f;

    private int cutProgress = 0;
    private bool isCutting = false;
    public bool IsCut { get; private set; } // Property to check if the tree is cut

    private List<Transform> childTransforms = new List<Transform>();

    private void Start()
    {
        currentHealth = treeMaxHealth;
        CollectChildTransforms();
    }

    private void CollectChildTransforms()
    {
        // Collect all child transforms.
        foreach (Transform child in transform)
        {
            childTransforms.Add(child);
        }
    }

    private void Update()
    {
        // Add any tree-specific logic here if needed.
    }

    public void TakeDamage(int damage)
    {
        // Check if the tree is already cut, and prevent further damage.
        if (IsCut)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            CutDown();
        }
        else
        {
            // Trigger the cutting animation when the tree is hit.
            CutTree();
        }
    }

    private void CutDown()
    {
        IsCut = true; // Set the flag to indicate the tree is cut.
        DropLogs();
        StartCoroutine(RegrowTree());
    }

    private void DropLogs()
    {
        for (int i = 0; i < logsPerHit; i++)
        {
            Vector3 logDropPosition = CalculateLogDropPosition();
            Instantiate(logPrefab, logDropPosition, Quaternion.identity);
        }
    }

    private Vector3 CalculateLogDropPosition()
    {
        // Generate a random angle and radius within the specified range.
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(0f, maxLogDropRadius);

        // Calculate the log's position based on the angle and radius.
        Vector3 logPosition = logDropPoint.position +
            Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;

        return logPosition;
    }

    private IEnumerator RegrowTree()
    {
        yield return new WaitForSeconds(regrowthTime);
        Instantiate(treePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Trigger the cutting animation and apply damage when the tree is cut.
    private void CutTree()
    {
        if (isCutting)
        {
            return; // Tree is already being cut.
        }

        isCutting = true;
        StartCoroutine(CuttingAnimation());

        // Call DropLogs here to drop logs when the tree is hit.
        DropLogs();
    }

    private IEnumerator CuttingAnimation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < cutAnimationDuration)
        {
            float t = elapsedTime / cutAnimationDuration;

            // Scale down the next child object in the list progressively.
            int indexToCut = Mathf.Clamp(cutProgress, 0, childTransforms.Count - 1);
            childTransforms[indexToCut].localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the scaled child object is fully cut.
        int finalIndexToCut = Mathf.Clamp(cutProgress, 0, childTransforms.Count - 1);
        childTransforms[finalIndexToCut].localScale = Vector3.zero;

        // Increment the cut progress.
        cutProgress++;
        isCutting = false;
    }
}
