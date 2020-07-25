## BUG AND DEBUG
```diff
+一定要先规划好再写代码！**
```
<font size=4>BUG1:1复制粘贴忘改名字了</font>  

```
	remainText.text = "REMAIN\n" + (pacdotNum - noweat);
	
	nowText.text = "EATEN\n" + (noweat);
	
	remainText.text = "SCORE\n" + score;
```
BUG2:写了函数但并不调用使功能无法实现  
BUG3:运算符“!”无法应用于“Vector2”类型的操作数	  

```
!(Vector2)transform.position
```
我试图用它来说pacman如果走到墙了那么该位置不能到达（？）怎么说，就是令人信服的逻辑但是实现不了。  
所以我们启用了鬼能想到的射线  
```
 private bool Valid(Vector2 dir)
    {//记录下当前位置
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);//目的地往外发射线
        return (hit.collider == GetComponent<Collider2D>());
            //射线检测，射线到碰撞器及射线所打到的碰撞器是不是pacman身上那个
    }
```
BUG4:计数错误||不能把会后来会变设为if的判断
```/* if((index+1) >= Movepoints.Count)```
BUG5:**why？**注释掉的这个重选路径版本goast会穿墙 

```
/*	 if((index+1) >= Movepoints.Count)
         {
              LoadAPath(MovepointsGos[Random.Range(0, 3)]);
          }
          index = (index + 1) % Movepoints.Count; }*/

 	   index++;
          if (index >= Movepoints.Count)
          {
              index = 0;
              LoadAPath(MovepointsGos[Random.Range(0, 3)]);
          }
```
BUG6:当前上下文中不存在名称“startCountDownPrefab	  
这个问题是，我们是之后拖拽赋值的在inspector面板，所以，先创建空物体，而不是直接使用在unity面板里创建的物体   

**解决的BUG：**

BUG7.8.9:

gamepanel动画并不显示分数变化，似乎text固定住了，但是代码我感觉没错啊  
玩家还没点start，pacman和ghost就先动了！！！我惊了真的，明明状态是false  
unity报错了。ArgumentOutOfRangeException: Argument is out of range.  
   Parameter name: index  
   System.Collections.Generic.List`1[System.Int32].get_Item (Int32 index) (at /Users/builduser/buildslave/mono/build/mcs/class/corlib/System.Collections.Generic/List.cs:635)  
   GoastMove.Start () (at Assets/Scripts/GoastMove.cs:18)  

