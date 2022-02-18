using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    bool isFire;
    Vector3 direction;
    public float speed = 10f;

    public void Fire(Vector3 dir)
    {
        direction = dir;
        isFire = true;
        // 발사 10초 후 파괴
        Destroy(gameObject, 10f);
    }

    void Start()
    {

    }

    void Update()
    {
        // isFire가 True인 경우만 direction 방향으로 초당 speed 거리 날아가도록
        if (isFire)
        {
            transform.Translate(direction * Time.deltaTime * speed);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // SimpleCharacterController를 가져와서 컴포넌트가 유효하면
        var controller = collision.collider.GetComponent<SimpleCharacterController>();
        if(controller != null)
        {
            // 체력 -1
            controller.hp -= 1;
        }
        // 플레이어 아니더라도 파괴
        Destroy(gameObject);
    }
}
