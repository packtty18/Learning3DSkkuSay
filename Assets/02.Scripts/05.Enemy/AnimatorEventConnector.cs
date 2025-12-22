using UnityEngine;

public class AnimatorEventConnector : MonoBehaviour
{
    [SerializeField] EnemyController _controller;

    public void AttackEvent()
    {
        _controller.Attack.Attack();
    }

    public void AnimationStart()
    {
        _controller.AnimationRunning = true;
    }

    public void AnimationEnd()
    {
        _controller.AnimationRunning = false;
    }
}
 