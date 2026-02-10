using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //닉네임입력
    public TMP_InputField NickNameInput;
    //접속버튼
    public Button ConnectButton;
    //접속패널
    public GameObject ConnectPannel;
    //에임UI
    public GameObject aimUI;
    // Start is called before the first frame update
    void Start()
    {
        ConnectButton.onClick.AddListener(
            () =>
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            );
    }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 128;
        PhotonNetwork.SerializationRate = 128;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }


    public void SpawnPlayer()
    {
        float posX = Random.Range(-9.5f, 9.5f);
        float posZ = Random.Range(-14f, 14f);
        //포톤네트워크의 인스탄티에이트 = Resources폴더의 "Player"라는 이름의 오브젝트를 소환
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(posX, 4.5f, posZ), Quaternion.identity);
        CameraController_0210 cc = Camera.main.GetComponent<CameraController_0210>();
        cc.Target = player.GetComponent<PlayerController>().CameraPos;
    }

    //포톤의 마스터 서버 접속시 호출
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 },null);

    }

    //룸에 접속시 호출
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        ConnectPannel.SetActive(false);
        aimUI.SetActive(true);
        StartCoroutine(CoDestroyBullet());
        SpawnPlayer();
    }

    IEnumerator CoDestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            go.GetComponent<PhotonView>().RPC("DestroyRPC",RpcTarget.All);
        }
    }


    //연결 해제시 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ConnectPannel.SetActive(true);

    }
}
