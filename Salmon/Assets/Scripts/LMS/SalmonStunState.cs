using UnityEngine;

/// <summary>
/// 연어가 기절 상태에 있는 것을 나타내는 클래스
/// </summary>
public class SalmonStunState : SalmonState
{
    public SalmonStunState(SalmonAnimation _salmonAnim, SalmonStateMachine _stateMachine, SalmonObject _salmonObject, string _animBoolName)
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
