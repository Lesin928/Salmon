using UnityEngine;

// TODO: (추가할일 적는부분)
// FIXME: (고칠거 적는부분)
// NOTE : (기타 작성)

/// <summary>
/// 플레이어 애니메이션을 관리하는 클래스
/// </summary>
public class SalmonAnimation : MonoBehaviour
{
    #region Components  
    public Animator anim { get; private set; }   
    #endregion
     
    #region States 
    public SalmonStateMachine stateMachine { get; private set; } // 플레이어의 상태를 관리하는 상태 머신
    public SalmonObject salmonObject { get; private set; } // 플레이어의 속성을 관리하는 객체 

    // 플레이어의 상태  
    public SalmonMoveState moveState { get; private set; }
    public SalmonIdleState idleState { get; private set; }

    //public SalmonStunState stunState { get; private set; }

    #endregion

    protected void Awake()
    {   
        // 상태 머신 인스턴스 생성
        stateMachine = new SalmonStateMachine();
        salmonObject = GetComponentInParent<SalmonObject>(); // 플레이어의 속성을 관리하는 객체 생성

        // 각 상태 인스턴스 생성 (this: 플레이어 객체, stateMachine: 상태 머신, "Idle"/"Move": 상태 이름) 
        moveState = new SalmonMoveState(this, stateMachine, salmonObject, "Move");
        idleState = new SalmonIdleState(this, stateMachine, salmonObject, "Idle");
        //stunState = new SalmonStunState(this, stateMachine, salmonObject, "Stun");
    }
    protected void Start()
    { 
        // 게임 시작 시 초기 상태를 대기 상태(idleState)로 설정
        stateMachine.Initialize(idleState); 
    } 
    protected void Update()
    { 
        stateMachine.currentState.Update();  
    }



}
