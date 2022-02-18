using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // 버튼 사용하므로.

public class GameHint : MonoBehaviour
{
    public GameObject Hint; // 힌트 이미지
    public Button btnHint;  // 힌트 보기 버튼

    // Start is called before the first frame update
    void Start()
    {
        // 시작할 때에는 힌트 보이지 않게 설정
        Hint.SetActive(false);

        // 힌트 버튼 클릭했을 때 ShowHint 함수 실행
        btnHint.onClick.AddListener(ShowHint);
    }

    void ShowHint()
    {
        // 힌트가 보이게 한다
        Hint.SetActive(true);
        // 2초 뒤에 힌트가 사라지게 한다. (Invoke의 첫 번째 인자는 함수명)
        Invoke("HideHint", 3);
    }

    // 함수명이 필요하기 때문에 HideHint라는 함수를 하나 만들어둔다.
    void HideHint()
    {
        Hint.SetActive(false);
    }
}