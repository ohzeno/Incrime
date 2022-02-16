using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed = 3;
    [SerializeField]
    private float runSpeed = 5;
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;


    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    private bool isFixCamera = false;

    // 움직임 체크 변수
    private Vector3 lastPos;


    [SerializeField]
    private GameObject ProofUI;

    // 앉았을 때 얼마나 앉을지 결정하는 변수.
    [SerializeField]
    private float crouchPosY;
    private float crouchScaleY = 0.6f;
    private float originPosY;
    private float originScaleY;
    private float applyCrouchPosY;
    private float applyCrouchScaleY;

    // 땅 착지 여부
    private BoxCollider boxCollider;


    // 민감도
    [SerializeField]
    private float lookSensitivity;


    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;


    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    /*private CrossHair crossHair;*/
    /*private StatusController statusController;*/

    private Rigidbody myRigid;

    private Vector3 defaultPosAndAngle = new Vector3(0f, 0f, 0f);


    HashSet<string> movingScene = new HashSet<string>();

    private bool mouse = true;

    public static CursorLockMode currentCurrentLockMode = CursorLockMode.None;

    void Start()
    {
        movingScene.Add("BreakRoom");
        movingScene.Add("DirectorMaOffice");
        movingScene.Add("Hallway");
        movingScene.Add("MeetingRoom");
        movingScene.Add("OfficeCubicles");
        movingScene.Add("ReceptionDesk");
        movingScene.Add("RestRoom");
        movingScene.Add("SecurityRoom");
        movingScene.Add("TeamLeaderLeeOffice");
        boxCollider = GetComponent<BoxCollider>();
        myRigid = GetComponent<Rigidbody>();
        /*crossHair = FindObjectOfType<CrossHair>();*/
        /*statusController = FindObjectOfType<StatusController>();*/
        applySpeed = walkSpeed;

        // 초기화.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
        
        originScaleY = theCamera.transform.localScale.y;
        applyCrouchScaleY = originScaleY;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name+"씬이름");
        if (movingScene.Contains(scene.name))
        {
            myRigid.useGravity = true;
        }
        else
        {
            myRigid.useGravity = false;
        }
        transform.position = defaultPosAndAngle;
        currentCameraRotationX = 0;
        myRigid.MoveRotation(Quaternion.Euler(0, 0, 0));
    }


    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryFix();
        TryJump();
        TryCrouch();
        TryRun();
        MoveCheck();
        IsLockCursor();

        if (!isFixCamera)
        {
            CameraRotation();
            CharacterRotation();
        }
    }

    void IsLockCursor()
    {
        Cursor.lockState = currentCurrentLockMode;
    }

    void FixedUpdate()
    {
        if (!isFixCamera)
        {
            Move();
        }
    }

    public void FixPlayer()
    {
        isFixCamera = true;
        Debug.Log("카메라 고정"+isFixCamera);
    }

    public void UnfixPlayer()
    {
        isFixCamera = false;
    }

    private void TryFix()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isFixCamera = !isFixCamera;
            Debug.Log(Cursor.lockState);
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                PlayerController.currentCurrentLockMode = CursorLockMode.None;
            }
            else
            {
                PlayerController.currentCurrentLockMode = CursorLockMode.Locked;
            }
        }
    }


    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown("c"))
        {
            Crouch();
        }
    }


    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
            applyCrouchScaleY = crouchScaleY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
            applyCrouchScaleY = originScaleY;
        }

        StartCoroutine(CrouchCoroutine());

    }

    // 부드러운 동작 실행.
    IEnumerator CrouchCoroutine()
    {

        float _posY = theCamera.transform.localPosition.y;
        float _scaleY = myRigid.transform.localScale.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            _scaleY = Mathf.Lerp(_posY, applyCrouchScaleY, 0.2f);
            theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, _posY, theCamera.transform.localPosition.z);
            myRigid.transform.localScale = new Vector3(myRigid.transform.localScale.x, _scaleY, myRigid.transform.localScale.z);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
        myRigid.transform.localScale = new Vector3(myRigid.transform.localScale.x, applyCrouchScaleY, myRigid.transform.localScale.z);
    }


    // 지면 체크.
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, boxCollider.bounds.extents.y - boxCollider.center.y + 0.1f);
        //Debug.DrawRay(transform.position, Vector3.down * 
        //    (boxCollider.bounds.extents.y - boxCollider.center.y + 0.1f), Color.red);
        //Debug.Log("isGround값 : "+ transform.position);
    }


    // 점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround /*&& statusController.GetCurrentSP() > 0*/)
        {
            Jump();
        }
        
    }


    // 점프
    private void Jump()
    {

        // 앉은 상태에서 점프시 앉은 상태 해제.
        if (isCrouch)
            Crouch();
        /*statusController.DecreaseStamina(100);*/
        myRigid.velocity = transform.up * jumpForce;
    }


    // 달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) /*&& statusController.GetCurrentSP() > 0*/)
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) /*|| statusController.GetCurrentSP() <= 0*/)
        {
            RunningCancel();
        }
    }

    // 달리기 실행
    private void Running()
    {
        if (isCrouch)
            Crouch();

        isRun = true;
        /*statusController.DecreaseStamina(10);*/
        applySpeed = runSpeed;
    }


    // 달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }


    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void MoveCheck()
    {
        if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
            isWalk = true;
        else
            isWalk = false;
        /*crossHair.WalkingAnimation(isWalk);*/
        lastPos = transform.position;

    }


    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //Debug.Log(myRigid.rotation);
        //Debug.Log(myRigid.rotation.eulerAngles);
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
