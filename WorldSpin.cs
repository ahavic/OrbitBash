using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldSpin : MonoBehaviour
{
    Transform thisTransform;
    [SerializeField] float spinSpeed = 1f;


    private void Awake()
    {
        thisTransform = GetComponent<Transform>(); 

    }

    // Update is called once per frame
    void Update()
    {
        thisTransform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }
}
