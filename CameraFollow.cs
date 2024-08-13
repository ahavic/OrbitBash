using Scripts.Character.Movement;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target = null;
    [SerializeField] float distanceDamp = 10f;
    [SerializeField] Vector3 defaultDistance = new Vector3(0,2,-10);
    float rotationalDamp = 5f;
    float initialSmoothTimer = 2f;
    [SerializeField] float smoothTimer = 2f;

    bool isManueverOffset = false;
    Vector3 offset = Vector3.zero;
    Vector3 targetOffset = Vector3.zero;
    [SerializeField] float offY, offZ;

    [SerializeField] Vector3 velocity = Vector3.one;

    Transform myTransform = null;

    CharacterMovement movement = null;

    private void Awake()
    {
        myTransform = transform;
        smoothTimer = initialSmoothTimer;
    }

    private void Start()
    {
        movement = FindObjectOfType<CharacterMovement>();
        target = movement.transform;
        movement.OnEnterManuever += StartManuever;
        movement.OnExitManuever += EndManuever;
    }

    private void OnDestroy()
    {
        movement.OnEnterManuever -= StartManuever;
        movement.OnExitManuever -= EndManuever;
    }

    private void Update()
    {
        if(isManueverOffset)
        {
            offset = Vector3.Lerp(offset, targetOffset, .03f);
        }
    }

    private void LateUpdate()
    {
        SmoothFollow(offset);
    }

    void SmoothFollow(Vector3 offset)
    {
        Vector3 toPos = target.position + (target.rotation * (defaultDistance + offset));
        //Vector3 curPos = Vector3.Lerp(myTransform.position, toPos,Time.deltaTime * distanceDamp);
        Vector3 curPos = Vector3.SmoothDamp(myTransform.position, toPos, ref velocity, distanceDamp, 100, smoothTimer * Time.deltaTime);
        myTransform.position = curPos;        

        Quaternion toRot = Quaternion.LookRotation(target.position - myTransform.position, target.up);
        Quaternion curRot = Quaternion.Slerp(myTransform.rotation, toRot, rotationalDamp * Time.deltaTime);
        myTransform.rotation = curRot;
    }

    void StartManuever(eManuevers m)
    {
        switch(m)
        {
            case eManuevers.Loop:
                targetOffset = new Vector3(0, -10, -8);
                isManueverOffset = true;
                break;
            case eManuevers.UTurn:
                targetOffset = new Vector3(0, 20, 0);
                isManueverOffset = true;
                break;
            default:
                break;
        }        
    }

    void EndManuever()
    {
        offset = Vector3.zero;
        smoothTimer = initialSmoothTimer;
        isManueverOffset = false;
    }
}
