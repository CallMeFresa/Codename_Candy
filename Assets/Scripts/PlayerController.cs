using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float jumpforce = 10, moveForce = 10;
    private PlayerInput playerInput;
    private Vector2 input;
    public bool jumpIsTriggered;
    private GroundController groundController;
    private float impulseValue = 5;
    #region
    //public LayerMask layer;
    //public static float customGravity = 9.81f;
    //private float globalGravity = 1f;
    #endregion
    private void Awake()
    {
        rb = GetComponent <Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundController = GetComponent<GroundController>();
    }
    private void Update()
    {
        input = playerInput.actions["Move"].ReadValue<Vector2>();

     
    }
    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(input.x, 0f, input.y) * moveForce;
        velocity.y = rb.linearVelocity.y;

        if (jumpIsTriggered)
        {
            velocity.y = jumpforce;
            jumpIsTriggered = false;
        }
        rb.linearVelocity = velocity; 
  
    }
    public void JumpEvent(InputAction.CallbackContext callback)
    {

        if (groundController.isGrounded)
        {
            jumpIsTriggered = true; 
        }
        #region
        //if (callback.performed && isGrounded == true)
        //{ b
        //    Vector3 gravity = globalGravity * customGravity * Vector3.up;
        //    rb.AddForce(gravity * jumpforce , ForceMode.Acceleration);
        //    Debug.Log("Salto ctm" + gravity);
        //}
        #endregion
    }

    public void ImpulseEvent (InputAction.CallbackContext callback)
    {

    }

}
