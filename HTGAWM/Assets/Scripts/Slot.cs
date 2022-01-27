using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Proof proof;   // 획득한 증거

    public Image proofImage; // 증거의 이미지



    private void SetColor(float _alpha)
    {
        Color color = proofImage.color;
        color.a = _alpha;
        proofImage.color = color;
    }


    //증거 추가
    public void AddProof(Proof _proof)
    {
        proof = _proof;
        proofImage.sprite = _proof.proofImage;

        SetColor(1);

    }

    //슬롯에 증거 지우기
    private void RemoveProof()
    {
        proof = null;
        proofImage.sprite = null;
        SetColor(0);
    }


   
}
