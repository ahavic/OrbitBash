using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Scripts.Character.Movement;

public class LagCompensation : MonoBehaviourPun, IPunObservable
{

    protected CharacterMovement movement;
    protected Vector3 remotePlayerPosition;
    protected Vector3 remotePlayerLookDirection;
    protected Vector3 lookDirection;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (photonView.IsMine)
            return;

        var lagDistance = remotePlayerPosition - transform.position;
        if (lagDistance.magnitude > 5f)
        {
            transform.position = remotePlayerPosition;
            lagDistance = Vector3.zero;
        }

        if (lagDistance.magnitude < 0.11f)
        {
            movement.OrbitMovement(0);
        }
        else
        {
            transform.position = remotePlayerPosition;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(lookDirection);
        }
        else
        {
            remotePlayerPosition = (Vector3)stream.ReceiveNext();
            remotePlayerLookDirection = (Vector3)stream.ReceiveNext();
        }
    }
}