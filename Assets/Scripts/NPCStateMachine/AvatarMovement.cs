using Unity.VisualScripting;
using UnityEngine;

public class AvatarMovement : MonoBehaviour
{
    public Area area;
    public Actor actor;

    enum EState
    {
        Wandering,
        Waiting,
        Talking
    }

    EState state = EState.Wandering;

    [SerializeField] float maxWaitTime = 3f;
    private float waitTime = 0f;
    private void Start()
    {
        RandomizeState();   
    }

    private void Update()
    {
        if(state == EState.Waiting)
        {
            waitTime -= Time.deltaTime;
            if(waitTime < 0f)
            {
                ChangeState(EState.Wandering);
            }
        }
        else if(state == EState.Wandering)
        {
            if (HasArrived())
            {
                ChangeState(EState.Waiting);
            }
        }
    }

    public void RandomizeState()
    {
        if (Random.Range(0f, 100.0f) > 50f)
        {
            ChangeState(EState.Wandering);
        }
        else
        {
            ChangeState(EState.Waiting);
        }
    }

    void ChangeState(EState newState)
    {
        state = newState;

        if (state == EState.Wandering)
        {
            actor.agent.isStopped = false;
            SetRandomPosition();
        }
        else if (state == EState.Waiting) 
        {
            waitTime = maxWaitTime;
            actor.agent.isStopped = true;
        }
        else if (state == EState.Talking) 
        {
            actor.agent.isStopped = true;
        }

    }

    public void Talking()
    {
        ChangeState(EState.Talking);
    }

    bool HasArrived()
    {
        return actor.agent.remainingDistance <= actor.agent.stoppingDistance;
    }

    void SetRandomPosition()
    {
        actor.agent.SetDestination(area.GetRandomPoint());
    }
}
