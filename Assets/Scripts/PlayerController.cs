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
    public enum Estados { caminando, cargando, corriendo };
    [Header("Config estados")]
    [SerializeField] private Estados currentStatus;
    [SerializeField] private float maxCaminando = 5;
    [SerializeField] private float maxCargando = 0.0001f;
    [SerializeField] private float maxCorriendo = 20;

    [Header("Config cargando")]
    [SerializeField] private float timeCargando = 3;
    private float _timeCargando = 0;
    [SerializeField] private float limitToCaminando = 0.1f;

    [Header("Config impulso")]
    [SerializeField] private float impulso = 1000f;
    private bool _impulsed = false;
    private float _impulsedCooldown = 1;

    [Header("Character")]
    [SerializeField] private Transform root;
    [SerializeField] private float rotationPower = 1;

    [Space]
    private Rigidbody rb;
    [SerializeField] private float jumpforce = 10, moveForce = 10;
    private PlayerInput playerInput;
    private Vector2 input;
    public bool jumpIsTriggered;
    private GroundController groundController;
    [SerializeField] private float impulseValue = 5;
    public float velocity;
    public Vector3 velocity3;
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
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundController = GetComponent<GroundController>();

        SwithState(Estados.caminando);
    }

    private void SwithState(Estados stateToSet)
    {
        currentStatus = stateToSet;

        switch (currentStatus)
        {
            case Estados.caminando:
                moveForce = maxCaminando;
                break;

            case Estados.cargando:
                _impulsed = false;
                _timeCargando = timeCargando;
                moveForce = maxCargando;
                break;

            case Estados.corriendo:
                _impulsed = true;
                moveForce = maxCorriendo;
                break;
        }
    }

    private void Update()
    {
        input = playerInput.actions["Move"].ReadValue<Vector2>();

        velocity = rb.linearVelocity.magnitude;

        cam.fieldOfView = math.clamp(math.lerp(cam.fieldOfView, velocity.Remap(0, moveForce, camerasFov.x, camerasFov.y), Time.deltaTime * speedFovChange), camerasFov.x, camerasFov.y);

        if (currentStatus == Estados.cargando)
        {
            _timeCargando -= Time.deltaTime;

            if (_timeCargando <= 0)
            {
                _impulsedCooldown = 1;
                SwithState(Estados.corriendo);
            }
        }

        if (_impulsedCooldown >= 0)
        {
            _impulsedCooldown -= Time.deltaTime;
        }

        if (_impulsedCooldown <= 0 && currentStatus == Estados.corriendo && velocity <= limitToCaminando)
        {
            SwithState(Estados.caminando);
        }

        root.localEulerAngles = new Vector3(velocity3.y.Remap(-1, 1, rotationPower, -rotationPower), 0, 0);
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(input.x, 0f, input.y) * moveForce;
        velocity.y = rb.linearVelocity.y;

        if (_impulsed)
        {
            velocity = new Vector3(velocity.x + impulso, velocity.y, velocity.z + impulso);
            _impulsed = false;
        }

        if (jumpIsTriggered)
        {
            velocity.y = jumpforce;
            jumpIsTriggered = false;
        }

        rb.linearVelocity = math.lerp(rb.linearVelocity, velocity, Time.deltaTime * speedMoveChange);
        velocity3 = rb.linearVelocity;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
            return;

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

    public void OnImpulse(InputValue value)
    {
        if (value.isPressed && currentStatus == Estados.caminando)
        {
            SwithState(Estados.cargando);
        }
        else if (!value.isPressed && currentStatus == Estados.cargando)
        {
            SwithState(Estados.caminando);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogWarning(collision.GetContact(0).normal);
    }
}