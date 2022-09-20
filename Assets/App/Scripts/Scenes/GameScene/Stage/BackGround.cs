using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 1;

    private Vector2 beforeCameraPos = Vector2.zero;
    private Transform cameraTrans = null;
    private Material backgroundMaterial = null;

    private void Awake()
    {
        backgroundMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        cameraTrans = Camera.main.transform;
        beforeCameraPos = cameraTrans.position;
    }

    private void LateUpdate()
    {
        Vector2 currentCameraPos = cameraTrans.position;
        Vector2 playerPosDiff = currentCameraPos - beforeCameraPos;

        Vector2 currentOffset = backgroundMaterial.mainTextureOffset;
        currentOffset += playerPosDiff * scrollSpeed;        

        beforeCameraPos = currentCameraPos;

        backgroundMaterial.mainTextureOffset = currentOffset;
    }
}
