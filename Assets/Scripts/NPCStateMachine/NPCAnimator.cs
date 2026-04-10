using UnityEngine;

public class NPCAnimator : MonoBehaviour
{   
    public Animator animator;
    public Actor actor;
    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", actor.CurrentSpeed);
    }
}
