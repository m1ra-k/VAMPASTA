using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AnimationClip[] raviAnimations;

    private GameProgressionManager GameProgressionManager;
    private float speed = 350f;
    private Rigidbody2D rb;
    private Vector2 movementVector;
    private Vector2 prevMovementVector;
    private Animator animator;

    private Vector2 targetPosition;
    private float stepSize = 100f;
    private float moveSpeed = 300f;
    private bool isMoving;
    public LayerMask obstacleLayer;

    void Start()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        targetPosition = transform.position;
    }

    void Update()
    {
        Move();

        if (movementVector != prevMovementVector)
        {
            DetermineAnimation();
        }

        if (movementVector == Vector2.zero)
        {
            DetermineFrame();
        }

        prevMovementVector = movementVector;
    }

    void Move()
    {
        if (!isMoving)
        {
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    

            if (movementVector != Vector2.zero)
            {
                GameProgressionManager.facingUp = movementVector.x == 0 && movementVector.y == 1;
                // print($"vector is {movementVector.x == 0 && movementVector.y == 1}");
                // print($"gpm is {GameProgressionManager.facingUp}");

                if (Mathf.Abs(movementVector.x) > Mathf.Abs(movementVector.y))
                {
                    movementVector.y = 0;
                }
                else
                {
                    movementVector.x = 0;
                }

                Vector2 nextPosition = (Vector2)transform.position + movementVector * stepSize;

                if (!Physics2D.Raycast(transform.position, movementVector, 160f, obstacleLayer))
                {
                    targetPosition = nextPosition;
                    isMoving = true;
                }
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    void DetermineAnimation()
    {
        animator.speed = 1;

        switch (movementVector)
        {
            case Vector2 v when v == Vector2.up:
                animator.Play(raviAnimations[0].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.down:
                animator.Play(raviAnimations[1].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.left:
                animator.Play(raviAnimations[2].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.right:
                animator.Play(raviAnimations[3].name, 0, 0f);
                break;
        }
    }

    void DetermineFrame()
    {
        animator.Play(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, 0);
        animator.speed = 0;
    }
}
