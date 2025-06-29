using UnityEngine;

// TODO: (추가할일 적는부분)
// FIXME: (고칠거 적는부분)
// NOTE : (기타 작성)

/// <summary>
/// 연어의 상태를 나타내는 클래스 
/// </summary>
public class SalmonState
{
    #region Components  
    protected SalmonStateMachine stateMachine;
    protected SalmonAnimation salmonAnimation; 
    protected SalmonObject salmonObject;    
    protected SalmonController salmonController;     
    #endregion

    #region Variables // 플레이어의 상태를 나타내는 변수들
    private string animBoolName;
    protected float xInput;
    protected float yInput;   

    protected float stateTimer;
    protected bool triggerCalled;
    #endregion  

    public SalmonState(SalmonAnimation _salmonAnim, SalmonStateMachine _stateMachine, SalmonObject _salmonObject, string _animBoolName)
    {
        this.salmonAnimation = _salmonAnim;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
        this.salmonObject = _salmonObject;
    } 

    public virtual void Enter()
    {   /*
        playerAnimation.anim.SetBool(animBoolName, true);
        rb = playerAnimation.rb;
        triggerCalled = false; */
    }

    public virtual void Update()
    { /*
        //stateTimer -= Time.deltaTime; 
        playerAnimation.anim.SetFloat("yVelocity", rb.linearVelocityY); */
    }

    public virtual void Exit()
    {/*
        playerAnimation.anim.SetBool(animBoolName, false);*/
    }

    /// <summary>
    /// 애니메이션이 끝났을 때 호출되는 메셔드
    /// </summary>
    public virtual void AnimationFinishTrigger()
    {/*
        triggerCalled = true;*/
    }  
}
