/****************************************************************************
 * Author: 闫辰祥
 * Date: 2024-12-24
 * Description: 弹幕控制器，最早是用于制作《我思故我在》的弹幕效果
 *
 * 修改记录:
 * 2025-05-10 闫辰祥 增加了弹幕的生成位置的控制,现在可以自由控制弹幕的起始位置了
 ****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.Singleton;

/// <summary>
/// 这个类用于管理弹幕的生成。
/// </summary>
public class BarrageController : Singleton<BarrageController>
{
    public Barrage _barragePrefab; // 弹幕预制件
    private Transform _barrageParent; // 弹幕的父容器
    private List<string> _states; // 状态列表




    List<Barrage> _barrages = new List<Barrage>();



    [Range(0.1f, 5f)]
    public float _updateInterval = 3f; // 每次执行规则的时间间隔

    public MoveDirection _moveDirection = MoveDirection.Left;

    public BarrageXPosition _barrageXPosition = BarrageXPosition.Right;
    public BarrageYPosition _barrageYPosition = BarrageYPosition.Full;


    [Range(60, 150)]
    public float barrageSpeedMin = 60f;
    [Range(60, 150)]
    public float barrageSpeedMax = 150f;

    [Range(20, 100)]
    public int fontSizeMin = 40;
    [Range(20, 100)]
    public int fontSizeMax = 80;

    public enum BarrageXPosition
    {
        Left,
        Center,
        Right,
        Full
    }

    public enum BarrageYPosition
    {
        Top,
        Center,
        Bottom,
        Full
    }


    public enum MoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private void Start()
    {
        _barrageParent = transform;
        _states = new List<string>(鸣谢名单.鸣谢名单列表);

        StartGeneratingBarrage();
    }


    /// <summary>
    /// 开始生成弹幕。
    /// </summary>
    public void StartGeneratingBarrage()
    {
        StartCoroutine(GenerateBarrage());
    }

    /// <summary>
    /// 生成弹幕。
    /// </summary>
    private IEnumerator GenerateBarrage()
    {
        while (true)
        {
            if (_states != null && _states.Count > 0)
            {

                // 从states中随机选择一个状态
                string randomState = _states[Random.Range(0, _states.Count)];
                GenerateBarrage(randomState, _barrageXPosition, _barrageYPosition);
            }

            // 每隔3秒生成一个弹幕
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    /// <summary>
    /// 生成单个弹幕。
    /// </summary>
    public Barrage GenerateBarrage(string text, BarrageXPosition xPos, BarrageYPosition yPos)
    {
        GameObject barrageObject = Instantiate(_barragePrefab.gameObject, _barrageParent);

        Vector3 spawnPos = GetSpawnPosition(xPos, yPos);
        barrageObject.transform.position = spawnPos;

        Barrage barrage = barrageObject.GetComponent<Barrage>();
        if (barrage != null)
        {
            barrage.Init(text, Random.Range(fontSizeMin, fontSizeMax), Random.Range(barrageSpeedMin, barrageSpeedMax));
        }
        _barrages.Add(barrage);
        return barrage;
    }

    private Vector3 GetSpawnPosition(BarrageXPosition xPos, BarrageYPosition yPos)
    {
        float x = 0f, y = 0f;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // X方向
        switch (xPos)
        {
            case BarrageXPosition.Left:
                x = Random.Range(0, screenWidth / 3f);
                break;
            case BarrageXPosition.Center:
                x = Random.Range(screenWidth / 3f, screenWidth * 2f / 3f);
                break;
            case BarrageXPosition.Right:
                x = Random.Range(screenWidth * 2f / 3f, screenWidth);
                break;
            case BarrageXPosition.Full:
                x = Random.Range(0, screenWidth);
                break;
        }

        // Y方向
        switch (yPos)
        {
            case BarrageYPosition.Top:
                y = Random.Range(screenHeight * 2f / 3f, screenHeight);
                break;
            case BarrageYPosition.Center:
                y = Random.Range(screenHeight / 3f, screenHeight * 2f / 3f);
                break;
            case BarrageYPosition.Bottom:
                y = Random.Range(0, screenHeight / 3f);
                break;
            case BarrageYPosition.Full:
                y = Random.Range(0, screenHeight);
                break;
        }

        return new Vector3(x, y, 0f);
    }

    private void Update()
    {
        if (_barrages.Count == 0) return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 用临时列表存储待移除弹幕，避免遍历时修改集合
        List<Barrage> toRemove = new List<Barrage>();

        foreach (var barrage in _barrages)
        {
            if (barrage == null) { toRemove.Add(barrage); continue; }

            float delta = barrage.speed * Time.deltaTime; // 用每个弹幕自己的速度
            Vector3 pos = barrage.transform.position;
            switch (_moveDirection)
            {
                case MoveDirection.Left:
                    pos.x -= delta;
                    break;
                case MoveDirection.Right:
                    pos.x += delta;
                    break;
                case MoveDirection.Up:
                    pos.y += delta;
                    break;
                case MoveDirection.Down:
                    pos.y -= delta;
                    break;
            }
            barrage.transform.position = pos;

            // 判断是否超界
            bool outOfScreen = false;
            switch (_moveDirection)
            {
                case MoveDirection.Left:
                    if (pos.x < -500f) outOfScreen = true;
                    break;
                case MoveDirection.Right:
                    if (pos.x > screenWidth + 500f) outOfScreen = true;
                    break;
                case MoveDirection.Up:
                    if (pos.y > screenHeight + 500f) outOfScreen = true;
                    break;
                case MoveDirection.Down:
                    if (pos.y < -500f) outOfScreen = true;
                    break;
            }
            if (outOfScreen)
            {
                Destroy(barrage.gameObject);
                toRemove.Add(barrage);
            }
        }

        // 移除已销毁的弹幕
        foreach (var barrage in toRemove)
        {
            _barrages.Remove(barrage);
        }
    }
}