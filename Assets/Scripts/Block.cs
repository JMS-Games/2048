using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int startNum = 2;
    public float speed;

    [SerializeField]
    private Text curNum;
    private bool isMove;
    private bool isLive;
    private Rigidbody2D rigid;
    private GameObject moveTarget;

    void Awake()
    {
        Init();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        curNum.text = startNum.ToString();
    }

    public void Init()
    {
        curNum = transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        curNum.text = startNum.ToString();
        isMove = false;
        isLive = true;
        StartCoroutine(OpenAnim());
    }

    IEnumerator OpenAnim()
    {
        transform.localScale = Vector3.zero;
        Vector3 addVec = new Vector3(0.1f, 0.1f, 0.1f);
        while(transform.localScale != Vector3.one)
        {
            transform.localScale += addVec;
            addVec += new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSeconds(0.03f);
        }
    }

    public int GetNum()
    {
        return int.Parse(curNum.text);
    }

    public void Merge()
    {
        int newScore = GetNum() * 2;
        curNum.text = newScore.ToString();
        if (GameManager.instance.GetScore() < newScore)
        {
            if(newScore >= 10000)
            {
                curNum.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            }
            else if (newScore >= 1000)
            {
                curNum.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (newScore >= 100)
            {
                curNum.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            }
            GameManager.instance.SetScore(newScore);
        }

        // todo Merging effect anim??
    }

    void FixedUpdate()
    {
        rigid.velocity = Vector2.zero;
        if (!isMove || moveTarget == null)
            return;
        Vector2 moveVec = new Vector2(moveTarget.transform.position.x - transform.position.x, moveTarget.transform.position.y - transform.position.y);
        rigid.MovePosition(rigid.position + moveVec * speed * Time.fixedDeltaTime);

        if ((transform.position - moveTarget.transform.position).magnitude < 0.75 && !isLive)
        {
            ObjectPool.ReturnObject(this);
        }

        if (transform.position == moveTarget.transform.position)
        {
            transform.SetParent(moveTarget.transform);
            isMove = false;
            moveTarget = null;
        }
    }

    public void Move(GameObject target)
    {
        moveTarget = target;
        isMove = true;
    }

    public void Dead()
    {
        isLive = false;
    }
}
