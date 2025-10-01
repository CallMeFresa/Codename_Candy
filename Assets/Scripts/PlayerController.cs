using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform root;
    [SerializeField] private Camera cam;
    [SerializeField] private GroundController groundController;

    public enum Estados { caminando, cargando, corriendo };
    [Header("Config estados")]
    [SerializeField] private Estados currentStatus;
    [SerializeField, Tooltip("Velocidad maxima en el estado Caminando")] private float maxCaminando = 5;
    [SerializeField] private float maxCargando = 0.0001f;
    [SerializeField] private float maxCorriendo = 20;

    [Header("Config cargando")]
    [SerializeField] private float timeCargando = 3;
    private float _timeCargando = 0;
    [SerializeField] private bool checkLimitToCaminando = true;
    [SerializeField] private float limitToCaminando = 0.1f;

    [Header("Config impulso")]
    [SerializeField] private float impulso = 1000f;
    private float _impulso;
    private bool _impulsed = false;
    private float _impulsedCooldown = 1;

    [Header("Config combos")]
    [SerializeField] private float maxToPerfect = 0.5f;
    [SerializeField] private float impulsePerfect = 100f;
    [SerializeField] private float maxToGood = 1;
    [SerializeField] private float impulseOc = -10f;

    [Header("Character")]
    [SerializeField] private float rotationPower = 1;

    [Space]
    [SerializeField] private float jumpforce = 10, moveForce = 10;
    [SerializeField] private bool jumpIsTriggered;
    [SerializeField] private float impulseValue = 5;
    [SerializeField] private float currentVelocity;
    [SerializeField] private Vector3 currentVelocity3;

    [Header("Config camera")]
    [SerializeField] private float speedFovChange = 4;
    [SerializeField] private float speedMoveChange = 10;
    [SerializeField] private Vector2 camerasFov;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI text_speed;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 input;
    private Vector3 velocity;
    private float collisionDiference;

    #region ground layer gravity
    //public LayerMask layer;
    //public static float customGravity = 9.81f;
    //private float globalGravity = 1f;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

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
                _impulso = impulso;
                moveForce = maxCorriendo;
                break;
        }
    }

    private void Update()
    {
        input = playerInput.actions["Move"].ReadValue<Vector2>();

        currentVelocity = rb.linearVelocity.magnitude;

        cam.fieldOfView = math.clamp(math.lerp(cam.fieldOfView, currentVelocity.Remap(0, maxCorriendo, camerasFov.x, camerasFov.y), Time.deltaTime * speedFovChange), camerasFov.x, camerasFov.y * 1.5f);

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

        if (checkLimitToCaminando && _impulsedCooldown <= 0 && currentStatus == Estados.corriendo && currentVelocity <= limitToCaminando)
        {
            SwithState(Estados.caminando);
        }

        root.localEulerAngles = new Vector3(currentVelocity3.y.Remap(-1, 1, rotationPower, -rotationPower), 0, 0);

        text_speed.text = $"Speed: {currentVelocity * 3.6f}";
    }

    private void FixedUpdate()
    {
        velocity = new Vector3(input.x, 0f, input.y) * moveForce;
        velocity.y = rb.linearVelocity.y;

        if (_impulsed)
        {
            velocity = new Vector3(velocity.x + _impulso, velocity.y, velocity.z + _impulso);
            _impulsed = false;
        }

        if (jumpIsTriggered)
        {
            velocity.y = jumpforce;
            jumpIsTriggered = false;
        }

        rb.linearVelocity = math.lerp(rb.linearVelocity, velocity, Time.deltaTime * speedMoveChange);
        currentVelocity3 = rb.linearVelocity;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (groundController.isGrounded)
        {
            jumpIsTriggered = true;
        }

        #region Jump
        //if (callback.performed && isGrounded == true)
        //{
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
        if (!groundController.airCoolDownReady)
            return;

        groundController.airCoolDownReady = false;

        collisionDiference = Vector3.Distance(collision.GetContact(0).normal, root.up);

        string combo;
        if (collisionDiference <= maxToPerfect)
        {
            combo = "<color=green>Perfect!</color>";
            _impulsed = true;
            _impulso = impulsePerfect;
        }
        else if (collisionDiference > maxToPerfect && collisionDiference <= maxToGood)
        {
            combo = "<color=orange>Good!</color>";
        }
        else
        {
            combo = "<color=red>Oc!</color>";
            _impulsed = true;
            _impulso = impulseOc;
        }

        Debug.Log($"Collision: <b>{collision.GetContact(0).normal}</b> / Root: <b>{root.up}</b> / Difference: <b>({collisionDiference})</b>\n{combo}");
    }
}