// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System;
// public class SkillTreeEditor : EditorWindow
// {
//     public GameObject skillButtonPrefab; // 预制体
//     public Canvas container; // 场景中的Canvas

//     private SkillNode _rootSkill;
//     [MenuItem("😁YanGameFrameWork😁/技能树编辑器")]
//     public static void ShowWindow()
//     {
//         GetWindow<SkillTreeEditor>("技能树编辑器");
//     }

//     private void OnGUI()
//     {
//         skillButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Skill Button Prefab", skillButtonPrefab, typeof(GameObject), false);
//         container = (Canvas)EditorGUILayout.ObjectField("Canvas", container, typeof(Canvas), true);

//         if (GUILayout.Button("Clear Canvas"))
//         {
//             ClearCanvas(); // 清除Canvas下的所有子对象
//         }

//         if (GUILayout.Button("Generate Skill Tree UI"))
//         {
//             if (skillButtonPrefab != null && container != null)
//             {
//                 ClearCanvas();
//                 InitializeSkillTree();
//                 // GenerateUI(rootSkill, canvas.transform, 0, 0);
//             }
//             else
//             {
//                 Debug.LogWarning("Please assign both a Skill Button Prefab and a Canvas.");
//             }
//         }
//     }



//     private void ClearCanvas()
//     {
//         if (container != null)
//         {
//             foreach (Transform child in container.transform)
//             {
//                 DestroyImmediate(child.gameObject);
//             }
//         }
//         else
//         {
//             Debug.LogWarning("Canvas is not assigned.");
//         }
//     }


//     private void CreateSkillNode(string name)
//     {


//         if (GameObject.Find(name) != null)
//         {
//             // Debug.LogWarning("SkillNode already exists: " + name);
//             return;
//         }


//         GameObject skillButton = PrefabUtility.InstantiatePrefab(skillButtonPrefab, container.transform) as GameObject;


//     }

//     private void InitializeSkillTree()
//     {
//         CreateSkillNode("初始抽卡花费降低1");
//         CreateSkillNode("初始抽卡花费降低2");
//         CreateSkillNode("初始抽卡花费降低3");
//         CreateSkillNode("初始抽卡花费降低4");
//         CreateSkillNode("初始抽卡花费降低5");
//         CreateSkillNode("抽卡花费增加值降低1");
//         CreateSkillNode("初始人口上限+1");
//         CreateSkillNode("SR卡概率提高x");
//         CreateSkillNode("SSR卡概率提高X");
//         CreateSkillNode("商店卡SR卡概率提高X");
//         CreateSkillNode("商店卡SSR卡概率提高X");



//         // _rootSkill
//         // .AddChild(node2)
//         // .AddChild(node3)
//         // .AddChild(node4)
//         // .AddChild(node5);

//         // node3.AddChild(node6);
//         // node3.AddChild(node8);


//         // node5.AddChild(node7);

//         // node8.AddChild(node9);
//         // node8.AddChild(node10).AddChild(node11);



//     }

//     // private void GenerateUI(SkillNode node, Transform parentTransform, float xOffset, float yOffset)
//     // {
//     //     if (node == null)
//     //     {
//     //         Debug.LogError("SkillNode is null.");
//     //         return;
//     //     }

//     //     if (parentTransform == null)
//     //     {
//     //         Debug.LogError("Parent Transform is null.");
//     //         return;
//     //     }

//     //     if (skillButtonPrefab == null)
//     //     {
//     //         Debug.LogError("Skill Button Prefab is not assigned.");
//     //         return;
//     //     }

//     //     GameObject skillButton = PrefabUtility.InstantiatePrefab(skillButtonPrefab, parentTransform) as GameObject;

//     //     skillButton.name = node.Name;
//     //     if (skillButton == null)
//     //     {
//     //         Debug.LogError("Failed to instantiate skill button prefab.");
//     //         return;
//     //     }

//     //     skillButton.GetComponentInChildren<TMP_Text>().text = node.Name;

//     //     // 设置UI元素的位置
//     //     RectTransform rectTransform = skillButton.GetComponent<RectTransform>();
//     //     rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);

//     //     // 计算子节点的位置
//     //     float childXOffset = xOffset - (node.Children.Count - 1) * 200f / 2;
//     //     float childYOffset = yOffset + 200f;

//     //     foreach (SkillNode child in node.Children)
//     //     {
//     //         GenerateUI(child, parentTransform, childXOffset, childYOffset);
//     //         childXOffset += 150f;
//     //     }
//     // }
// }