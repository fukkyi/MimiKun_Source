using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeMove : MonoBehaviour
{
    [SerializeField]
    private GameObject slime;

    [SerializeField]
    private Vector2 startSlimePosition;

    //�ړ����x(0�܂���0�ȉ��ɂ͂��Ȃ�)
    [SerializeField]
    private float moveSpeed = default;

    //�X���C���̏����n�_���獶�E�ɒl�̐������ړ�����
    [SerializeField]
    private Vector2 moveRange = default;

    //�ړ�����
    private Vector2 direction = new Vector2(1.0f,0);

    //�����n�_�ɖ߂��Ă��邩�ǂ���
    private bool isReturn = false;

    //MImikun�ƏՓ˂��Ă��邩�ǂ���
    private bool isCollision = false;

    void Start()
    {
        startSlimePosition = new Vector2(slime.transform.position.x, slime.transform.position.y); 
        
    }

    
    void Update()
    {
        //�����n�_����moveRange���������Ɉړ�����
        if(slime.transform.localPosition.x >= startSlimePosition.x - moveRange.x�@&& !isReturn && !isCollision)
        {
            LeftMove();
        }
        else if(!isCollision)
        {
            RightMove();
            isReturn = true;
            //�����n�_����moveRange�������E�Ɉړ������珉���n�_�ɖ߂�
            if (slime.transform.localPosition.x >= startSlimePosition.x + moveRange.x)
            {
                isReturn = false;
            }
        }
    }

    void LeftMove()
    {
        //Debug.Log("���ړ���");
        slime.transform.Translate(-direction * Mathf.Abs(moveSpeed) * Time.deltaTime);
    }

    void RightMove()
    {
        //Debug.Log("�E�ړ���");
        slime.transform.Translate(direction * Mathf.Abs(moveSpeed) * Time.deltaTime);
    }

    //�X���C���ƏՓˎ���~����
    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if(col.gameObject.name == "MimiKun")
        {
            //Debug.Log("<color=red>�Փ�</color>");
            isCollision = true;
        }
        
        
    }

    //�X���C���Ɨ��ꂽ�Ƃ��ēx����
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name == "MimiKun")
        {
            //Debug.Log("<color=green>���E</color>");
            isCollision = false;
        }
    }
}
