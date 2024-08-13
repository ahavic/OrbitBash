using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.HitBox
{
    public class HitBoxData : MonoBehaviour
    {
        public sHitboxData hbData;
        Action<int> HitboxEvent = delegate { };

        #region Hitbox Event mutators
        public void AddHitboxEvent(Action<int> e)
        {
            if(!HitboxEvent.GetInvocationList().Contains(e))
                HitboxEvent += e;
        }

        public void RemoveHitboxEvent(Action<int> e)
        {
            if (HitboxEvent.GetInvocationList().Contains(e))
                HitboxEvent -= e;
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            print(gameObject.name + " collided with: " + other.name);

            if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                print("environment coll\n" + other.name);
                HitboxEvent(hbData.id);
                gameObject.SetActive(false);
                return;
            }

            IHurtBox hurtBox = other.GetComponent<IHurtBox>();

            if (hurtBox != null)
            {
                if (hurtBox.ApplyHurtbox(hbData))
                {
                    print("hit another player");
                    HitboxEvent(hbData.id);
                    gameObject.SetActive(false);
                }

                return;
            }         
        }
    }
}