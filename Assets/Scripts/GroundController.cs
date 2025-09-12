using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField]
    private float groundDistanceTolerance;

    [SerializeField]
    private LayerMask groundLayerMask;

    private CapsuleCollider capsuleCollider;
    public bool isGrounded { get; private set; }
    // los { ]} es para saber como la variable obtiene y setea su valor, por ejemplo, aqui
    // puede tener (get) su valor por comportamientos exteriores pero solo este script 
    // (private set) puede setear dicho valor y no otros scripts
    public float? distanceToGround { get; private set; }
      // el "?"  inidica que un valor puede ser "null"

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        float sphereCastRadius = capsuleCollider.radius - 0.1f;
        //esto es para que el metodo no se realice si el jugador esta junto con obstaculos
        Vector3 sphereCastOrigin = transform.position + new Vector3(0, capsuleCollider.radius, 0);
        bool isGroundedBelow = Physics.SphereCast(
            sphereCastOrigin, 
            sphereCastRadius, 
            Vector3.down, 
            out RaycastHit hitInfo,
            1000, 
            groundLayerMask, 
            QueryTriggerInteraction.Ignore);

        if (isGroundedBelow)
        {
            distanceToGround = transform.position.y - hitInfo.point.y;
        }

        else
        {
            distanceToGround = null; 
        }
        isGrounded = isGroundedBelow && distanceToGround <= groundDistanceTolerance;
    }
}
