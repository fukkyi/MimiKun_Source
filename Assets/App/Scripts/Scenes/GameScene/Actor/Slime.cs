using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField]
    private Collider2D bodyCollider = null;

    private Animator animator = null;
    private slimeMove slimeMove = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        slimeMove = GetComponent<slimeMove>();
    }

    /// <summary>
    /// ”š”j‚³‚¹‚é
    /// </summary>
    public void Explosion()
    {
        slimeMove.enabled = false;
        animator.SetTrigger("Explosion");

        bodyCollider.gameObject.layer = LayerTagUtil.LayerNumberDeadEnemy;

        Destroy(gameObject, 0.5f);
    }
}
