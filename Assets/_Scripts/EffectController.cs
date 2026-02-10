using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectController : MonoBehaviourPunCallbacks
{
    PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _photonView.RPC("DestroyWaitRPC", RpcTarget.All);
    }


    [PunRPC]
    void DestroyWaitRPC() => Destroy(gameObject, 1.0f);

}
