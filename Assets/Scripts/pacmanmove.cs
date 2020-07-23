using UnityEngine;

public class pacmanmove : MonoBehaviour
{
    //ctrl K+F  typesetting

    public float speed = 0.35f;
    //吃豆人下一次移动去的目的地
    private Vector2 dest = Vector2.zero;
    private void Start()
    {//保证吃豆人在游戏刚开始的时候不会动
        dest = transform.position;//当前的位置

    }
    private void FixedUpdate()//方法(函数)
    {//移动的核心
        Vector2 temp = Vector2.MoveTowards(transform.position, dest, speed);//插值temporary
        GetComponent<Rigidbody2D>().MovePosition(temp);//方法
        //按键检测且必须先达到上一个dest位置才可以进行新一次目标指令
        if((Vector2)transform.position == dest )
        {
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))&& Valid(Vector2.up))
            {
                dest = (Vector2)transform.position + Vector2.up;
            }
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))&& Valid(Vector2.down))
            {
                dest = (Vector2)transform.position + Vector2.down;
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))&& Valid(Vector2.left))
            {
                dest = (Vector2)transform.position + Vector2.left;
            }
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Valid(Vector2.right))
            {
                dest = (Vector2)transform.position + Vector2.right;
            }//获取移动方向
            Vector2 dir = dest - (Vector2)transform.position;
            //把获取到的移动方向设置给动画状态机
            GetComponent<Animator>().SetFloat("Dirx", dir.x);
            GetComponent<Animator>().SetFloat("Diry", dir.y);
        }

        
    }
    private bool Valid(Vector2 dir)//射线检测，射线到碰撞器及射线所打到的碰撞器是不是pacman身上那个
    //用来确定还能继续往某个方向走不会撞墙
    {//记录下当前位置
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);//目的地往外发射线
        return (hit.collider == GetComponent<Collider2D>());
            
    }

}
