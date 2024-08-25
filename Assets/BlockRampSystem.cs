using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.XR;
public enum SurfaceFriction
{
    WoodOnWood,
    WoodOnConcrete,
    WoodOnMetal,
    WoodOnIce,
    MetalOnConcrete,
};
public class BlockRampSystem : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D rampCollider;
    [SerializeField] private BoxCollider2D blockCollider;
    [SerializeField] private float mass;
    [SerializeField] private Vector2 gravity;
    [SerializeField] private SurfaceFriction surfaceFriction = SurfaceFriction.WoodOnWood;

    private Vector2 topRampPoint;
    private Vector2 botRampPoint;
    private Vector2 cornerRampPoint;

    private float angle;
    private float xDiff;
    private float yDiff;
    private float slope;
    private float yInt;
    private float rampLength;
    private float blockWidth;
    private float velocity = 0;

    private float frictionCoefficient;

    private Vector3 originalPosition;
    private bool stopped = false;
    void Start()
    {
        originalPosition = blockCollider.transform.position;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        { 
            blockCollider.transform.position = originalPosition; 
            velocity = 0;
        }
        topRampPoint = rampCollider.points[1] + (Vector2)rampCollider.transform.position;
        botRampPoint = rampCollider.points[2] + (Vector2)rampCollider.transform.position;
        blockWidth = blockCollider.gameObject.transform.localScale.x;
        xDiff = botRampPoint.x - topRampPoint.x;
        yDiff = botRampPoint.y - topRampPoint.y;
        angle = Mathf.Atan2(yDiff, xDiff);

        slope = yDiff / xDiff;
        yInt = topRampPoint.y - (slope * topRampPoint.x);

        float yPos = (slope * blockCollider.transform.position.x) + yInt;

        switch (surfaceFriction)
        {
            case SurfaceFriction.WoodOnWood:
                frictionCoefficient = 0.5f;
                break;
            case SurfaceFriction.WoodOnConcrete:
                frictionCoefficient = 0.45f;
                break;
            case SurfaceFriction.WoodOnMetal:
                frictionCoefficient = 0.3f;
                break;
            case SurfaceFriction.MetalOnConcrete:
                frictionCoefficient = 0.6f;
                break;
            case SurfaceFriction.WoodOnIce:
                frictionCoefficient = 0.05f;
                break;
        }

        Vector2 fg = gravity * mass;
        Vector2 fgy = fg * Mathf.Cos(-angle);
        Vector2 fgx = fg * Mathf.Sin(-angle);
        Vector2 fn = -fgy;
        Vector2 ff = frictionCoefficient * fn;
        if(Mathf.Abs(ff.magnitude) > fgx.magnitude) { ff = -fgx; }
        float totalForce = fgx.magnitude - ff.magnitude;
        float acceleration = totalForce/mass;

        velocity += acceleration * Time.deltaTime * 0.01f;
        if (stopped) { velocity = 0f; }
        float xVelocity = velocity * Mathf.Sin(-angle);
        blockCollider.transform.position = new Vector3(blockCollider.transform.position.x + xVelocity, yPos + blockWidth / 2, 0);
        blockCollider.transform.SetPositionAndRotation(new Vector3(blockCollider.transform.position.x + xVelocity, yPos + blockWidth / 2, 0), Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            stopped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            stopped = false;
        }
    }
}
