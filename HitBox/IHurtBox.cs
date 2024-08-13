using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.HitBox
{
    public interface IHurtBox
    {
        bool ApplyHurtbox(sHitboxData hit);
    }
}