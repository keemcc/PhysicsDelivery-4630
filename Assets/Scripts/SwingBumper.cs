using UnityEngine;
using System.Collections.Generic;

// Put this on the bumper pivot object. The pivot's origin is the hinge point.
// Swings out to swingAngle quickly (like a pinball flipper), then returns to rest.
[RequireComponent(typeof(Rigidbody))]
public class SwingBumper : MonoBehaviour
{
    [Header("Swing")]
    public float swingAngle = 60f;        // degrees from rest
    public float swingSpeed = 600f;       // degrees/sec going out
    public float returnSpeed = 200f;      // degrees/sec coming back
    public Vector3 swingAxis = Vector3.up; // local axis to rotate around (flip sign of angle to reverse)

    [Header("Push")]
    public float pushMultiplier = 1f;     // scales how fast the player is shoved (1 = matches the bumper's surface speed)
    public float maxPushSpeed = 15f;      // cap on the shove speed (units/sec)

    [Header("Trigger")]
    public Collider triggerZone;          // a trigger collider (can be on another object). If empty, uses a trigger collider on this object.
    public string playerTag = "Player";
    public float retriggerDelay = 0.5f;   // seconds after returning to rest before swinging again if the player is still in the zone

    Quaternion restRotation;
    float currentAngle;
    enum State { Idle, Swinging, Returning }
    State state = State.Idle;
    Rigidbody rb;
    int playersInside;
    float idleTimer;

    void Awake()
    {
        restRotation = transform.localRotation;

        // Kinematic body so the swing is a proper physics move instead of a teleporting collider.
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("SwingBumper: add a Rigidbody to this object.", this);
            enabled = false;
            return;
        }
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (triggerZone == null) triggerZone = GetComponent<Collider>();
        if (triggerZone == null)
        {
            Debug.LogWarning("SwingBumper: no triggerZone assigned and no collider on this object.", this);
            return;
        }

        // Trigger events only go to scripts on the collider's own object, so relay them here.
        var relay = triggerZone.GetComponent<SwingBumperTriggerRelay>();
        if (relay == null) relay = triggerZone.gameObject.AddComponent<SwingBumperTriggerRelay>();
        relay.owners.Add(this);
    }

    public void OnZoneEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playersInside++;
        Swing();
    }

    public void OnZoneExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playersInside = Mathf.Max(0, playersInside - 1);
    }

    public void Swing()
    {
        if (state != State.Idle) return;
        state = State.Swinging;
        idleTimer = 0f;
    }

    void OnCollisionEnter(Collision collision) { Push(collision); }
    void OnCollisionStay(Collision collision) { Push(collision); }

    // Shove the player along the direction the bumper surface is moving, flat on the ground.
    void Push(Collision collision)
    {
        if (state != State.Swinging) return;
        if (!collision.collider.CompareTag(playerTag)) return;
        Rigidbody body = collision.rigidbody;
        if (body == null) return;

        Vector3 worldAxis = transform.TransformDirection(swingAxis.normalized);
        Vector3 angularVelocity = worldAxis * (Mathf.Deg2Rad * swingSpeed * Mathf.Sign(swingAngle));
        Vector3 surfaceVelocity = Vector3.Cross(angularVelocity, collision.GetContact(0).point - transform.position);
        surfaceVelocity.y = 0f;

        Vector3 push = Vector3.ClampMagnitude(surfaceVelocity * pushMultiplier, maxPushSpeed);
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(push.x, Mathf.Min(v.y, 0f), push.z); // no upward launch
    }

    void FixedUpdate()
    {
        if (state == State.Swinging)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, swingAngle, swingSpeed * Time.fixedDeltaTime);
            if (Mathf.Approximately(currentAngle, swingAngle)) state = State.Returning;
        }
        else if (state == State.Returning)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.fixedDeltaTime);
            if (Mathf.Approximately(currentAngle, 0f)) state = State.Idle;
        }
        else
        {
            if (playersInside > 0)
            {
                idleTimer += Time.fixedDeltaTime;
                if (idleTimer >= retriggerDelay) Swing();
            }
            return;
        }

        Quaternion local = restRotation * Quaternion.AngleAxis(currentAngle, swingAxis.normalized);
        Quaternion parentRot = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        rb.MoveRotation(parentRot * local);
    }
}

// Auto-added to the trigger zone object; forwards player entry to the SwingBumper.
public class SwingBumperTriggerRelay : MonoBehaviour
{
    [HideInInspector] public List<SwingBumper> owners = new List<SwingBumper>();

    void OnTriggerEnter(Collider other)
    {
        if (owners?.Count > 0)
        {
            for (int i = 0; i < owners.Count; i++)
            {
                owners[i].OnZoneEnter(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (owners?.Count > 0)
        {
            for (int i = 0; i < owners.Count; i++)
            {
                owners[i].OnZoneExit(other);
            }
        }
    }
}
