using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PinballButton : MonoBehaviour
{
    public UnityEvent onPress;

    public LayerMask target;

    private void OnCollisionEnter(Collision collision)
    {
        if ((target & (1 << collision.gameObject.layer)) != 0)
        {
            onPress.Invoke();
        }
    }
}
