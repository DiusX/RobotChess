using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDir = Vector3.zero;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveDir.y = 1f;
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveDir.y = -1f;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveDir.x = -1f;
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveDir.x = 1f;

        transform.position += moveSpeed * Time.deltaTime * moveDir;

    }
}
