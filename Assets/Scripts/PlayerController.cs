using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float forwardSpeed = 5f;
    [SerializeField]
    private float sideSpeed = 10f;
    [SerializeField]
    private float laneWidth = 4f;
    [SerializeField]
    private float jumpHeight = 4f;
    [SerializeField]
    private float jumpDuration = 2f;

    [SerializeField]
    private CoinManager coinManager; // Reference to the CoinManager
    [SerializeField]
    private Animator characterAnimator; // Reference to the Animator component on the character model
    [SerializeField]
    private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField]
    private AudioClip coinCollectSound; // Reference to the coin collection sound

    private Vector3 targetPosition;
    private RunnerManager gameManager;
    private bool isJumping = false;
    private float jumpStartTime;
    private Vector3 jumpStartPosition;
    private int currentLane = 1;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private void Start()
    {
        targetPosition = transform.position;
        gameManager = FindObjectOfType<RunnerManager>();

        if (characterAnimator == null)
        {
            Debug.LogError("Character Animator is not assigned.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
        }

        if (coinCollectSound == null)
        {
            Debug.LogError("Coin collect sound is not assigned.");
        }
    }

    private void Update()
    {
        // Automatically move forward
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Smoothly move to the target position in the x direction
        Vector3 newPosition = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * sideSpeed);

        // Handle jump
        if (isJumping)
        {
            float elapsed = Time.time - jumpStartTime;
            if (elapsed < jumpDuration)
            {
                float t = elapsed / jumpDuration;
                float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;
                transform.position = new Vector3(transform.position.x, jumpStartPosition.y + height, transform.position.z);
            }
            else
            {
                isJumping = false;
                transform.position = new Vector3(transform.position.x, jumpStartPosition.y, transform.position.z);
                characterAnimator.SetBool("isJumping", false); // Reset the jump animation
            }
        }

        // Swipe detection
        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0);
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDirection = endTouchPosition - startTouchPosition;

                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                {
                    if (swipeDirection.x > 0)
                    {
                        RightSwipe();
                    }
                    else
                    {
                        LeftSwipe();
                    }
                }
                else
                {
                    if (swipeDirection.y > 0)
                    {
                        UpSwipe();
                    }
                    else
                    {
                        DownSwipe();
                    }
                }
            }
        }
    }

    private void UpSwipe()
    {
        Jump();
    }

    private void DownSwipe()
    {
        // Implement DownSwipe behavior if needed
    }

    private void LeftSwipe()
    {
        MoveLeft();
    }

    private void RightSwipe()
    {
        MoveRight();
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveLeft();
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveRight();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && !isJumping)
        {
            Jump();
        }
    }

    private void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            targetPosition += Vector3.left * laneWidth;
        }
    }

    private void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            targetPosition += Vector3.right * laneWidth;
        }
    }

    private void Jump()
    {
        isJumping = true;
        jumpStartTime = Time.time;
        jumpStartPosition = transform.position;
        characterAnimator.SetBool("isJumping", true); // Trigger the jump animation
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isJumping = false;
            characterAnimator.SetBool("isJumping", false); // Reset the jump animation
        }
        else if (other.CompareTag("Barricade"))
        {
            gameManager.GameOver();
        }
        else if (other.CompareTag("Coin"))
        {
            CoinMovement coin = other.GetComponent<CoinMovement>();
            if (coin != null)
            {
                coin.Collect();
                coinManager.CollectCoin(); // Update the coin count
                PlayCoinCollectSound();
            }
        }
    }

    private void PlayCoinCollectSound()
    {
        if (audioSource != null && coinCollectSound != null)
        {
            audioSource.PlayOneShot(coinCollectSound);
        }
    }
}
