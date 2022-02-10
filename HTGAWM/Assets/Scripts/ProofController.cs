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
    private float range;    //증거 습득 가능한 최대 거리

    private bool pickupActivated = false; // 습득 가능할 시에 true

    //필요 컴포넌트
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

    private void OpenProofUI()
    {
        if (hitInfo.transform != null)
        {
            if (tempProofObject != null)
            {
                Destroy(tempProofObject);
            }
            pickupActivated = true;
            orbitCamera.m_Target = oldHitInfo.transform;
            proofDescription.text = oldHitInfo.transform.GetComponent<Proof>().proofDescription;
            Debug.Log(proofName + " 보기");
            SetLayersRecursively(oldHitInfo.transform, 8);
            playerController.FixPlayer();
            proofUI.gameObject.SetActive(true);
            collectButton.gameObject.SetActive(true);
        }
    }

    public void CloseProofUI()
    {
        if (tempProofObject != null)
        {
            Destroy(tempProofObject);
        }
        pickupActivated = false;
        if (oldHitInfo.transform != null)
        {
            SetLayersRecursively(oldHitInfo.transform, 7);
            Debug.Log(proofName + " 닫기");
        }
        playerController.UnfixPlayer();
        proofUI.gameObject.SetActive(false);
        proofRawImage.texture = proofRenderTexture;
        shareButton.gameObject.SetActive(false);
    }

    public void CollectProof()
    {
        if (pickupActivated)
        {
            if (oldHitInfo.transform != null)
            {
                Debug.Log("아이템 수집");
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

    public void OnClickCollectedSlot(BaseEventData data)
    {
        Debug.Log(data.selectedObject + "는 무엇인가");
        Slot targetSlot = data.selectedObject.transform.GetComponent<Slot>();

        if (targetSlot.proof.proofName.Length != 0 && !pickupActivated)
        {
            Debug.Log(targetSlot + "는 Slot이다.");
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
            Debug.Log(proofJson.proofName + " 보기");

            playerController.FixPlayer();
            collectButton.gameObject.SetActive(false);
            proofUI.gameObject.SetActive(true);
        }
    }

    public void OnClickShareProof()
    {
        Debug.Log("증거 공유");
        Application.ExternalCall("socket.emit", "SHARE_PROOF", JsonUtility.ToJson(proofJson));
    }

    public void ReceiveSharedProof(string str)
    {
        Debug.Log("공유받은 증거 보기");
        Proof.ProofJson receiveProof = JsonUtility.FromJson<Proof.ProofJson>(str);
        Debug.Log("공유받은 증거: " + receiveProof.sceneName + "/" + receiveProof.objectName);
        //sharedProofObject = GameObject.Instantiate(Resources.Load(receiveProof.sceneName + "/"+receiveProof.objectName)) as GameObject;
        LoadPrefabInTempProof(receiveProof.sceneName + "/" + receiveProof.objectName);
        tempProofObject.transform.position = new Vector3(0f, 0f, 0f);

        orbitCamera.m_Target = tempProofObject.transform;
        SetLayersRecursively(tempProofObject.transform, 8);
        proofDescription.text = tempProofObject.transform.GetComponent<Proof>().proofDescription;
        proofUI.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(false);
    }
}
