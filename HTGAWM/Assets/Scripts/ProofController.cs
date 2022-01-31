using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProofController : MonoBehaviour
{
    private Sprite proofImage;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float range;    //증거 습득 가능한 최대 거리

    [SerializeField]
    private Text proofName;

    [SerializeField]
    private Text proofDescription;

    [SerializeField]
    private Canvas proofUI;

    [SerializeField]
    private OrbitCamera orbitCamera;

    [SerializeField]
    private RawImage proofRawImage;

    private bool pickupActivated = false;

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
        if(hitInfo.transform != null)
        {
            proofDescription.text = oldHitInfo.transform.GetComponent<Proof>().proofDescription;
            Debug.Log(proofName + " 보기");
            SetLayersRecursively(oldHitInfo.transform, 8);
            playerController.FixPlayer();
            proofUI.gameObject.SetActive(true);
        }
    }

    public void CloseProofUI()
    {
        if (oldHitInfo.transform != null)
        {
            SetLayersRecursively(oldHitInfo.transform, 7);
            Debug.Log(proofName + " 닫기");
        }
        playerController.UnfixPlayer();
        proofUI.gameObject.SetActive(false);
        
    }

    private void ProofInfoAppear()
    {
        outline = hitInfo.transform.GetComponent<Outline>();
        outline.enabled = true;
        orbitCamera.m_Target = hitInfo.transform;
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
}
