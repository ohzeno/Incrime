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
    private Camera proofCamera;
    [SerializeField]
    private OrbitCamera orbitCamera;

    [SerializeField]
    private RawImage proofRawImage;

    private bool pickupActivated = false;

    private RaycastHit hitInfo;
    private GameObject proofObject;
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
    void FixedUpdate()
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
            if (hitInfo.transform.tag == "Proof")
            {
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
            proofObject = hitInfo.transform.gameObject;
            Debug.Log(proofName + " 보기");
            proofObject.layer = 8;
            playerController.FixPlayer();
            proofUI.gameObject.SetActive(true);
        }
    }

    public void CloseProofUI()
    {
        if (proofObject != null)
        {
            proofObject.layer = 7;
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
}
