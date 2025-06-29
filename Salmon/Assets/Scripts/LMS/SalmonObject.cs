using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
///  연어 객체를 관리하는 클래스
///  이후 세분화 할 예정
/// </summary>
public class SalmonObject : MonoBehaviour
{
    #region Components     
    protected SalmonStateMachine stateMachine;
    protected SalmonAnimation playerAnimation;
    protected SalmonObject playerObject;
    public Rigidbody rb;
    public CinemachineCamera freeLookCamera; // FreeLook 카메라 참조
    #endregion

    #region Salmon Info    
    [Header("속도 관련")]
    [SerializeField] private float accelerationBase = 2f; // a in y = a^x
    [SerializeField] private float decelerationBase = 1f; // a in y = a^-x 
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float minSpeed = 0.1f; // 브레이크 후 최소 속도
    [SerializeField] private Vector3 waterCurrent = Vector3.zero; // 외부 물살 방향
    [SerializeField] private float jumpForce = 8f;  // 세게 튀는 힘


    [Header("회전 관련")]
    [SerializeField] private float rotationSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    //레거시 코드
    /*
    [Header("회전 관련")]  
    public float rotationAcceleration = 90f; // 회전 가속도
    public float maxAngularSpeed = 180f; // 최대 회전 속도 
    public float rotationDamping = 180f; //회전 감속도
    */

    [Header("시간 관련")]
    [SerializeField] private float speedTime = 0f; //가속 시간
    [SerializeField] private float brakeTime = 0f; //감속 시간
    [SerializeField] private float angularVelocity = 0f;

    [Header("조작 체크")]
    [SerializeField] private Vector2 moveInput = Vector2.zero;
    [SerializeField] private int turnDirection = 0; // -1: 좌, 1: 우 
    [SerializeField] private bool isChangingDirection = false;
    #endregion

    #region Collision Info  
    [Header("콜라이더 체크")]
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform waterCheck;
    [SerializeField] public Transform obstructionCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsWater;
    [SerializeField] protected LayerMask whatIsObstruction;
    [SerializeField] public float groundCheckDistance = 1;
    [SerializeField] public float waterCheckDistance = 1;
    [SerializeField] public float obstructionCheckDistance = 1;
    #endregion

    #region Detected Info  
    public virtual bool IsGroundDetected() =>
    Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWaterDetected() =>
    Physics.Raycast(waterCheck.position, Vector3.down, waterCheckDistance, whatIsWater);

    public virtual bool IsobstructionDetected() =>
    Physics.Raycast(obstructionCheck.position, Vector3.forward, obstructionCheckDistance, whatIsObstruction);
    #endregion

    #region Setters and Getters 
    public float AccelerationBase => accelerationBase;
    public float DecelerationBase => decelerationBase;
    public float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }
    public float MaxSpeed => maxSpeed;
    public float MinSpeed => minSpeed;
    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }   
    public Vector3 WaterCurrent
    {
        get => waterCurrent;
        set => waterCurrent = value;
    }
    public float RotationSmoothTime => rotationSmoothTime;

    public float SpeedTime
    {
        get => speedTime;
        set => speedTime = value;
    }
    public float BrakeTime
    {
        get => brakeTime;
        set => brakeTime = value;
    }
    public float AngularVelocity
    {
        get => angularVelocity;
        set => angularVelocity = value;
    }

    public int TurnDirection
    {
        get => turnDirection;
        set => turnDirection = value;
    }

    public bool IsChangingDirection
    {
        get => isChangingDirection;
        set => isChangingDirection = value;
    }

    public Vector2 MoveInput
    {
        get => moveInput;
        set => moveInput = value;
    }
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    } 
    void FixedUpdate()
    {
        // 점프 후 공중에서 이동 방향으로 X축 회전 적용
        if (!IsWaterDetected())
        {
            Debug.Log("공중에 있습니다.");
            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0f;
            if (horizontalVelocity.sqrMagnitude > 0.1f)
            {
                Quaternion airRot = Quaternion.LookRotation(horizontalVelocity.normalized);
                transform.rotation = Quaternion.Euler(airRot.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
            }
        }
        // 시네머신 카메라 기준 이동 방향 계산
        if (moveInput != Vector2.zero && freeLookCamera != null)
        {
            Transform camTransform = freeLookCamera.transform;

            Vector3 camForward = camTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = camTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 desiredDirection = camForward * moveInput.y + camRight * moveInput.x;
            desiredDirection.Normalize();

            if (desiredDirection != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, smoothedAngle, 0f);
            }
        }

        //회전시 속도 잠깐 감소
        if (isChangingDirection)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime * 5f); 
        }

        // 입력시 움직임
        if(moveInput != Vector2.zero)
        {
            speedTime += Time.fixedDeltaTime;
            currentSpeed = Mathf.Pow(accelerationBase, speedTime);
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (currentSpeed > 0.1f)
        {
            speedTime = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime * 5f); // 현재 속도 감소
        }
        else
        { 
            currentSpeed = 0f; // 속도가 0 이하로 떨어지면 0으로 설정
        }

        Vector3 flatVelocity = transform.forward * currentSpeed + waterCurrent;
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
        // 수평 회복 적용
        //MaintainUprightRotation(); 
    }

    /// <summary>  
    /// 물살을 적용하는 메서드
    /// </summary>  
    public void ApplyWaterCurrent(Vector3 current)
    {
        waterCurrent = current; 
    }

    /// <summary>
    /// 연어를 날려버리는 메서드
    /// </summary>
    public void TakePush(Vector3 vector, float force)
    {
        rb.AddForce(vector * force + rb.linearVelocity, ForceMode.Impulse);
    } 
    /// <summary>
    /// 연어 수평회복 메서드
    /// </summary>
    private void MaintainUprightRotation()
    {
        if (IsWaterDetected())
        {
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0f, currentRot.eulerAngles.y, 0f); // Y만 유지하고 X/Z는 수평으로 보정

            transform.rotation = Quaternion.Slerp(currentRot, targetRot, 2f * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 연어 회전 메서드 (레거시)
    /// </summary>
    private void Rotation()
    {
        /*
        if (isTurningRight) turnDirection = 1;  
        else if (isTurningLeft) turnDirection = -1;  
        else turnDirection = 0;  

        if (currentSpeed > 0.1f && turnDirection != 0)
        {
            angularVelocity += turnDirection * rotationAcceleration * Time.fixedDeltaTime;
            angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularSpeed, maxAngularSpeed);
        }
        else
        {
            angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, rotationDamping * Time.fixedDeltaTime);
        }


        //최종 회전 적용
        transform.Rotate(Vector3.up, angularVelocity * Time.fixedDeltaTime); */
    }

    /// <summary>
    /// 연어 이동 메서드 (레거시)
    /// </summary>
    private void SalmonMove()
    { 
        // 속도 계산 (레거시) 
        /*
        if (isBraking)            
        {
            Debug.Log("Braking");
            speedTime = 0f;
            brakeTime += Time.fixedDeltaTime;
            float decel = Mathf.Pow(decelerationBase, brakeTime) * currentSpeed;
            currentSpeed = Mathf.Max(decel, minSpeed);
        }
        else if (isAccelerating)
        {
            Debug.Log("Accelerating"); 
            brakeTime = 0f;
            speedTime += Time.fixedDeltaTime;
            currentSpeed = Mathf.Pow(accelerationBase, speedTime);
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        { 
            Debug.Log("Idle");
            speedTime = 0f;
            brakeTime = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime);
        }
        */


        //최종 속도는 현재 벡터와 물살 벡터의 합 (레거시)
        //rb.linearVelocity = transform.forward * currentSpeed + waterCurrent;
    }

}
 