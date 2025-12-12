using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float rotateSpeed = 10f;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void MoveWithRotation(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        //회전
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp( transform.rotation,targetRot, rotateSpeed * Time.deltaTime );
        }

        // 이동
        controller.Move(dir * speed * Time.deltaTime);
    }

    public void Knockback(Vector3 dir, float force)
    {
        controller.Move(dir * force * Time.deltaTime);
    }
}
