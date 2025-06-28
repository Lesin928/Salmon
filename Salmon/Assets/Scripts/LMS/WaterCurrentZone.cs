using UnityEngine;

public class WaterCurrentZone : MonoBehaviour
{
    [SerializeField] private Vector3 currentDirection = Vector3.zero;
    [SerializeField] private float strength = 0f;

    private void OnTriggerStay(Collider other)
    {
        // 레이어가 WaterCurrent인 경우에만 처리
        //if (other.gameObject.layer != LayerMask.NameToLayer("WaterCurrentLayer")) return;
        SalmonObject salmon = other.GetComponent<SalmonObject>();
        if (salmon != null)
        {
            Vector3 current = currentDirection.normalized * strength;
            salmon.ApplyWaterCurrent(current);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 레이어가 WaterCurrent인 경우에만 처리
        //if (other.gameObject.layer != LayerMask.NameToLayer("WaterCurrentLayer")) return;
        SalmonObject salmon = other.GetComponent<SalmonObject>();
        if (salmon != null)
        {
            salmon.ApplyWaterCurrent(Vector3.zero);
        }
    }

    private void Start()
    {
        // 무조건 벡터의 y값은 0으로 설정
        if (currentDirection.y != 0f)
        {
            currentDirection.y = 0f;         
        }    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Vector3 direction = transform.rotation * currentDirection.normalized * strength;
        Gizmos.DrawRay(origin, direction);
        Gizmos.DrawSphere(origin + direction, strength * 0.1f);
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
