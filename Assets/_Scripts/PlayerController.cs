using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject InfoWidget;
    public TMP_Text NickNameText;
    public Image CurrentHpImage;

    PhotonView _photonView;
    Animator _animator;

    Vector3 _currentPos;
    Quaternion _currentRotation;

    float _currentHp = 10f;
    float _maxHp = 10f;
    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();

        NickNameText.text = _photonView.IsMine ? PhotonNetwork.NickName : _photonView.Owner.NickName;
        NickNameText.color = _photonView.IsMine ? Color.blue : Color.red;
        CurrentHpImage.color = _photonView.IsMine ? Color.red : Color.magenta;
        
        _currentHp = _maxHp;
        CurrentHpImage.fillAmount = _currentHp / _maxHp;

    }

    // Update is called once per frame
    void Update()
    {
        if(_photonView.IsMine)
        {
            Move();
            Fire();
        }
        else
        {
            transform.position = _currentPos;
            transform.rotation = _currentRotation;
        }
        InfoWidget.transform.LookAt(Camera.main.transform.position);
    }

    void Move()
    {
        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(h,0,v);

            transform.rotation = Quaternion.LookRotation(movement);

            transform.Translate(Vector3.forward *3*Time.deltaTime);
            _animator.SetBool("isMove", true);
        }
        else
        {
            _animator.SetBool("isMove", false);
        }
    }

    void Fire()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("Attack");

            GameObject go = PhotonNetwork.Instantiate("Bullet", transform.position, Quaternion.identity);
            go.GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, transform.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet") && _photonView.IsMine && other.GetComponent<PhotonView>().IsMine==false)
        {
            _currentHp--;
            CurrentHpImage.fillAmount = _currentHp / _maxHp;

            other.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.AllBuffered);
            if(_currentHp <=0)
            {
                float posX = Random.Range(-10f, 10f);
                float posZ = Random.Range(-10f, 10f);
                transform.position = new Vector3(posX, 0, posZ);
                CurrentHpImage.fillAmount = 1f;
                _currentHp = _maxHp;
            }
        }
    }

    //주고받을정보를 전달해주는 함수 IPunObservable 상속시 인터페이스 구현하여 생성
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CurrentHpImage.fillAmount);
        }
        if(stream.IsReading)
        {
            _currentPos = (Vector3)stream.ReceiveNext();
            _currentRotation = (Quaternion)stream.ReceiveNext();
            CurrentHpImage.fillAmount = (float)stream.ReceiveNext();

        }
    }
}
