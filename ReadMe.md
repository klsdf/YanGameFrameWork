# YanGameFrameWork框架

作者：闫辰祥

**YanGameFrameWork框架**，是一个轻量级、即插即用、全面的游戏开发框架，旨在快速完成一些常见的模块和功能。是一款为Gamejam而生的框架。

下面是框架的几个核心特征

- 不强依赖任何第三方框架，直接引入就能用
- 全面的预制系统，覆盖绝大多数常见的需求
- 无任何侵入性设计，引入前和引入后对项目无任何修改

## 快速上手

### 找到框架的仓库

1. 访问https://github.com/klsdf/YanGameFrameWork.git

### 找到框架的仓库

1. 访问https://github.com/klsdf/YanGameFrameWork.git

### 安装

#### 直接安装

1. 把YanGameFrameWork的文件夹复制到游戏的Asserts目录下
2. 好了！框架已经成功引入了！

#### 或者通过submodule来安装

1. 打开项目的git仓库
2. 添加[YanGameFrameWork](https://github.com/klsdf/YanGameFrameWork)的submodule到Asserts内的任意文件夹下





#### 安装成功了吗？




### 引入一些unity自带的框架



1. 引入TMP
2. 引入Localization
3. 引入Newton 的json



### 使用

1. 首先需要把框架的核心预制体YanGameFrameWork放到场景内
2. 对于核心功能，可以直接使用YanGF来得到

   ```c#
   YanGF.Debug.Log("测试", "Hello World!");
   ```


## 核心模块

### Debug系统（Debug System）

#### 日志输出

- 提供日志记录、警告和错误信息的输出功能。
- 支持全局开关

具体使用：

```c#
// 普通日志
DebugController.Instance.Log(nameof(YourClass), "这是一条普通日志");

// 警告日志
DebugController.Instance.LogWarning(nameof(YourClass), "这是一条警告日志");

// 错误日志
DebugController.Instance.LogError(nameof(YourClass), "这是一条错误日志");

// 断言日志
DebugController.Instance.LogAssert(nameof(YourClass), () => condition, "断言失败信息");

// 异常日志
DebugController.Instance.LogException(nameof(YourClass), exception);
```

#### GM和Debug窗口

本部分参考了[In-game Debug Console](https://assetstore.unity.com/packages/tools/gui/in-game-debug-console-68068?srsltid=AfmBOophIUrqfp-n8jygcExQxlser_sRPSp3HF8dg0kakgiDh7GAJNbk)的设计

- 在游戏内使用~键可以呼出控制台
- 控制台中会显示游戏内的所有log信息
- 控制台内集成了gm输入框，开发者可以自由输入设定的质量快速调试

### 事件系统（Event System）

YanGameFramework的事件系统是一个基于发布者-订阅者模式的事件管理系统，支持多种类型的事件处理：

- 无参数事件
- 单参数事件
- 双参数事件
- 一次性事件
- 带优先级的事件
- 提供轻量级的事件通信机制，支持泛型事件参数传递。
- 支持优先级事件监听和一次性事件订阅。


#### 注册事件

```c#

// 注册无参数事件
EventSystemController.Instance.AddListener("GameStart", OnGameStart, 1);

// 注册一次性事件
EventSystemController.Instance.AddOnceListener("LevelComplete", OnLevelComplete);

```

#### 触发事件


```c#

// 触发事件
EventSystemController.Instance.TriggerEvent("GameStart");

// 移除事件
EventSystemController.Instance.RemoveListener("GameStart", OnGameStart);
```




### 状态机系统（FSM System）

- 提供状态机的基础实现，支持状态的切换和生命周期管理。

### HTTP请求工具（HTTP Util）

- 提供HTTP请求的封装，支持GET、POST、PUT、DELETE等请求方法。

### 单例模式（Singleton）

- 提供单例模式的实现，支持泛型单例类。

### 单元测试(TestAssert)

- 提供单元测试的断言功能，支持多种断言方法。



### Tween系统（Tween System）

tween用于实现补间动画



#### 同步函数的Tween

TweenController.Tween方法

| 参数               | 类型                          | 描述                     | 额外信息 |
| ---------------- | --------------------------- | ---------------------- | ---- |
| target           | T                           | 需要被Tween的对象            |      |
| propertySelector | Expression<Func<T, TValue>> | 需要写一个lambda，返回需要被修改的属性 |      |
| endValue         | TValue                      | 把这个属性的值改为什么            |      |
| duration         | float                       | 持续时间，也就是花多久把这个值改为目标值   |      |
| onComplete       | Action                      | 结束时的回调函数               | 可空类型 |



```csharp
    public void AlphaTweenTest()
    {
        YanGF.Tween.Tween(
            canvasGroup,
            item => item.alpha,
            0f,
            1f,
            () => Debug.Log("Tween完成")
        );
    }
```



#### 异步函数的Tween





### UI系统（UI System）

- 管理UI的显隐

#### 显示一个UI

1. 让面板继承UIPanelBase
2. 做一个预制体，放到Resource里面
3. 然后把这个脚本挂载到预制体里面
4. 使用 `YanGF.UI.PushPanel<StaffPanel>``();`来弹出一个面板

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YanGameFrameWork.UISystem;

public class StaffPanel : UIPanelBase
{
    public Button closeButton;
    public override void Start()
    {
        base.Start();
        closeButton.onClick.AddListener(() =>
        {
            YanGF.UI.PopPanel<StaffPanel>();
        });
    }
    public override void OnLocalize()
    {
        // throw new System.NotImplementedException();
    }
}



```

```csharp

staffButton.onClick.AddListener(() =>
{
	YanGF.UI.PushPanel<StaffPanel>();
});

```



#### 隐藏一个UI

```c#
closeButton.onClick.AddListener(() =>
{
	YanGF.UI.PopPanel<StaffPanel>();
});
```



## 项目模块



### 场景管理

此场景非彼场景



#### 注册场景



#### 移动到场景









### 读表工具（ConfigTableReader）


#### 数据表格式

本框架的数据格式形如：

- 第一行：变量名，需要和代码内保持一致
- 第二行：变量类型
- 第三行：注释，默认不会进行处理

之后都是数据行




| name   | age   | description        |
| ------ | ----- | ------------------ |
| string | int   | string             |
| 萝莉的姓名  | 小萝莉年龄 | 小萝莉的描述             |
| 小樱     | 12    | 活泼可爱的粉色头发萝莉，喜欢收集卡片 |
| 小爱     | 10    | 温柔的金发萝莉，总是带着微笑     |
| 小美     | 11    | 聪明的蓝发萝莉，擅长解谜游戏     |
| 小萌     | 9     | 可爱的双马尾萝莉，喜欢吃甜点     |
| 小雅     | 13    | 优雅的黑发萝莉，喜欢读书和音乐    |
| 小莉     | 8     | 调皮的红发萝莉，总是充满活力     |



#### 读取数据表


1. 编写数据类
```c#
class LoliData
{
	public string name;
	public int age;
	public string description;
}
```


2. 创建csv并放到Assets的StreamingAssets中
3. 使用数据类作为泛型，来读取数据

```c#

[Button("测试读取配置表通过路径")]
public void TestReadConfigTableByPath()
{
	string filePath = Path.Combine(Application.streamingAssetsPath, "LoliData.csv");
	List<LoliData> data = ConfigTableReader.ReadConfigTableByPath<LoliData>(filePath);
	foreach (var item in data)
	{
		Debug.Log($"name: {item.name}, age: {item.age}, description: {item.description}");
	}
}
```


### 飞书反馈（GlobalDebugFeedbackController）






### 成就系统（Achievement System）

- 提供成就的注册、更新和解锁功能。
- 支持事件型和进度型成就。
- 提供成就的UI展示和提示功能。



#### 定义成就


- EventAchievement：事件型成就，用于在某个事件触发时触发
- ProgressAchievement：进度型成就，用于达成某个进度时触发




| 参数       | 类型     | 描述                         | 额外信息 |
| -------- | ------ | -------------------------- | ---- |
| key      | string | 成就的key                     |      |
| isHide   | bool   | 是否是需要隐藏的成就，也就是没解锁的时候需不需要隐藏 |      |
| onUnlock | Action | 当解锁时触发的回调                  | 可空   |



```c#
 YanGF.Achievement.RegisterAchievement(new EventAchievement(
		"Start",
		false, () =>
	{
		print("梦的开始");
	}));
	YanGF.Achievement.RegisterAchievement(
		new ProgressAchievement(
			"Money1",
			100, false, () =>
	{
		print("靓仔，要来点拼好饭吗？");
	}));



```


#### 触发成就

事件型成就传入key即可

进度型成就要传入key和**增量**，注意这个是增量捏



```c#
YanGF.Achievement.UpdateOrUnlockAchievement("Start");

YanGF.Achievement.UpdateOrUnlockAchievement("Money1", 10);




```







### AI请求




### 音频管理系统（Audio System）

- 提供音频资源的统一管理和控制接口。
- 支持背景音乐和音效的音量控制、淡入淡出等功能。



#### 播放一个循环的音频（PlayLoop）


```c#
YanGF.Audio.PlayLoop(clip);
```




#### 播放一个单次循环的音频（PlayOnce）

```c#
YanGF.Audio.PlayOnce(clip);
```




#### 设置主音量

音量值（通常在-80到20之间）

```c#
YanGF.Audio.SetMasterVolume(mappedValue);
```




### 可等待协程（AwaitableCoroutine）



#### 将协程转为Task



```c#

IEnumerator TaskA()
{
	Debug.Log("A 开始");
	yield return new WaitForSeconds(1);
	Debug.Log("A 结束");
}

IEnumerator TaskB()
{
	Debug.Log("B 开始");
	yield return new WaitForSeconds(2);
	Debug.Log("B 结束");
}

IEnumerator TaskC()
{
	Debug.Log("C 开始");
	yield return new WaitForSeconds(1);
	Debug.Log("C 结束");
}

async void Start()
{
	Debug.Log("开始执行任务序列");

	await CoroutineTaskRunner.Run(TaskA());
	await CoroutineTaskRunner.Run(TaskB());
	await CoroutineTaskRunner.Run(TaskC());
	await StartAsync();

	Debug.Log("所有任务完成了喵~");
}

```

#### 将Task转为协程



```c#

TaskToCoroutine.WaitForTask(OpenDialogAsync());
private async Task OpenDialogAsync()
{

}

```



### 摄像机控制（CameraController）

摄像机控制提供了4种核心功能和1种摄像机特效

- 跟随
- 注视
- 拖动
- 缩放


#### 震动摄像机（ShakeCamera）

| 参数        | 类型    | 描述  | 额外信息 |
| --------- | ----- | --- | ---- |
| duration  | float |     |      |
| magnitude | float |     |      |



```c#

 YanGF.Camera.ShakeCamera(0.3f, 0.1f);

```


#### 注册拖拽的条件（RegisterCanDragEvent）

在开启拖拽摄像机的选项后，如果希望手动设置拖拽的条件，可以使用`RegisterCanDragEvent`，返回true时可以被拖拽，false不可以被拖拽

```c#
   YanGF.Camera.RegisterCanDragEvent(() =>
       {

           if (Util.IsPointerOverUI(gameObject))
           {
               print("现在还不能拖拽哦,因为鼠标在商店卡牌UI上");
               return false;
           }

           return true;
       });
```




#### 移动摄像机（MoveToTarget）

```c#
  YanGF.Camera.MoveToTarget(target, 0.3f, YanGameFrameWork.CameraController.MoveCurveType.Linear, () =>
        {
            YanGF.Camera.ZoomToTarget(targetSize: 5, duration: 0.1f, onComplete: () =>
            {
                YanGF.Tutoria.FocusOn(new List<Transform> { target }, () =>
                {
                    //Time.timeScale = 0;

                    if (YanGF.Scene.ActiveScene == YanGF.Scene.GetSceneObjByType<SceneGame>())
                    {
                        YanGF.Camera.IsEnableZoom = false;
                        YanGF.Camera.IsEnableDarg = false;
                    }

                });
            });

        });
```





#### 缩放摄像机（ZoomToTarget）


```c#

YanGF.Camera.ZoomToTarget(targetSize: 5, duration: 0.1f, onComplete: () =>
            {
                YanGF.Tutoria.FocusOn(new List<Transform> { target }, () =>
                {
                    //Time.timeScale = 0;

                    if (YanGF.Scene.ActiveScene == YanGF.Scene.GetSceneObjByType<SceneGame>())
                    {
                        YanGF.Camera.IsEnableZoom = false;
                        YanGF.Camera.IsEnableDarg = false;
                    }

                });
            });
```


### 本地化系统（Localization System）

- 提供了YanGF自己实现的一套基于csv的本地化方案
- 兼容unity自己的本地化方案
- 在设计上采用了策略模式，开发者可以轻松切换不同的本地化方案





#### 翻译文本（Translate）


```C#
 string text1 =YanGF.Localization.Translate("UIPanel_TopBar_StoreHouseText","仓库已有：");
```


#### 订阅翻译变化事件（OnLanguageChanged）

当语言变更时，会调用这个函数。

```c#
   YanGF.Localization.OnLanguageChanged += UpdateUI;
```


### 对象池系统（Object Pool System）

- 本对象池是对unity内置对象池的二次封装，可以同时管理多个不同种类的对象池

### 对话系统（DialogSystem）




### 场景控制系统（Scene Control System）

- 支持游戏对象的动态池化管理和对象生命周期回调。

### 存档系统（Save System）

- 本部分的API设计参考了[Easy Save - The Complete Save Data &amp; Serializer System](https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-game-data-serializer-system-768)‘
- 开发者只需简单地调用 `YanGF.Save.Save<int>("test", 1);`即可轻松存储1这个数值到存档的test字段


#### 存储存档（Save）

```c#
string testString = "测试字符串";
YanGF.Save.Save("testString", testString, "TestSave");
```


#### 读取存档（Load）

```c#
string result = YanGF.Save.Load<string>("testString", "默认值", "TestSave");
```


### 实用工具库（Practical Library）

- 包含常用的代码模板和工具类，提升开发效率。

### 工具箱（Utility Toolkit）

- 提供各种实用的开发辅助工具，简化开发流程。


### 资源管理系统（ResourceControlSystem）

- 提供资源的管理和加载功能，支持多种资源类型。




### 数据管理系统 （ModelControlSystem）

YanGameFramework的模块控制系统是一个用于管理游戏数据模块的框架，采用单例模式实现。该系统主要用于：

- 管理游戏中的各种数据模块
- 提供模块的注册、更新、获取和移除功能
- 支持模块数据的序列化和Inspector可视化
- 实现模块数据变化的监听机制
- 提供数据的管理和加载功能，支持多种数据类型。


#### 定义数据类

编写数据类，需要继承基类。别忘了在set里面写一下NotifyDataChanged

```c#

[Serializable]
public class GameRuntimeData : YanModelBase
{
	private int _money;
	public int Money
	{
		get => _money;
		set
		{
			_money = value;
			NotifyDataChanged(this);
		}
	}

	public override YanModelBase Clone(YanModelBase model)
	{
		GameRuntimeData data = model as GameRuntimeData;
		this._money = data._money;
		return this;
	}
}


```



#### 读取数据



 使用GetModel获得这个model
 
```c#
public int GetMoney()
{
	return YanGF.Model.GetModel<GameRuntimeData>().Money;
}
```





### 教程系统（Tutorial System）

- 内置了一个非常通用的聚焦UI，可以快速实现游戏中聚焦某一个对象的效果。



#### 聚焦对象（FocusOn）

聚焦对象是指，出现一个遮罩画面，将除了需要被聚焦物体的部分全部遮住的效果。


| 参数        | 类型               | 描述                                      | 额外信息 |
| --------- | ---------------- | --------------------------------------- | ---- |
| targets   | List\<Transform> | 需要被遮罩的物体，注意要传入有大小的transfrom，否则会圈出大小为0的圈 |      |
| pauseGame | Action           | 当聚焦对象时，往往会暂停游戏，暂停函数的方法需要自行指定            |      |



```c#
YanGF.Tutoria.FocusOn(new List<Transform> { _tutoriaTarget }, () =>
{
	Time.timeScale = 0;
	YanGF.Camera.IsEnableZoom = false;
	YanGF.Camera.IsEnableDarg = false;

});
```




#### 隐藏引导（Hide）


| 参数         | 类型     | 描述                           | 额外信息 |
| ---------- | ------ | ---------------------------- | ---- |
| resumeGame | Action | 当引导消失后，往往会继续游戏，继续函数的方法需要自行指定 |      |



```c#

 YanGF.Tutoria.Hide(() =>
        {
            Time.timeScale = 1;

            YanGF.Camera.IsEnableZoom = true;
            YanGF.Camera.IsEnableDarg = true;
        });
```








### 技能系统



### 鼠标状态控制（CursorManager）








## 编辑器和特性

本部分的设计参考了Odin，让所有的函数都可以加上[Button]特性，从而在面板中出现一个可以点击的按钮。同时按钮会根据当前函数的参数而动态增加输入框。

### Button特性








## 预制Shader

这是框架所自带的常用shader

URP_FlashEffect

MaskTransition




## 预制字体

- 本框架提供了多个免费的预制字体，并已经创建了TMP的assert资产。即开即用。
- 提供了多种语言的字体

## 代码风格约束

要想使用代码风格约束

## 可选第三方框架

引入本框架时，无需引入任何第三方框架。但是引入这些第三方框架，会让YanGF拥有更强大的能力。

### LibTessDotNet
