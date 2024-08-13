using Scripts.Character;
using Scripts.Character.Movement;
using UnityEngine;

namespace Scripts.Character.Movement
{
    public class Manuever : StateMachineBehaviour
    {
        CharacterMovement movement = null;
        [SerializeField] string ManueverBoolAnimName = "";
        int animBool;
        [SerializeField] eManuevers manuever = eManuevers.idle;
        [SerializeField] float energyExpense = 2f;


        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animBool = Animator.StringToHash(ManueverBoolAnimName);
            movement = animator.GetComponentInParent<CharacterMovement>();
            movement.BeginManuever(manuever);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            movement.ExpendEnergy(energyExpense);
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            movement.EndManuever(manuever);
            if (!string.IsNullOrEmpty(ManueverBoolAnimName))
                animator.SetBool(animBool, false);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}