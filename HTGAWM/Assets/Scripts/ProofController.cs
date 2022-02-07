using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [SerializeField]
    private RenderTexture proofRenderTexture;
    [SerializeField]
    private RawImage proofRawImage;

    [SerializeField]
    private Button collectButton;

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
        collectButton.gameObject.SetActive(true);

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

    public void OnClickedCollectedSlot(BaseEventData data)
    {
        Debug.Log(data.selectedObject + "는 무엇인가");
        Slot targetSlot = data.selectedObject.transform.GetComponent<Slot>();

        if (targetSlot.proof != null)
        {
            Debug.Log(targetSlot + "는 Slot이다.");
            proofRawImage.texture = targetSlot.proof.proofTexture;
            proofDescription.text = targetSlot.proof.proofDescription;
            Debug.Log(targetSlot.proof.proofName + " 보기");
            playerController.FixPlayer();
            collectButton.gameObject.SetActive(false);
            proofUI.gameObject.SetActive(true);
        }
    }

    public void ReceiveSharedProof(Transform _transform)
    {
        Debug.Log("공유받은 증거 보기");
        orbitCamera.m_Target = _transform;
        SetLayersRecursively(_transform, 8);
        proofDescription.text = _transform.GetComponent<Proof>().proofDescription;
        proofUI.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(false);
    }
}
