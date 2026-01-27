using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BulletController : MonoBehaviourPunCallbacks
{
    PhotonView _photonView;
    Vector3 _direction;


    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _photonView.RPC("DestroyWaitRPC", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction.normalized * 10 * Time.deltaTime);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    [PunRPC]
    void DestroyWaitRPC() => Destroy(gameObject,3.0f);

    [PunRPC]
    void DirRPC(Vector3 direction) => _direction = direction;

}
