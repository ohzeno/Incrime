using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseProofUI : MonoBehaviour
{
    [SerializeField]
    private ProofController proofController;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCloseProofUI()
    {
        proofController.CloseProofUI();
    }
}
