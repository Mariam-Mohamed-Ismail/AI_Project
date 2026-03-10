using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public RectTransform playerInMap;
    public RectTransform Map2DEnd;
    public Transform map3dParent;
    public Transform map3dEnd;

    private Vector3 normalized, mapped;

    private void Update()
    {
        normalized = Divide(
          map3dParent.InverseTransformPoint(this.transform.position),
          map3dEnd.position - map3dParent.position
        );
        normalized.y = normalized.z;
        mapped = Multiply(normalized, Map2DEnd.localPosition);
        mapped.z = 0;
        playerInMap.localPosition = mapped;
    }

    private static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
