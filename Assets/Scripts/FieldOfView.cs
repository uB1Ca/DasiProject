using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 45f;
    public LayerMask targetMask;

    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindTargets());
    }

    IEnumerator FindTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            foreach (Collider target in targetsInViewRadius)
            {
                Transform targetTransform = target.transform;
                Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
                {
                    float dstToTarget = Vector3.Distance(transform.position, targetTransform.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask))
                    {
                        visibleTargets.Add(targetTransform);
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleInRadians), 0, MathF.Cos(angleInRadians));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(transform.position, viewRadius);
        Vector3 viewAngleA = DirFromAngle(-viewAngle * 0.5f);
        Vector3 viewAngleB = DirFromAngle(viewAngle * 0.5f);
        
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position,transform.position+viewAngleB * viewRadius);
    }
}