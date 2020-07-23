
using UnityEngine;

public class Pacdot : MonoBehaviour
{
    public bool isSuperDot = false;
//豆子遇吃豆人
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman_0")
        {

            if (isSuperDot)
            {
                Gamemanager.Instance.OnEatPacdot(gameObject);
                Gamemanager.Instance.OnEatSuperPacdot();//别忘了调用
                Destroy(gameObject);//todo:告诉gamemanager我是超级豆子，而且被吃调用
                //让吃豆人变成超级吃豆人，可以吃鬼
            }
            else {
                Gamemanager.Instance.OnEatPacdot(gameObject);////自己大胆加一句，加对了
                Destroy(gameObject); }
          
           

        }
    }




}
