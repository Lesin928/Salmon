using UnityEngine;

// TODO: (추가할일 적는부분)
// FIXME: (고칠거 적는부분)
// NOTE : (기타 작성)

/// <summary>
/// 상태 머신을 관리하는 클래스
/// </summary>
public class SalmonStateMachine
{
    //현재의 상태를 나타내는 변수
    public SalmonState currentState { get; private set; }

    /// <summary>
    /// 상태 머신을 초기화하는 메서드
    /// </summary>
    /// <param name="_startState">초기 상태로 지정할 변수</param>
    public void Initialize(SalmonState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    /// <summary>
    /// 상태를 변경하는 메서드
    /// </summary>
    /// <param name="_newState">변경 할 상태 변수</param>
    public void ChangeState(SalmonState _newState)
    { 
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

}
