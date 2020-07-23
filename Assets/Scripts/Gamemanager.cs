
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour//域与属性：https://www.jianshu.com/p/00bea9f0209a
{//MonoBehaviour is the base class from which every Unity script derives.派生

//When you use C#, you must explicitly derive from MonoBehaviour.
    //属性不表示存储位置，这是属性和域的根本性的区别。
    private static Gamemanager _instance;
    //this is a field.  It is private to your class and stores the actual data.
    public static Gamemanager Instance
    {// this is a property.  When you access it uses the underlying field, but only exposes
    // the contract that will not be affected by the underlying field
        get//get只读，在别的脚本里读取被封装的内容
        {
            return _instance;
        }
    }//为什么他不显示在控制面版
    //因为它是static呀，又不是成员变量，不显示。



   
    public GameObject blinky;
    public GameObject clyde;
    public GameObject inky;
    public GameObject pinky;
    public GameObject pacman;
    //UI:USER INTERFACE
    public GameObject winPrefab;
    public GameObject startCountDownPrefab;
    public GameObject gameoverPrefab;
    public AudioClip startclip;
    public GameObject startPanel;
    public GameObject gamePanel;
    public Text remainText;
    public Text nowText;
    public Text scoreText;

    //要做超级豆子和超级吃豆人
    public bool isSuperPacman = false;
    private List<GameObject> pacdotGos = new List<GameObject>();//用于创建对象和调用构造函数。
    
   //使不同鬼路径不同;方法，设计两个List数组，usingindex一个用来存每一次的random，一个放入排好序的四条路经rawIndex，raw是未加工的
    public List<int> usingIndex = new List<int>();
    public List<int> rawIndex = new List<int> { 0, 1, 2, 3 };
   
    //score count
    
    private int pacdotNum = 0;
    private int noweat= 0;
    public int score = 0;


    private void Awake()
    //Awake is called when the script instance is being loaded.
//Awake is used to initialize any variables or game state before the game starts.
 //you should use Awake to set up references between scripts, and use Start to pass any information back and forth. 
    {   

        _instance = this;//用于将某个对象传递给属于其他类的方法我觉得这里是这条

        //这里实现了路径的选择分配

        //for (int i=0; i<rawIndex.Count;i++)，不能把之后会改动的项拿座比较值，如果这样，他只能给出两个usingIndex，之后停止循环
        for (int i = 0; i < 4; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);//因为是Add所以控制面板上usingindex数不变，而且Add好处是，每天路径用时可不一样————并不是，实际上，他只管了start一次循环，
            //之后用Random.Range随便选的。。。
            //usingIndex.RemoveAll;??bug,而且这个for只循环1次，惊了呀，解答见上。
            rawIndex.RemoveAt(tempIndex);//把raw里面的拿出来

        }

        //把所有豆子放在一个列表下，为每隔一段时间生成一个超级豆子做准备：private List<GameObject> pacdotGos = new List<GameObject>();
        foreach (Transform t in GameObject.Find("MazeComplete").transform)//transform:Position, rotation and scale of an object.
        {
            pacdotGos.Add(t.gameObject);//把豆子添加进list
        }
        pacdotNum = GameObject.Find("Mazecomplete").transform.childCount;//孩子的数量，就是pacdot数量啦
        //遍历组件，用于把东西都放进去，约等于for循环
    }

    //start设置开始时大家都不许动突突突
    private void Start()
    {
        SetGameState(false);
    }

    //start按钮按下时的UI
    public void OnStartButton()
       
    {   //放声音
        StartCoroutine(PlayStartCountDown());
        AudioSource.PlayClipAtPoint(startclip, new Vector3(21,15,-9));//声音大小和距离相机远近有关

        //让开始面板消失
        startPanel.SetActive(false);
           // SetGameState(true);
            // Invoke("CreateSuperPacdot", 10f);//invoke,调用，在游戏开始后十秒invokes the method methodName in time seconds.,注意，产生就一个。
        //f是float
    }

    //exit按钮按下时的UI
    public void OnExitButton()
    {
        Application.Quit();

    }
    //倒计时UI，这是一个协程——理解应用有点困难coroutine
    //1、返回值类型必须是IEumerator迭代器

    //2、有返回值，返回参数的时候用yield return ……

    //协程方法的调用用StartCoroutine（CoroutineMethod（））

   IEnumerator PlayStartCountDown()
    {
      GameObject go =  Instantiate(startCountDownPrefab);//?
        yield return new WaitForSeconds(4f);//yield:出产
        Destroy(go);
        SetGameState(true);
        Invoke("CreateSuperPacdot", 10f);
        gamePanel.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

   
    private void Update()//实时记得分
        //Update is called every frame, if the MonoBehaviour is enabled.

//Update is the most commonly used function to implement s执行any kind of game behaviour.
    {
        //当吃完了//victory
        if (noweat==pacdotNum&&pacman.GetComponent<pacmanmove>().enabled!=false)//后半段是为了终止循环这个if，耗内存

        {
            gamePanel.SetActive(false);
            //隐藏面板
            Instantiate(winPrefab);//实例化winPrefab，耗内存,显示胜利标语
            StopAllCoroutines();// 游戏胜利，停止所有携程
            SetGameState(false);
            SetGameState(false);
        }

        //游戏结束，重载游戏
        if (noweat == pacdotNum)
        {
            if(Input.anyKeyDown)//按下键即可
            {
                SceneManager.LoadScene(0);
            }
        }
            
        //判断game面板没有被隐藏。更新数字
        //！！！！！这个不好用了，why？？？why？？？孩子不明白
        if (gamePanel.activeInHierarchy)
        {
            remainText.text = "REMAIN\n" + (pacdotNum - noweat);
            nowText.text = "EATEN\n" + (noweat);
            scoreText.text = "SCORE\n" + score;
        }
        
    }

    

    //当吃掉一个豆子会有的操作
    public void OnEatPacdot(GameObject go)//存个参数好把被吃掉的豆子传过来

    {
        score += 100;
        noweat++;
        pacdotGos.Remove(go);//这边在我建的挑幸运豆子的list里也把被吃的删掉
    }

    //当吃掉一个超级豆子会有的操作
    public void OnEatSuperPacdot()

    {
        score += 200;
        Invoke("CreateSuperPacdot", 10f);
        isSuperPacman = true;
        FreezeEnemy();
        Invoke("RecoveryEnemy", 3f);
        // RecoveryEnemy();//调用
        //MissingReferenceException: The object of type 'GameObject' has been destroyed but you are still trying to access it.
        //  Your script should either check if it is null or you should not destroy the object.
        //  Gamemanager.CreateSuperPacdot()(at Assets / Scripts / Gamemanager.cs:85)

    }
    

    //生成超级豆子
    private void CreateSuperPacdot()
    {
        if(pacdotGos.Count<5)
        {
            return;//防止10s和摧毁i两个线程冲突
        }
        int tempIndex = Random.Range(0, pacdotGos.Count);
        pacdotGos[tempIndex].transform.localScale = new Vector3(3, 3, 3);
        pacdotGos[tempIndex].GetComponent<Pacdot>().isSuperDot = true;//Returns the component of Type type if the game object has one attached, null if it doesn't
    }
    //解冻怪物时的操作
    private void RecoveryEnemy()
    {
        DisFreezeEnemy();
        isSuperPacman = false;
    //  Invoke("RecoveryEnemy", 3f);//invoke,调用，
    }

    //冻结敌人
    private void FreezeEnemy()
    {
        blinky.GetComponent<GoastMove>().enabled = false;
        clyde.GetComponent<GoastMove>().enabled = false;
        inky.GetComponent<GoastMove>().enabled = false;
        pinky.GetComponent<GoastMove>().enabled = false;
        //太强了把goastmove整个脚本🈲了//教程说只有update方法不被执行，hummm
        //这些都是inspector面板显示的东西，可以获取到哎
        blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);//我算是懂了，这个f就是float
        clyde.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);//这个叫角色渲染器
        inky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);

    }
    //解冻敌人
    private void DisFreezeEnemy()
    {
        blinky.GetComponent<GoastMove>().enabled = true;
        clyde.GetComponent<GoastMove>().enabled = true;
        inky.GetComponent<GoastMove>().enabled = true;
        pinky.GetComponent<GoastMove>().enabled = true;

        blinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);//我算是懂了，这个f就是float
        clyde.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        inky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    //UI 初始不动
    private void SetGameState(bool state)
    {
        blinky.GetComponent<GoastMove>().enabled = state;
        clyde.GetComponent<GoastMove>().enabled = state;
        inky.GetComponent<GoastMove>().enabled = state;
        pinky.GetComponent<GoastMove>().enabled = state;
        pacman.GetComponent<pacmanmove>().enabled = state;
    }
}