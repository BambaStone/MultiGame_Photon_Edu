using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    Vector3 move;
    public float MoveSpeed = 100f;
    public GameObject Head;
    public GameObject CameraPos;
    public GameObject Muzzle;
    public float MouseSensitivity = 100f;
    private bool onGround = false;
    private Rigidbody _rigidbody;
    private float _playerXRot = 0f;
    private float _playerYRot = 0f;
    private bool _run = false;


    public GameObject InfoWidget;
    public TMP_Text NickNameText;
    public Image CurrentHpImage;

    PhotonView _photonView;

    Vector3 _currentPos;
    Quaternion _currentRotation;
    Quaternion _currentHeadRotation;

    float _currentHp = 10f;
    float _maxHp = 10f;
    // Start is called before the first frame update


    void Start()
    {
        _photonView = GetComponent<PhotonView>();

        NickNameText.text = _photonView.IsMine ? PhotonNetwork.NickName : _photonView.Owner.NickName;
        NickNameText.color = _photonView.IsMine ? Color.blue : Color.red;
        CurrentHpImage.color = _photonView.IsMine ? Color.red : Color.magenta;
        
        _currentHp = _maxHp;
        CurrentHpImage.fillAmount = _currentHp / _maxHp;

        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = gameObject.GetComponent<Rigidbody>();

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
            Head.transform.rotation = _currentHeadRotation;
        }
        InfoWidget.transform.LookAt(Camera.main.transform.position);
    }

    private void FixedUpdate()
    {
        if (_run)
        {
            move = move * MoveSpeed * 2 * Time.deltaTime;
            _rigidbody.velocity = new Vector3(move.x, _rigidbody.velocity.y, move.z);

        }
        else
        {
            move = move * MoveSpeed * 2 * Time.deltaTime;
            _rigidbody.velocity = new Vector3(move.x, _rigidbody.velocity.y, move.z);
        }
    }

    void Move()
    {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
            _playerYRot += mouseX;
            _playerXRot -= mouseY;
            _playerXRot = Mathf.Clamp(_playerXRot, -90f, 90f);
            move = Vector3.zero;
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                Vector3 thisforward = gameObject.transform.forward;
                Vector3 thisright = gameObject.transform.right;
                thisforward.Normalize();
                thisright.Normalize();
                move = (thisforward * v + thisright * h).normalized;
            }
            else
            {
                move = Vector3.zero;
            }
            transform.rotation = Quaternion.Euler(0, _playerYRot, 0);
            Head.transform.localRotation = Quaternion.Euler(_playerXRot, 0, 0);
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _run = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _run = false;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                if (onGround)
                {
                    _rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
                }
            }
    }

    void Fire()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //머즐플래시
            PhotonNetwork.Instantiate("Muzzle_Flash", Muzzle.transform.position, Muzzle.transform.rotation);

            //ShotEffectSpawner.EffectActive();
            float rayDistance = 100f; // 레이의 최대 거리
            // 마우스의 스크린 좌표에서 화면 안쪽으로 뻗어나가는 레이를 생성
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit camHit;

            Vector3 targetPos;

            // 2. 화면(카메라)에서 마우스 방향으로 먼저 레이를 쏴서 충돌 지점 찾기
            if (Physics.Raycast(camRay, out camHit, rayDistance))
            {
                Debug.Log("Hit Object: " + camHit.collider.name);
                targetPos = camHit.point;// 충돌한 실제 3D 월드 지점
                
            }
            else
            {
                targetPos = camRay.GetPoint(100f);// 충돌한 게 없으면 마우스 방향으로 먼 지점을 목표로 설정
            }

            // 3. 특정 오브젝트(이 스크립트가 붙은 곳)에서 목표 지점 방향 계산
            Vector3 rayOrigin = transform.position; // 예: 총구 위치
            Vector3 direction = (targetPos - rayOrigin).normalized;

            RaycastHit hit;
            
            if (Physics.Raycast(rayOrigin, direction, out hit, rayDistance))
            {
                Debug.Log("맞은 물체: " + hit.collider.name);
                
                //히트이펙트
                PhotonNetwork.Instantiate("HitEffect", hit.point,Quaternion.identity);
                // hit.collider를 사용해 상호작용
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("HitRPC", RpcTarget.All);
                }
                
            }
        }
    }
    

    public void HitGun()
    {
        _currentHp--;
        CurrentHpImage.fillAmount = _currentHp / _maxHp;
        if (_currentHp <= 0)
        {
            float posX = Random.Range(-9.5f, 9.5f);
            float posZ = Random.Range(-14f, 14f);
            transform.position = new Vector3(posX, 4.5f, posZ);
            CurrentHpImage.fillAmount = 1f;
            _currentHp = _maxHp;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
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
            stream.SendNext(Head.transform.rotation);
        }
        if(stream.IsReading)
        {
            _currentPos = (Vector3)stream.ReceiveNext();
            _currentRotation = (Quaternion)stream.ReceiveNext();
            CurrentHpImage.fillAmount = (float)stream.ReceiveNext();
            _currentHeadRotation = (Quaternion)stream.ReceiveNext();

        }
    }


    [PunRPC]
    void HitRPC() => HitGun();

}
