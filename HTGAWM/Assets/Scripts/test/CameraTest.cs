using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    float mouseSpeed = 3;
    float mouseY = 0;
    float mouseX = 0;
    bool flag = true;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        mouseX += Input.GetAxis("Mouse X") * 3;

        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        mouseY = Mathf.Clamp(mouseY, -55.0f, 55.0f);

        transform.localEulerAngles = new Vector3(-mouseY, mouseX, 0);

        TryFix();
    }

    private void TryFix()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            flag = !flag;
            if (flag)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            Debug.Log("tap´©¸§" + Cursor.lockState + " " + flag);

        }
    }
}
