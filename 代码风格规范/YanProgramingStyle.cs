/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20 12:00
 * Description: 代码风格规范的模板，所有代码前都需要加这个。
 * 代码规范的.editorconfig在YanGameFrameWork文件夹根目录下。
 * 请确保安装了EditorConfig for VS Code或者类似的玩意。
 *
 * 修改记录：
 * 2025-03-20 闫辰祥 创建，一般默认可以不写，第一次写完之后，如果对代码有更改。需要加这个注释。
 ****************************************************************************/

namespace YanGameFrameWork.Example
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    /// <summary>
    /// 类前用文档注释
    /// 类名使用大驼峰命名
    /// </summary>
    public class YanProgramingStyle
    {

        /// <summary>
        /// 私有变量前用下划线+小驼峰命名
        /// </summary>
        private int _examplePrivateInt;

        /// <summary>
        /// 公有变量前用小驼峰命名，但是不要使用下划线
        /// </summary>
        public int examplePublicInt;

        /// <summary>
        /// 属性用大驼峰
        /// </summary>
        public int ExampleProperty { get; set; }


        /// <summary>
        /// 常量使用大写
        /// </summary>
        public const string ExampleConstString = "example";

        /// <summary>
        /// 枚举使用大写
        /// </summary>
        public enum ExampleEnum
        {
            ExampleEnum1,
            ExampleEnum2,
            ExampleEnum3
        }

        /// <summary>
        /// 所有的变量和属性都需要把含义或者数据类型写在开头，例如_buttonStartGame，就需要把button写在前面
        /// 变量的含义尽量不要写缩写，button不要写成btn。background不要写成bg。
        /// </summary>
        private Button _buttonStartGame;


        /// <summary>
        /// 对于一些需要强调的数据结构，例如列表，字典，数组等，需要把数据结构写在含义后面
        /// 实际上是把定于前置。例如开始游戏的按钮就写成_buttonStartGame
        /// 而描述玩家位置的列表写成listPlayerPosition
        /// </summary>
        public List<Vector3> listPlayerPosition;




        /// <summary>
        /// 方法前用需要文档注释
        /// 所有方法名都使用大驼峰命名，不管是不是私有
        /// 方法的缩进采用Allman风格
        /// </summary>
        public void Example()
        {
            // 临时变量需要小驼峰，代码内注释尽量不要写在行内，需要换行写
            int tempInt = 0;
            tempInt++;
        }
    }
}