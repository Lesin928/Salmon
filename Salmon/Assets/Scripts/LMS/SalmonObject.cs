using System.Collections;
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
    protected Rigidbody rb;
    #endregion

    #region Salmon Info    
    [Header("속도 관련")]
    public float acceleration = 1f;
    public float maxSpeed = 5f;
    public float brakeDeceleration = 2f;
    public float waterResistance = 0.5f;
    public float driftBackSpeed = -0.5f;

    [Header("회전 관련")]
    public float turnSpeed = 90f; // 도/초 단위
    public float turnSmoothness = 5f;

    private float currentSpeed = 0f;
    private float targetYaw = 0f;
     
    private Vector2 moveInput = Vector2.zero;

    private bool isAccelerating = false;
    private bool isBraking = false;
    private Vector3 swimDirection = Vector3.forward;

    #endregion

    #region Setters and Getters
    public virtual float TargetYaw
    {
        get => targetYaw;
        set
        {
            targetYaw = value;
            // 회전 시 방향 벡터 갱신
            swimDirection = Quaternion.Euler(0f, targetYaw, 0f) * Vector3.forward;
        }
    }   
    public virtual bool IsAccelerating
    {
        get => isAccelerating;
        set
        {
            isAccelerating = value;
            if (isAccelerating)
            {
                isBraking = false; // 가속 중이면 브레이크 해제
            }
        }
    }   

    public virtual bool IsBraking
    {
        get => isBraking;
        set
        {
            isBraking = value;
            if (isBraking)
            {
                isAccelerating = false; // 브레이크 중이면 가속 해제
            }
        }
    }   


    public virtual Vector3 MoveInput
    {
        get => moveInput;
        set
        {
            moveInput = value;
        }
    }
    #endregion

    private void Awake()
    {
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;             // 중력 제거 (수영이므로)
        rb.linearDamping = 0f;             // 물리 마찰 직접 계산할 예정 
        targetYaw = transform.eulerAngles.y;
    }


    void Update()
    {

        // A/D 입력에 따라 목표 Y 회전값 갱신
        targetYaw += moveInput.x * turnSpeed * Time.deltaTime;

        // 현재 방향에서 목표 방향으로 부드럽게 회전
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, turnSpeed * Time.deltaTime);
    } 
    void FixedUpdate()
    {
        if (isAccelerating)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (isBraking)
        {
            currentSpeed -= brakeDeceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }
        else
        {/*
            currentSpeed -= waterResistance * Time.fixedDeltaTime;

            if (currentSpeed <= 0f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, driftBackSpeed, Time.fixedDeltaTime);
            }*/
        }

        // 최종 속도 적용 (항상 바라보는 방향 기준)
        Vector3 velocity = transform.forward * currentSpeed;
        rb.linearVelocity = velocity;
    }

}
