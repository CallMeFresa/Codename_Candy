using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Extensions
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float jumpforce = 10, moveForce = 10;
    private PlayerInput playerInput;
    private Vector2 input;
    public bool jumpIsTriggered;
    private GroundController groundController;
    [SerializeField] private float impulseValue = 5;
    public float velocity;
    public float speedFovChange = 4;
    public float speedMoveChange = 10;
    public Vector2 camerasFov;
    public Camera cam;

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

        velocity = rb.linearVelocity.magnitude;

        cam.fieldOfView = math.lerp(cam.fieldOfView, velocity.Remap(0, moveForce, camerasFov.x, camerasFov.y), Time.deltaTime * speedFovChange);
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
        rb.linearVelocity = math.lerp(rb.linearVelocity, velocity, Time.deltaTime * speedMoveChange);
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
