using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeMove : MonoBehaviour
{
    [SerializeField]
    private GameObject slime;

    [SerializeField]
    private Vector2 startSlimePosition;

    //移動速度(0または0以下にはしない)
    [SerializeField]
    private float moveSpeed = default;

    //スライムの初期地点から左右に値の数だけ移動する
    [SerializeField]
    private Vector2 moveRange = default;

    //移動方向
    private Vector2 direction = new Vector2(1.0f,0);

    //初期地点に戻っているかどうか
    private bool isReturn = false;

    //MImikunと衝突しているかどうか
    private bool isCollision = false;

    void Start()
    {
        startSlimePosition = new Vector2(slime.transform.position.x, slime.transform.position.y); 
        
    }

    
    void Update()
    {
        //初期地点からmoveRange分だけ左に移動する
        if(slime.transform.localPosition.x >= startSlimePosition.x - moveRange.x　&& !isReturn && !isCollision)
        {
            LeftMove();
        }
        else if(!isCollision)
        {
            RightMove();
            isReturn = true;
            //初期地点からmoveRange分だけ右に移動したら初期地点に戻る
            if (slime.transform.localPosition.x >= startSlimePosition.x + moveRange.x)
            {
                isReturn = false;
            }
        }
    }

    void LeftMove()
    {
        //Debug.Log("左移動中");
        slime.transform.Translate(-direction * Mathf.Abs(moveSpeed) * Time.deltaTime);
    }

    void RightMove()
    {
        //Debug.Log("右移動中");
        slime.transform.Translate(direction * Mathf.Abs(moveSpeed) * Time.deltaTime);
    }

    //スライムと衝突時停止する
    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if(col.gameObject.name == "MimiKun")
        {
            //Debug.Log("<color=red>衝突</color>");
            isCollision = true;
        }
        
        
    }

    //スライムと離れたとき再度動く
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name == "MimiKun")
        {
            //Debug.Log("<color=green>離脱</color>");
            isCollision = false;
        }
    }
}
