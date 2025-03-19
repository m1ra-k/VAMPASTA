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

    void Start()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!GameProgressionManager.transitioning) {
            movementVector.x = Input.GetAxisRaw("Horizontal");
            movementVector.y = Input.GetAxisRaw("Vertical");

            rb.velocity = movementVector.normalized * speed;
        }

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
