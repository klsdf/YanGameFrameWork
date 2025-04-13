# YanGameFrameWork框架

**YanGameFrameWork框架**，主打一个轻量级、实用性。而且没有很多抽象的封装，主打一个我自己用的开心就完事了。

## 核心模块

### 音频管理系统（Audio System）

音频管理系统提供了统一的音频资源管理和控制接口。
命名空间：YanGameFrameWork.AudioSystem

- 支持BGM和音效的统一管理
- 提供音量控制、淡入淡出等基础功能
- 使用Template Method实现音频控制
- 通过Facede统一管理所有音频相关操作

### 事件系统（Event System）

提供轻量级的事件通信机制，用于解耦游戏系统间的交互。
命名空间：YanGameFrameWork.EventSystem

#### 核心功能

- 支持泛型事件参数传递
- 提供优先级事件监听
- 支持一次性事件订阅
- 自动清理未使用的事件监听器
- 线程安全的事件派发

#### 基本使用

```csharp
// 定义事件数据类型
public class PlayerData
{
    public int Health { get; set; }
    public Vector3 Position { get; set; }
}

// 订阅事件
EventManager.Instance.AddListener<PlayerData>("OnPlayerDamaged", OnPlayerDamaged);

// 发布事件
PlayerData data = new PlayerData { Health = 80, Position = transform.position };
EventManager.Instance.TriggerEvent("OnPlayerDamaged", data);

// 取消订阅
EventManager.Instance.RemoveListener<PlayerData>("OnPlayerDamaged", OnPlayerDamaged);
```

#### 高级特性

1. 优先级监听

```csharp
// 添加高优先级监听器
EventManager.Instance.AddListener<PlayerData>("OnPlayerDamaged", OnPlayerDamaged, 100);
```

2. 一次性事件

```csharp
// 订阅只触发一次的事件
EventManager.Instance.AddOnceListener<PlayerData>("OnPlayerDamaged", OnPlayerDamaged);
```

#### 最佳实践

1. 事件命名规范

   - 使用动词+名词的形式
   - 使用OnXXX前缀表示事件
   - 清晰表达事件的用途
2. 性能优化

   - 及时移除不需要的事件监听
   - 避免在Update等频繁调用的方法中触发事件
   - 合理使用事件缓存池
3. 调试支持

   - 支持事件触发日志
   - 提供事件监听器统计
   - 内存泄漏检测

#### 注意事项

1. 在对象销毁前记得取消事件订阅
2. 避免事件循环触发
3. 合理控制事件参数的大小
4. 注意事件的触发时序

### 多语言系统（Localization System）

用于实现游戏的多语言本地化功能。

命名空间：YanGameFrameWork.LocalizationSystem
使用方法：

```c#
LocalizationManager.Instance.GetLocalizedString("一段神秘的文本");
```

### 对象池系统（Object Pool System）

提供高效的游戏对象重用机制，用于优化频繁创建和销毁对象的性能问题。

**命名空间：YanGameFrameWork.ObjectPoolSystem**



### UI系统（UI System）

提供一个灵活的UI框架，用于创建和管理游戏内的用户界面。

命名空间：YanGameFrameWork.UISystem



#### 主要功能

- 支持多种UI控件，如按钮、文本框和滑动条。
- 提供UI事件的绑定和处理机制。
- 支持UI皮肤和主题的定制。

#### 基本使用

```csharp
// 创建一个按钮并设置位置
UIButton myButton = UIManager.CreateButton("MyButton");
myButton.SetPosition(new Vector2(100, 200));
// 绑定点击事件
myButton.OnClick += OnButtonClick;
```

#### 注意事项

- 确保UI元素在适当的生命周期内被创建和销毁。
- 避免在UI事件处理中执行耗时的操作。

### 场景控制系统（Scene Control System）

#### 主要功能

- 支持游戏对象的动态池化管理
- 自动扩容和收缩池容量
- 支持预加载对象池
- 支持对象池分类管理
- 提供对象生命周期回调

#### 基本使用

```csharp
// 获取对象池管理器实例
ObjectPoolManager poolManager = ObjectPoolManager.Instance;

// 创建对象池
poolManager.CreatePool("EnemyPool", enemyPrefab, 10);

// 从池中获取对象
GameObject enemy = poolManager.GetObject("EnemyPool");

// 返回对象到池中
poolManager.ReturnObject("EnemyPool", enemy);
```

### 事件系统（Event System）

提供全局事件通信机制。

- 事件的订阅与发布
- 支持带参数的事件传递
- 事件优先级管理
- 自动事件垃圾回收

### 存档系统（Save System）

处理游戏数据的保存和读取。
命名空间：YanGameFrameWork.SaveSystem

### 实用工具库（Practical Library）

我自己常用的一些代码，比如单例类的模板什么的。

### 工具箱（Utility Toolkit）

包含各种实用的开发辅助工具。
