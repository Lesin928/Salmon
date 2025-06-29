using UnityEngine;

/// <summary>
/// 연어가 물에 있을 때의 상태를 나타내는 클래스
/// </summary>
public class SalmonWaterState : SalmonState
{
    public SalmonWaterState(SalmonAnimation _salmonAnim, SalmonStateMachine _stateMachine, SalmonObject _salmonObject, string _animBoolName)
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