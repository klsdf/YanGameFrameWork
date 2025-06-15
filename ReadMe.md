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



### 安装

#### 直接安装

1. 把YanGameFrameWork的文件夹复制到游戏的Asserts目录下
2. 好了！框架已经成功引入了！







#### 或者通过submodule来安装

1. 打开项目的git仓库
2. 添加[YanGameFrameWork](https://github.com/klsdf/YanGameFrameWork)的submodule到Asserts内的任意文件夹下





#### 安装成功了吗？













### 使用

1. 首先需要把框架的核心预制体YanGameFrameWork放到场景内

1. 对于核心功能，可以直接使用YanGF来得到

   ```c#
   YanGF.Debug.Log("测试", "Hello World!");
   ```

   







## 核心模块

### Debug系统（Debug System）

- 提供日志记录、警告和错误信息的输出功能。
- 支持日志合并和堆栈跟踪显示。


### 事件系统（Event System）

- 提供轻量级的事件通信机制，支持泛型事件参数传递。
- 支持优先级事件监听和一次性事件订阅。

### 状态机系统（FSM System）

- 提供状态机的基础实现，支持状态的切换和生命周期管理。

### HTTP请求工具（HTTP Util）

- 提供HTTP请求的封装，支持GET、POST、PUT、DELETE等请求方法。

### 单例模式（Singleton）

- 提供单例模式的实现，支持泛型单例类。

### 单元测试(TestAssert)

- 提供单元测试的断言功能，支持多种断言方法。

### Tween系统（Tween System）

- 提供Tween的实现，支持多种Tween类型。

### UI系统（UI System）

- 提供灵活的UI框架，支持多种UI控件和事件绑定。











## 项目模块

### 飞书反馈




### 成就系统（Achievement System）

- 提供成就的注册、更新和解锁功能。
- 支持事件型和进度型成就。
- 提供成就的UI展示和提示功能。


### AI请求


### 音频管理系统（Audio System）

- 提供音频资源的统一管理和控制接口。
- 支持背景音乐和音效的音量控制、淡入淡出等功能。


### 可等待协程（AwaitableCoroutine）

### 摄像机控制（CameraController）



### 本地化系统（Localization System）

- 实现游戏的多语言本地化功能，支持动态文本替换。

### 对象池系统（Object Pool System）

- 提供高效的游戏对象重用机制，优化对象的创建和销毁性能。



### 对话系统（DialogSystem）









### 场景控制系统（Scene Control System）

- 支持游戏对象的动态池化管理和对象生命周期回调。

### 存档系统（Save System）

- 处理游戏数据的保存和读取，支持多种存档格式。

### 实用工具库（Practical Library）

- 包含常用的代码模板和工具类，提升开发效率。

### 工具箱（Utility Toolkit）

- 提供各种实用的开发辅助工具，简化开发流程。



### 资源管理系统（ResourceControlSystem）

- 提供资源的管理和加载功能，支持多种资源类型。


### 数据管理系统 （ModelControlSystem）

- 提供数据的管理和加载功能，支持多种数据类型。

### 教程系统（Tutorial System）

- 提供教程的注册、更新和解锁功能。
- 支持事件型和进度型教程。
- 提供教程的UI展示和提示功能。





## 编辑器和特性

本部分的设计参考了Odin，让所有的函数都可以加上[Button]特性，从而在面板中出现一个可以点击的按钮。同时按钮会根据当前函数的参数而动态增加输入框。





## 预制Shader

这是框架所自带的常用shader





## 预制字体

- 本框架提供了多个免费的预制字体，并已经创建了TMP的assert资产。即开即用。

- 提供了多种语言的字体





## 代码风格约束

要想使用代码风格约束











## 可选第三方框架

引入本框架时，无需引入任何第三方框架。但是引入这些第三方框架，会让YanGF拥有更强大的能力。



### LibTessDotNet





