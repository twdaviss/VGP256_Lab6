using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRampSystem : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D rampCollider;
    [SerializeField] private BoxCollider2D blockCollider;

    private Vector2 topRampPoint;
    private Vector2 botRampPoint;
    private Vector2 cornerRampPoint;

    private float angle;
    private float xDiff;
    private float yDiff;
    private float slope;
    private float rampLength;

    void Start()
    {
        topRampPoint = rampCollider.points[1];
        botRampPoint = rampCollider.points[2];
        cornerRampPoint = rampCollider.points[0];
        xDiff = Mathf.Abs(botRampPoint.x - topRampPoint.x);
        yDiff = Mathf.Abs(botRampPoint.y - topRampPoint.y);
        slope = yDiff / xDiff;
        rampLength = Mathf.Sqrt(Mathf.Pow(xDiff,2) + Mathf.Pow(yDiff,2));

        angle = -Mathf.Acos(xDiff / rampLength);
    }

    void Update()
    {
        float yPos = cornerRampPoint.y + (slope * blockCollider.transform.position.x + topRampPoint.y);
        blockCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle);
        blockCollider.transform.position = new Vector3(blockCollider.transform.position.x, yPos, blockCollider.transform.position.z);
    }
}
