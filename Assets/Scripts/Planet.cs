using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;
    public GameManager gm;
    public ParticleSystem effect;
    

    public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;

    float deadTime;
    SpriteRenderer spriteRenderer; 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
   
    }

    void Start()
    {
    
    }
    void OnEnable()
    {
        anim.SetInteger("Level", level);
    }
    void OnDisable()
    {
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;

    }
    void Update()
    {

        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x / 2f;
            float rightBorder = 4.2f - transform.localScale.x / 2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.y = 7;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }


    }

    public void Drag()
    {
        isDrag = true;

    }
    public void Drop()
    {
        isDrag = false;
        if (rigid != null)
        {
            rigid.simulated = true;
        }
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine("AttachRoutine");

       
    }
    IEnumerator AttachRoutine()
    {
        if (isAttach)
        {
            yield break;
        }

        isAttach = true;
        gm.SfxPlay(GameManager.Sfx.Attach);
        yield return new WaitForSeconds(0.2f);

        isAttach = false;
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            Planet other = collision.gameObject.GetComponent<Planet>();

            if(level == other.level && !isMerge && !other.isMerge && level < 8)
            {
                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                if(myY < otherY || (myY == otherY && myX > otherX))
                {
                    other.Hide(transform.position);
                    levelUp();
                }


            }
        }
    }
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;

        rigid.simulated = false;
        circle.enabled = false;
        if(targetPos == Vector3.up * 100)
        {
            EffexctPlay();
        }

        StartCoroutine(HideRoutine(targetPos));

    }
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while(frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            else if (targetPos == Vector3.up * 100)
            {

                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            yield return null;
        }
        gm.score += (int)Mathf.Pow(2, level);
        isMerge = false;

        gameObject.SetActive(false);

    }
    public void levelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0f;

        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);


        anim.SetInteger("Level", level + 1);
        EffexctPlay();
        gm.SfxPlay(GameManager.Sfx.LevelUp);



        yield return new WaitForSeconds(0.3f);
        level++;

        gm.maxLevel = Mathf.Max(level, gm.maxLevel);

        isMerge = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            
            deadTime += Time.deltaTime;
            if (deadTime > 2)
            {
               
                float fiveValue = deadTime * 10;
                int intValue= Mathf.FloorToInt(fiveValue);
                int checkTime = intValue % 5;
                if (checkTime==0)
                {
                    spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
                  
                }
                else
                {
                    spriteRenderer.color = Color.white;
              
                }

            }
            if (deadTime > 5)
            {
                gm.GameOver();
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if(collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
            
        }
    }
    void EffexctPlay() {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
        

    }
}
