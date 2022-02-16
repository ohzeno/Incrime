using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ProofController : MonoBehaviour
{
    private Sprite proofImage;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float range;    //���� ���� ������ �ִ� �Ÿ�

    private bool pickupActivated = false; // ���� ������ �ÿ� true

    //�ʿ� ������Ʈ
    [SerializeField]
    private Text proofName;

    [SerializeField]
    private Text proofDescription;

    [SerializeField]
    private Canvas proofUI;

    [SerializeField]
    private OrbitCamera orbitCamera;

    private Proof.ProofJson proofJson;

    [SerializeField]
    private RenderTexture proofRenderTexture;
    [SerializeField]
    private RawImage proofRawImage;

    [SerializeField]
    private Button collectButton;

    [SerializeField]
    private Button shareButton;

    [SerializeField]
    private Inventory inventory;

    private bool isFirstHit = true;
    private RaycastHit oldHitInfo;
    private RaycastHit hitInfo;
    private Outline outline;

    private GameObject tempProofObject;
    private GameObject sharedProofObject;


    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text actionText;
    private int isCoroutinesActive;
    private AudioSource musicPlayer;
    public AudioClip typingSound;
    private Coroutine cntCoroutine;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckProof();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckProof();
            OpenProofUI();
        }
    }

    private void CheckProof()
    {

        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(range * Vector3.forward), Color.red);
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (oldHitInfo.transform != null && oldHitInfo.transform != hitInfo.transform)
            {
                oldHitInfo.transform.GetComponent<Outline>().enabled = false;
                SetLayersRecursively(oldHitInfo.transform, 7);

            }
            if (hitInfo.transform.tag == "Proof")
            {
                oldHitInfo = hitInfo;
                ProofInfoAppear();
            }
        }
        else
        {
            InfoDisappear();
        }
    }

    IEnumerator _typing(string tmpstory, Text obj)
    {
        musicPlayer = GetComponent<AudioSource>();
        PlaySound(typingSound, musicPlayer);
        yield return new WaitForSeconds(0.2f);
        isCoroutinesActive = 1;
        for (int i = 0; i <= tmpstory.Length; i++)
        {
            obj.text = tmpstory.Substring(0, i);
            yield return new WaitForSeconds(0.05f);
        }
        musicPlayer.Stop();
    }

    public void PlaySound(AudioClip clip, AudioSource audioPlayer)
    {
        audioPlayer.Stop();
        audioPlayer.clip = clip;
        audioPlayer.loop = false;
        audioPlayer.time = 0;
        audioPlayer.Play();
    }
    private void OpenProofUI()
    {
        if (hitInfo.transform != null)
        {
            if (tempProofObject != null)
            {
                Destroy(tempProofObject);
            }
            pickupActivated = true;

            Debug.Log(proofName.text + " 보기");
            //orbitCamera.m_Target = oldHitInfo.transform;
            //SetLayersRecursively(oldHitInfo.transform, 8);

            LoadPrefabInTempProof(SceneManager.GetActiveScene().name + "/" + oldHitInfo.transform.gameObject.name);
            SetLayersRecursively(tempProofObject.transform, 8);
            tempProofObject.transform.position = new Vector3(0f, 0f, 0f);
            orbitCamera.m_Target = tempProofObject.transform;

            Proof tempProof = tempProofObject.GetComponent<Proof>();

            if (isCoroutinesActive == 1 && cntCoroutine != null)
            {
                Debug.Log("코루틴 종료");
                musicPlayer = GetComponent<AudioSource>();
                musicPlayer.Stop();
                StopCoroutine(cntCoroutine);
                proofDescription.text = tempProof.proofDescription;
            }
            else
            {

                proofDescription.text = "";

                cntCoroutine = StartCoroutine(_typing(tempProof.proofDescription, proofDescription));
            }
            playerController.FixPlayer();
            proofUI.gameObject.SetActive(true);
            // proofDescription.text = oldHitInfo.transform.GetComponent<Proof>().proofDescription;
            collectButton.gameObject.SetActive(true);
            PlayerController.currentCurrentLockMode = CursorLockMode.None;
        }
    }

    public void CloseProofUI()
    {
        isCoroutinesActive = 0;
        if (tempProofObject != null)
        {
            Destroy(tempProofObject);
        }
        pickupActivated = false;
        if (oldHitInfo.transform != null)
        {
            SetLayersRecursively(oldHitInfo.transform, 7);
            Debug.Log(proofName + " �ݱ�");
        }
        playerController.UnfixPlayer();

        if (isCoroutinesActive == 1 && cntCoroutine != null)
        {
            musicPlayer = GetComponent<AudioSource>();
            musicPlayer.Stop();
            StopCoroutine(cntCoroutine);
        }
        proofUI.gameObject.SetActive(false);
        proofRawImage.texture = proofRenderTexture;
        shareButton.gameObject.SetActive(false);
        sharingUserObject.SetActive(false);
        // ui, close it and hold the mouse
        PlayerController.currentCurrentLockMode = CursorLockMode.Locked;
    }

    public void CollectProof()
    {
        if (pickupActivated)
        {
            if (oldHitInfo.transform != null)
            {
                Debug.Log("증거 수집 호출");
                Proof dest = oldHitInfo.transform.GetComponent<Proof>();
                dest.proofTexture = new Texture2D(proofRenderTexture.width, proofRenderTexture.height, TextureFormat.RGBA32, false);
                dest.proofTexture.Apply(false);
                Graphics.CopyTexture(proofRenderTexture, dest.proofTexture);
                inventory.AcquireProof(oldHitInfo.transform.GetComponent<Proof>());
            }
        }
    }

    private void ProofInfoAppear()
    {
        outline = hitInfo.transform.GetComponent<Outline>();
        outline.enabled = true;
        actionText.gameObject.SetActive(true);
        actionText.text = proofName.text = hitInfo.transform.GetComponent<Proof>().proofName;
    }

    private void InfoDisappear()
    {
        if (outline != null)
            outline.enabled = false;
        actionText.gameObject.SetActive(false);
    }

    public void SetLayersRecursively(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
        {
            SetLayersRecursively(child, layer);
        }
    }

    public void LoadPrefabInTempProof(string path)
    {
        if(tempProofObject != null)
        {
            Destroy(tempProofObject);
        }
        tempProofObject = GameObject.Instantiate(Resources.Load(path)) as GameObject;
        tempProofObject.layer = 9;
    }

    [SerializeField]
    private GameObject sharingUserObject;
    public void OnClickCollectedSlot(BaseEventData data)
    {
        Debug.Log(data.selectedObject + "의 타입 == Slot");
        Slot targetSlot = data.selectedObject.transform.GetComponent<Slot>();

        sharingUserObject.SetActive(false);

        if (targetSlot.proof.proofName.Length != 0 && !pickupActivated)
        {
            Debug.Log(targetSlot + "= Slot");
            if (SceneManager.GetActiveScene().name == "MeetingScene")
            {
                shareButton.gameObject.SetActive(true);
            }
            else
            {
                shareButton.gameObject.SetActive(false);
            }
            proofJson = targetSlot.proof;
            Debug.Log(targetSlot.proof.sceneName + "/" + targetSlot.proof.objectName);

            LoadPrefabInTempProof(targetSlot.proof.sceneName + "/" + targetSlot.proof.objectName);

            tempProofObject.transform.position = new Vector3(0f, 0f, 0f);
            orbitCamera.m_Target = tempProofObject.transform;
            SetLayersRecursively(tempProofObject.transform, 8);
            
            proofDescription.text = proofJson.proofDescription;
            Debug.Log(proofJson.proofName + " ����");

            playerController.FixPlayer();
            collectButton.gameObject.SetActive(false);
            proofUI.gameObject.SetActive(true);
        }
    }

    public void OnClickShareProof()
    {
        Debug.Log("증거 공유");
        proofJson.foundUserName = Client.role;
        Application.ExternalCall("socket.emit", "SHARE_PROOF", JsonUtility.ToJson(proofJson), Client.room );
    }

    [SerializeField]
    private Text sharingUserNameText;
    public void ReceiveSharedProof(string str)
    {
        Debug.Log("증거 정보 유니티 수신");
        Proof.ProofJson receiveProof = JsonUtility.FromJson<Proof.ProofJson>(str);
        Debug.Log("증거 프리팹 경로: " + receiveProof.sceneName + "/" + receiveProof.objectName);
        //sharedProofObject = GameObject.Instantiate(Resources.Load(receiveProof.sceneName + "/"+receiveProof.objectName)) as GameObject;
        LoadPrefabInTempProof(receiveProof.sceneName + "/" + receiveProof.objectName);
        tempProofObject.transform.position = new Vector3(0f, 0f, 0f);

        orbitCamera.m_Target = tempProofObject.transform;
        SetLayersRecursively(tempProofObject.transform, 8);
        proofDescription.text = tempProofObject.transform.GetComponent<Proof>().proofDescription;

        sharingUserNameText.text = receiveProof.foundUserName;

        proofUI.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(false);
    }
}
