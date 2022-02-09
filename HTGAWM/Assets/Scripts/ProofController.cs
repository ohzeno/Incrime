using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

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

    private Proof proof;

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
            if(oldHitInfo.transform != null && oldHitInfo.transform != hitInfo.transform)
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
            if(oldHitInfo.transform != null)
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

    public void OnClickCollectedSlot(BaseEventData data)
    {
        Debug.Log(data.selectedObject + "는 무엇인가");
        Slot targetSlot = data.selectedObject.transform.GetComponent<Slot>();

        if (targetSlot.proof != null)
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
            proof = targetSlot.proof;
            Debug.Log(targetSlot.proof.GetSceneName() + "/" + targetSlot.proof.GetObjectName());
            GameObject tempProof = GameObject.Instantiate(Resources.Load(proof.GetSceneName() + "/" + proof.GetObjectName())) as GameObject;
            tempProof.transform.position = new Vector3(0f, 0f, 0f);
            orbitCamera.m_Target = tempProof.transform;
            SetLayersRecursively(tempProof.transform, 8);
            
            proofDescription.text = proof.proofDescription;
            Debug.Log(proof.proofName + " 보기");
            playerController.FixPlayer();
            collectButton.gameObject.SetActive(false);
            proofUI.gameObject.SetActive(true);
        }
    }
    [Serializable]
    class ProofJson
    {
        public string no;

        public string proofName;

        public string proofDescription;

        public string objectName;
        public string sceneName;

        public ProofJson()
        {

        }

        public ProofJson(Proof _proof)
        {
            no = _proof.no;
            proofName = _proof.proofName;
            proofDescription = _proof.proofDescription;
            sceneName = _proof.GetSceneName();
            objectName = _proof.GetObjectName();
        }
    }

    public void OnClickShareProof()
    {
        Debug.Log("증거 공유");
        ProofJson proofJson = new ProofJson(proof);
        Application.ExternalCall("socket.emit", "SHARE_PROOF", JsonUtility.ToJson(proofJson));
    }

    public void ReceiveSharedProof(string str)
    {
        Debug.Log("공유받은 증거 보기");
        ProofJson receiveProof = JsonUtility.FromJson<ProofJson>(str);
        Debug.Log("공유받은 증거: " + receiveProof.sceneName + "/" + receiveProof.objectName);
        GameObject tempProof = GameObject.Instantiate(Resources.Load(receiveProof.sceneName + "/"+receiveProof.objectName)) as GameObject;
        tempProof.transform.position = new Vector3(0f, 0f, 0f);

        orbitCamera.m_Target = tempProof.transform;
        SetLayersRecursively(tempProof.transform, 8);
        proofDescription.text = tempProof.transform.GetComponent<Proof>().proofDescription;
        proofUI.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(false);
    }
}
