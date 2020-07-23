
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GoastMove: MonoBehaviour
{
   //储存所有路径点的transform组件
    public GameObject[] MovepointsGos;
    public float speed = 0.15f;
    //当前在前往哪个路径点途中
    private List<Vector3> Movepoints = new List<Vector3>();//高级三维数组
    private int index = 0;
    private Vector3 startPos;//巡逻起点、巡逻重点开始施工

    private void Start()
    {//加载路线
        startPos = transform.position + new Vector3(0, 3, 0);//起点
        LoadAPath(MovepointsGos[Gamemanager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder-2]]);
        //调用！！+套娃，index|index|老大老二....index就是数字、序数，嗯。
     
    }

    private void FixedUpdate()//方法(函数)
        //This function is called every fixed framerate frame, if the MonoBehaviour is enabled.

//FixedUpdate should be used instead of Update when dealing with Rigidbody.For example when adding a force to a rigidbody,
        //you have to apply the force every fixed frame inside FixedUpdate instead of every frame inside Update.
    {   if (transform.position != Movepoints[index])
        {
            //移动的核心
            Vector2 temp = Vector2.MoveTowards(transform.position, Movepoints[index], speed);//插值temporary
            GetComponent<Rigidbody2D>().MovePosition(temp);//方法
                                                           //按键检测且必须先达到上一个dest位置才可以进行新一次目标指令
        }
        else
        {
            /* if((index+1) >= Movepoints.Count)//之前报错原因：应该有+1
             {
                 LoadAPath(MovepointsGos[Random.Range(0, 3)]);
             }
             index = (index + 1) % Movepoints.Count; }*/
            index++;
            if (index >= Movepoints.Count)
            {
                index = 0;
                LoadAPath(MovepointsGos[Random.Range(0, 3)]);//一轮走过了，重选一条路，那我要是选了和之前一条路。。。。。？？？
            }
            Vector2 dir = Movepoints[index] - transform.position;
            //把获取到的移动方向设置给动画状态机
            GetComponent<Animator>().SetFloat("Dirx", dir.x);
            GetComponent<Animator>().SetFloat("Diry", dir.y);
        }
    }


    private void LoadAPath(GameObject go)//写一个函数来加载路线
    {
        Movepoints.Clear();
        //加载路线前清空之前的路径
        foreach (Transform t in go.transform)//遍历go的teansform组件
        { Movepoints.Add(t.position); }//存入数组
        //插入起点终点
        Movepoints.Insert(0, startPos);
        Movepoints.Add(startPos);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman_0")
        {
            if(Gamemanager.Instance.isSuperPacman){

                transform.position = startPos - new Vector3(0, 3, 0);
                index = 0;
                Gamemanager.Instance.score += 500;
                

            }


            else
            {
                // Destroy(collision.gameObject);//销毁pacman ，不优越，不如把他隐藏起来不影响他人
                collision.gameObject.SetActive(false);
                //死亡动画
                Gamemanager.Instance.gamePanel.SetActive(false);
                Instantiate(Gamemanager.Instance.gameoverPrefab);
                Invoke("ReStart",3f);
            }


        }
    }
    //重载游戏
    private void ReStart()
    {
        SceneManager.LoadScene(0);
    }
}
