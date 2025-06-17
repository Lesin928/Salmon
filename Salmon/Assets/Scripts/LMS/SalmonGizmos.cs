using UnityEngine;

/// <summary>
/// 연어의 이동 방향과 속도를 시각적으로 표시하는 Gizmos 클래스
/// </summary>
public class SalmonGizmos : MonoBehaviour
{ 
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnDrawGizmos()
    {
        // 캐릭터 전방 방향 (transform.forward)
        Gizmos.color = Color.cyan;
        Vector3 start = transform.position;
        Vector3 direction = transform.forward * 2f; // 길이 2의 방향 벡터
        Gizmos.DrawLine(start, start + direction);
        Gizmos.DrawSphere(start + direction, 0.1f); // 끝점에 구슬
     
        Vector3 start2 = transform.position;
        // 전방 방향 (W 입력 시 이동 예정 방향)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start2, start2 + transform.forward * 2f);
        Gizmos.DrawSphere(start2 + transform.forward * 2f, 0.1f);

        // 실제 이동 방향
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start2, start2 + rb.linearVelocity.normalized * 2f);
            Gizmos.DrawSphere(start2 + rb.linearVelocity.normalized * 2f, 0.1f);
        }
    }



}
