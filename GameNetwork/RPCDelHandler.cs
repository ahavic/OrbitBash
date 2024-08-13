using UnityEngine;
using Photon.Pun;
using System;
using Scripts.Projectiles;
using Scripts.Character;

public class RPCDelHandler : MonoBehaviourPun
{

    [SerializeField] ProjectileFire[] missileFires = null;
    [SerializeField] LaserFire laserFire = null;
    [SerializeField] ProjectileFire bombFire = null;
    [SerializeField] PlayerController playerController = null;
    
    Action<int> firing;

    private void Awake()
    {
        foreach (var fire in missileFires)
        {
            firing += fire.Fire;
        }
    }

    #region Fire RPC

    [PunRPC]
    public void FireMissileRPC()
    {
        firing(photonView.Owner.ActorNumber);
    }

    [PunRPC]
    public void FireBombRPC()
    {
        bombFire.Fire(photonView.Owner.ActorNumber);
    }

    [PunRPC]
    public void FireLaserRPC(Vector3 target)
    {
        laserFire.Fire(photonView.Owner.ActorNumber, target);
    }
    #endregion

    [PunRPC]
    public void ApplyDamageRPC(float damage, int id)
    {
        playerController.ApplyDamage(damage, id);
    }

    [PunRPC]
    public void RespawnRPC()
    {
        playerController.Respawn();
    }

    [PunRPC]
    public void AddKillRPC()
    {
        playerController.PlayerStats.AddKill();
    }
    public void AddDeathRPC()
    {
        playerController.PlayerStats.AddDeath();
    }
}
