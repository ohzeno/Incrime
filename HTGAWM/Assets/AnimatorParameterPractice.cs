using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterPractice : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // 게임 오브젝트에 붙어있는 애니메이터 컴포넌트를 가져옴
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        // F키를 누르면 New Float의 값을 3.1로 변경
        if(Input.GetKeyDown(KeyCode.F))
        {
            animator.SetFloat("New Float", 3.1f);
        }

        // F키를 누르면 New Float의 값을 2.9로 변경
        if(Input.GetKeyUp(KeyCode.F))
        {
            animator.SetFloat("New Float", 2.9f);
        }

        // I키를 누르면 New Int의 값을 1로 변경
        if(Input.GetKeyDown(KeyCode.I))
        {
            animator.SetInteger("New Int", 1);
        }

        // I키를 누르면 New Int의 값을 0로 변경
        if(Input.GetKeyUp(KeyCode.I))
        {
            animator.SetInteger("New Int", 0);
        }

        // B키를 누르면 New Bool의 값을 true로 변경
        if(Input.GetKeyDown(KeyCode.B))
        {
            animator.SetBool("New Bool", true);
        }

        // B키를 누르면 New Bool의 값을 false로 변경
        if (Input.GetKeyUp(KeyCode.B))
        {
            animator.SetBool("New Bool", false);
        }

        // T키를 누르면 New Trigger 신호
        if(Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("New Trigger");
        }
    }
}
