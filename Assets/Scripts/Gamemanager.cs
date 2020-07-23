
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{

    private static Gamemanager _instance;
    public static Gamemanager Instance
    {
        get
        {
            return _instance;
        }
    }//?，而且为啥那么他不显示在控制面版？不配吗？,这有点像调用这个脚本。。//  Gamemanager.Instance.score += 500;在鬼动.cs里



    //使不同吃豆人路径不同;方法，设计两个List数组，usingindex一个用来存每一次的random，一个放入排好序的四条路经rawIndex，raw是未加工的
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



    //score count





    public bool isSuperPacman = false;
    //面板会显示

    public List<int> usingIndex = new List<int>();
    public List<int> rawIndex = new List<int> { 0, 1, 2, 3 };
    //要做超级豆子
    private List<GameObject> pacdotGos = new List<GameObject>();
    private int pacdotNum = 0;
    private int noweat= 0;
    public int score = 0;


    private void Awake()
    {

        _instance = this;//?用于将某个对象传递给属于其他类的方法我觉得是这条
                         //for (int i=0; i<rawIndex.Count;i++)，不能把之后会改动的项拿座比较值，如果这样，他只能给出两个usingIndex，之后停止循环
        for (int i = 0; i < 4; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);//因为是Add所以控制面板上usingindex数不变，而且Add好处是，每天路径用时可不一样————并不是，实际上，他只管了start一次循环，
            //之后用Random.Range随便选的。。。
            //usingIndex.RemoveAll;??bug,而且这个for只循环1次，惊了呀，解答见上。
            rawIndex.RemoveAt(tempIndex);//把raw里面的拿出来

        }
        foreach (Transform t in GameObject.Find("MazeComplete").transform)//transform:Position, rotation and scale of an object.
        {
            pacdotGos.Add(t.gameObject);//把豆子添加进list
        }
        pacdotNum = GameObject.Find("Mazecomplete").transform.childCount;//孩子的数量，就是pacdot数量啦
        //遍历组件，用于把东西都放进去，约等于for循环
    }//每隔一段时间生成一个超级豆子

    private void Start()
    {
        SetGameState(false);
    }
    public void OnStartButton()
       
    {//放声音
       
        StartCoroutine(PlayStartCountDown());
        AudioSource.PlayClipAtPoint(startclip, new Vector3(21,15,-9));//声音大小和距离相机远近有关

        //让开始面板消失
        startPanel.SetActive(false);
           // SetGameState(true);
            // Invoke("CreateSuperPacdot", 10f);//invoke,调用，在游戏开始后十秒invokes the method methodName in time seconds.,注意，产生就一个。
        //？这个f就很灵性，官网上例子没f，是float
    }
    public void OnExitButton()
    {
        Application.Quit();

    }
   IEnumerator PlayStartCountDown()//这叫啥？携程？
    {
      GameObject go =  Instantiate(startCountDownPrefab);//?
        yield return new WaitForSeconds(4f);//yield:出产
        Destroy(go);
        SetGameState(true);
        Invoke("CreateSuperPacdot", 10f);
        gamePanel.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    private void RecoveryEnemy()
    {
        DisFreezeEnemy();
        isSuperPacman = false;
      //  Invoke("RecoveryEnemy", 3f);//invoke,调用，
    }

    public void OnEatPacdot(GameObject go)//存个参数好把被吃掉的豆子传过来

    {
        score += 100;
        noweat++;
        pacdotGos.Remove(go);//这边在我建的挑幸运豆子的list里也把被吃的删掉
    }

    private void Update()//实时记得分
        //Update is called every frame, if the MonoBehaviour is enabled.

//Update is the most commonly used function to implement s执行any kind of game behaviour.
    {
        if (noweat==pacdotNum&&pacman.GetComponent<pacmanmove>().enabled!=false)//后半段是为了终止循环这个if，耗内存

        {
            gamePanel.SetActive(false);
            //隐藏面板
            Instantiate(winPrefab);//实例化winPrefab，耗内存,显示胜利标语
            StopAllCoroutines();// 超多携程（？）游戏胜利，停止所有携程
            SetGameState(false);
            SetGameState(false);
        }//victory

        //重载游戏
        if (noweat == pacdotNum)
        {
            if(Input.anyKeyDown)//按下键
            {
                SceneManager.LoadScene(0);
            }
        }
            

        if (gamePanel.activeInHierarchy)//判断game面板没有被隐藏。更新数字
        {
            remainText.text = "REMAIN\n" + (pacdotNum - noweat);
            nowText.text = "EATEN\n" + (noweat);
            scoreText.text = "SCORE\n" + score;
        }
        
    }

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