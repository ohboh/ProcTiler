//This code is from Robert Toana's video, "One Way 3D Colliders (Platforms) | Unity Tutorial". (https://youtu.be/qwwjwb7XlUc)
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OneWayBoxCollider : MonoBehaviour
{
    [SerializeField] private Vector3 entryDirection = Vector3.up;
    [SerializeField] private bool localDirection = false;
    [SerializeField] private Vector3 triggerScale = Vector3.one * 1.25f;
    private new BoxCollider collider = null;
    private BoxCollider collisionCheckTrigger = null;

    public Vector3 Direction => localDirection ? transform.TransformDirection(entryDirection.normalized) : entryDirection.normalized;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        collisionCheckTrigger = gameObject.AddComponent<BoxCollider>();
        collisionCheckTrigger.size = new Vector3(
            collider.size.x * triggerScale.x,
            collider.size.y * triggerScale.y,
            collider.size.z * triggerScale.z
        );
        collisionCheckTrigger.center = collider.center;
        collisionCheckTrigger.isTrigger = true;
    }

    private void OnValidate()
    {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Physics.ComputePenetration(
            collisionCheckTrigger, transform.position, transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out Vector3 collisionDirection, out float _))
        {
            float dot = Vector3.Dot(Direction, collisionDirection);

            if (dot < 0)
            {
                Physics.IgnoreCollision(collider, other, false);
            }
            else
            {
                Physics.IgnoreCollision(collider, other, true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.TransformPoint(collider.center), Direction * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.TransformPoint(collider.center), -Direction * 2);
    }
}