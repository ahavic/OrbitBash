using Scripts.HitBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Character
{
    public class BarrellRollReflection : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            print("trigger enter\n" + gameObject.name);
            if (other.gameObject.layer == LayerMask.NameToLayer("ProjectileLayer"))
            {
                other.gameObject.GetComponent<IHurtBox>()?.ApplyHurtbox(new sHitboxData());
            }
        }
    }
}