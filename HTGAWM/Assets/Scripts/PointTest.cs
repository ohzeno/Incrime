using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var controller = other.gameObject.GetComponent<SimpleCharacterController>();
        // 유저랑 충돌했을 경우에만 True
        // 다른 오브젝트면 null
        if(controller != null)
        {
            controller.score += 1;
            Destroy(gameObject);
        }
    }

}
