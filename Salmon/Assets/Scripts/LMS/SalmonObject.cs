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
    public Rigidbody rb;
    #endregion

    #region Salmon Info    
    [Header("속도 관련")]
    public float accelerationBase = 2f; // a in y = a^x
    public float decelerationBase = 1f; // a in y = a^-x 
    public float currentSpeed = 0f;
    public float maxSpeed = 7f;
    public float minSpeed = 0.1f; // 브레이크 후 최소 속도
    public Vector3 waterCurrent = Vector3.zero; // 외부 물살 방향

    [Header("회전 관련")]  
    public float rotationAcceleration = 90f; // 회전 가속도
    public float maxAngularSpeed = 180f; // 최대 회전 속도 
    public float rotationDamping = 180f; //회전 감속도


    [Header("시간 관련")]
    public float speedTime = 0f; //가속 시간
    public float brakeTime = 0f; //감속 시간
    public float angularVelocity = 0f; 
    
    [Header("조작 체크")]
    private Vector2 moveInput = Vector2.zero;
    public bool isAccelerating = false;
    public bool isBraking = false;
    public bool isTurningRight = false;
    public bool isTurningLeft = false;
    public int turnDirection = 0; // -1: 좌, 1: 우 
    #endregion 

    #region Collision Info  
    [Header("콜라이더 체크")]
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform waterCheck;
    [SerializeField] public Transform obstructionCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsWater;
    [SerializeField] protected LayerMask whatIsObstruction;
    [SerializeField] public float groundCheckDistance = 4;
    [SerializeField] public float waterCheckDistance = 4;
    [SerializeField] public float obstructionCheckDistance = 4;


    #endregion
    public virtual bool IsGroundDetected() =>
    Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWaterDetected() =>
    Physics.Raycast(waterCheck.position, Vector3.down, waterCheckDistance, whatIsWater);

    public virtual bool IsobstructionDetected() =>
    Physics.Raycast(obstructionCheck.position, Vector3.forward, obstructionCheckDistance, whatIsObstruction);




    #region Setters and Getters 
    public virtual bool IsTurningRight
    {
        get => isTurningRight;
        set
        {
            isTurningRight = value;
            if (isTurningRight)
            {
                isTurningLeft = false; // 오른쪽 회전 중이면 왼쪽 회전 해제
            }
        }
    }
    public virtual bool IsTurningLeft
    {
        get => isTurningLeft;
        set
        {
            isTurningLeft = value;
            if (isTurningLeft)
            {
                isTurningRight = false; // 왼쪽 회전 중이면 오른쪽 회전 해제
            }
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
    } 
    void FixedUpdate()  //임시 작동
    { 
        // 점프/중력 처리: 일정 거리 아래에 수면이 없으면 중력 적용
        
        if (isBraking)
        {
            speedTime = 0f;
            brakeTime += Time.fixedDeltaTime;
            float decel = Mathf.Pow(decelerationBase, brakeTime) * currentSpeed;
            currentSpeed = Mathf.Max(decel, minSpeed);
        }
        else if (isAccelerating)
        {
            brakeTime = 0f;
            speedTime += Time.fixedDeltaTime;
            currentSpeed = Mathf.Pow(accelerationBase, speedTime);
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        { 
            speedTime = 0f;
            brakeTime = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.fixedDeltaTime);
        }

        //최종 속도는 현재 벡터와 물살 벡터의 합 (레거시)
        //rb.linearVelocity = transform.forward * currentSpeed + waterCurrent;
        Vector3 flatVelocity = transform.forward * currentSpeed + waterCurrent;
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);

 
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
        transform.Rotate(Vector3.up, angularVelocity * Time.fixedDeltaTime); 
    }

    /// <summary>  
    /// 물살을 적용하는 메서드
    /// </summary>  
    public void ApplyWaterCurrent(Vector3 current)
    {
        waterCurrent = current; 
    }
     

    //점프 구현
    //회전 개선
    //스턴 구현

    // 부력 : 수면이 닿아있으면, 부력에 뜨는 것처럼 점차 평행한 각도로 세워지면서 높이가 상승.
}
