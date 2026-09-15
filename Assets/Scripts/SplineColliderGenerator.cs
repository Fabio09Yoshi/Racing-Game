using UnityEngine;
using UnityEngine.Splines;

public class SplineColliderGenerator : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Collider Settings")]
    public float colliderSpacing = 1f;

    public float colliderWidth = 0.5f;
    public float colliderHeight = 1.5f;

    [Header("Position")]
    public float verticalOffset = 0.5f;

    [Header("Parent")]
    public Transform colliderParent;

    [ContextMenu("Generate Colliders")]
    public void GenerateColliders()
    {
        if (splineContainer == null)
        {
            Debug.LogWarning("SplineContainer não configurado.");
            return;
        }

        ClearColliders();

        Spline spline = splineContainer.Spline;

        float length = spline.GetLength();

        int amount = Mathf.CeilToInt(length / colliderSpacing);

        for (int i = 0; i < amount; i++)
        {
            float t1 = (float)i / amount;
            float t2 = (float)(i + 1) / amount;

            Vector3 point1 = splineContainer.EvaluatePosition(t1);
            Vector3 point2 = splineContainer.EvaluatePosition(t2);

            Vector3 direction = point2 - point1;

            float segmentLength = direction.magnitude;

            if (segmentLength <= 0.001f)
                continue;

            Vector3 center = (point1 + point2) / 2f;

            GameObject colliderObject =
                new GameObject("TrackCollider_" + i);

            if (colliderParent != null)
                colliderObject.transform.SetParent(colliderParent);
            else
                colliderObject.transform.SetParent(transform);

            colliderObject.transform.position =
                center + Vector3.up * verticalOffset;

            colliderObject.transform.rotation =
                Quaternion.LookRotation(direction.normalized, Vector3.up);

            BoxCollider box =
                colliderObject.AddComponent<BoxCollider>();

            box.size = new Vector3(
                colliderWidth,
                colliderHeight,
                segmentLength
            );
        }

        Debug.Log("Track colliders generated.");
    }

    [ContextMenu("Clear Colliders")]
    public void ClearColliders()
    {
        Transform parent =
            colliderParent != null ? colliderParent : transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            if (child.name.StartsWith("TrackCollider_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}