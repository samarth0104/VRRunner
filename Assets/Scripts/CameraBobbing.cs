using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [SerializeField]
    private float bobbingSpeed = 0.18f; // Speed of the bobbing
    [SerializeField]
    private float bobbingAmount = 0.05f; // Amount of bobbing

    private float defaultYPos = 0;
    private float timer = 0;

    private void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    private void Update()
    {
        // Calculate new Y position based on sinusoidal function
        float waveslice = Mathf.Sin(timer);
        timer += bobbingSpeed;

        if (timer > Mathf.PI * 2)
        {
            timer = timer - (Mathf.PI * 2);
        }

        float newY = defaultYPos + waveslice * bobbingAmount;

        Vector3 newPos = transform.localPosition;
        newPos.y = newY;
        transform.localPosition = newPos;
    }
}
