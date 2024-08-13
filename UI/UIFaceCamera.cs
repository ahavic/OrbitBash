using UnityEngine;
using Photon.Pun;
using System.Linq;

public class UIFaceCamera : MonoBehaviour
{
    private Camera cam;
    RectTransform thisTransform = null;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        thisTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lookDir = thisTransform.position + cam.transform.forward;

        thisTransform.LookAt(lookDir, cam.transform.up);
    }
}
