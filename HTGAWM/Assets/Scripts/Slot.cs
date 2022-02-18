using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    public Proof defaultProof;
    public Proof.ProofJson proof;   // 획득한 증거

    public RawImage proofRawImage; // 증거의 이미지


    void Start()
    {
        if(defaultProof != null)
            proof = new Proof.ProofJson(defaultProof);
    }

    private void SetColor(float _alpha)
    {
        Color color = proofRawImage.color;
        color.a = _alpha;
        proofRawImage.color = color;
    }


    //증거 추가
    public void AddProof(Proof _proof)
    {
        proof = new Proof.ProofJson(_proof);

        proofRawImage.texture = _proof.proofTexture;

        SetColor(1);
    }

    //슬롯에 증거 지우기
    private void RemoveProof()
    {
        proof = null;
        proofRawImage.texture = null;
        SetColor(0);
    }


   
}
