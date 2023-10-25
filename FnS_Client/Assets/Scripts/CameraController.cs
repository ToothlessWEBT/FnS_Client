using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _singleton;

    public static CameraController Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(CameraController)} instance already exists");
                Destroy(value);
            }
        }
    }

    private Camera ccamera;

    private void Awake()
    {
        Singleton = this;

        ccamera = GetComponent<Camera>();
    } 

    public Camera GetCamera() => ccamera;

    public void SetCameraPos(Vector3 pos)
    {
        transform.position = pos;
    }   
}
