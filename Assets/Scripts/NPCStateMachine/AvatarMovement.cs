using Unity.VisualScripting;
using UnityEngine;

public class AvatarMovement : MonoBehaviour
{
    public Area area;
    public Actor actor;
    public Transform player;
    [SerializeField] Actor otherNPC;
    enum EState
    {
        Wandering,
        Waiting,
        Talking,
        MeetUp
    }

    EState state = EState.Wandering;

    [SerializeField] float maxWaitTime = 3f;
    private float waitTime = 0f;
    private void Start()
    {
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
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
        else if (state == EState.MeetUp)
        {
            if (HasArrived()) 
            {
                ChangeState(EState.Talking);
                return;
            }

            ChangeState(EState.MeetUp);
            
        }
        else if (state == EState.Talking)
        {
            FacePlayer();
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
            actor.agent.updateRotation = true;
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
            actor.agent.updateRotation = false;
        }
        else if(state == EState.MeetUp)
        {
            if(otherNPC != null)
            {
                otherNPC.agent.SetDestination(new Vector3(transform.position.x + 0.75f, transform.position.y, transform.position.z));
            }
        }
    }

    public void Talking()
    {
        ChangeState(EState.Talking);
        
        
    }

    public void Walking()
    {
        ChangeState(EState.Wandering);
    }

    public void MeetUp()
    {
        ChangeState(EState.MeetUp);
    }

    bool HasArrived()
    {
        return actor.agent.remainingDistance <= actor.agent.stoppingDistance;
    }

    void SetRandomPosition()
    {
        actor.agent.SetDestination(area.GetRandomPoint());
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // keep only horizontal rotation

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
