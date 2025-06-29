using UnityEngine;

// TODO: (추가할일 적는부분)
// FIXME: (고칠거 적는부분)
// NOTE : (기타 작성)

/// <summary>
/// 연어의 이동 상태를 처리하는 클래스
/// </summary>
public class SalmonMoveState : SalmonState
{
    public SalmonMoveState(SalmonAnimation _salmonAnim, SalmonStateMachine _stateMachine, SalmonObject _salmonObject, string _animBoolName)
        : base(_salmonAnim, _stateMachine, _salmonObject, _animBoolName) { }
    public override void Enter()
    {
        base.Enter();
    } 
     
    public override void Update()
    { 
        base.Update(); 
    } 

    public override void Exit()
    {
        base.Exit();
    }
}
