using UnityEngine;

// Put this on the bumper pivot object. The pivot's origin is the hinge point.
// Swings out to swingAngle quickly (like a pinball flipper), then returns to rest.
public class SwingBumper : MonoBehaviour
{
    [Header("Swing")]
    public float swingAngle = 60f;        // degrees from rest
    public float swingSpeed = 600f;       // degrees/sec going out
    public float returnSpeed = 200f;      // degrees/sec coming back
    public Vector3 swingAxis = Vector3.up; // local axis to rotate around (flip sign of angle to reverse)

    [Header("Trigger")]
    public Collider triggerZone;          // a trigger collider (can be on another object). If empty, uses a trigger collider on this object.
    public string playerTag = "Player";
    public float retriggerDelay = 0.5f;   // seconds after returning to rest before swinging again if the player is still in the zone

    Quaternion restRotation;
    float currentAngle;
    enum State { Idle, Swinging, Returning }
    State state = State.Idle;
    int playersInside;
    float idleTimer;

    void Awake()
    {
        restRotation = transform.localRotation;

        if (triggerZone == null) triggerZone = GetComponent<Collider>();
        if (triggerZone == null)
        {
            Debug.LogWarning("SwingBumper: no triggerZone assigned and no collider on this object.", this);
            return;
        }

        // Trigger events only go to scripts on the collider's own object, so relay them here.
        var relay = triggerZone.GetComponent<SwingBumperTriggerRelay>();
        if (relay == null) relay = triggerZone.gameObject.AddComponent<SwingBumperTriggerRelay>();
        relay.owner = this;
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

    void Update()
    {
        if (state == State.Swinging)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, swingAngle, swingSpeed * Time.deltaTime);
            if (Mathf.Approximately(currentAngle, swingAngle)) state = State.Returning;
        }
        else if (state == State.Returning)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);
            if (Mathf.Approximately(currentAngle, 0f)) state = State.Idle;
        }
        else
        {
            if (playersInside > 0)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= retriggerDelay) Swing();
            }
            return;
        }

        transform.localRotation = restRotation * Quaternion.AngleAxis(currentAngle, swingAxis.normalized);
    }
}

// Auto-added to the trigger zone object; forwards player entry to the SwingBumper.
public class SwingBumperTriggerRelay : MonoBehaviour
{
    [HideInInspector] public SwingBumper owner;

    void OnTriggerEnter(Collider other)
    {
        if (owner != null) owner.OnZoneEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (owner != null) owner.OnZoneExit(other);
    }
}
