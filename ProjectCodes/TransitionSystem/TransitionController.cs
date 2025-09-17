using YanGameFrameWork.SceneControlSystem;
using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.Singleton;
using System.Threading.Tasks;
using System;  
public class TransitionController : Singleton<TransitionController>
{
   public Image transitionImage;

   public Image CinemaImageUp;
   public Image CinemaImageDown;

   public void FadeIn   (float time = 3f,Action onComplete = null)
   {
      transitionImage.color = new Color(0, 0, 0, 1);
      YanGF.Tween.Tween(transitionImage, t => t.color, new Color(0, 0, 0, 0), time,onComplete);
   }

   public void FadeOut(float time = 3f,Action onComplete = null)  
   {
      transitionImage.color = new Color(0, 0, 0, 0);
      YanGF.Tween.Tween(transitionImage, t => t.color, new Color(0, 0, 0, 1), time,onComplete);
   }


   public async Task FadeInAsync(float time = 3f)
   {
      transitionImage.color = new Color(0, 0, 0, 1);
      await YanGF.Tween.TweenAsync(transitionImage, t => t.color, new Color(0, 0, 0, 0), time);
   }

   public async Task FadeOutAsync(float time=2f)
   {
      transitionImage.color = new Color(0, 0, 0, 0);
      await YanGF.Tween.TweenAsync(transitionImage, t => t.color, new Color(0, 0, 0, 1), time);
   }

   /// <summary>
   /// 电影转场效果 - 上下条带缓缓落下
   /// </summary>
   /// <param name="time">动画时间</param>
   /// <param name="onComplete">完成回调</param>
   public void CinemaIn(float time = 2f, Action onComplete = null)
   {
      // 上条带向下移动100像素
      YanGF.Tween.Tween(CinemaImageUp.transform, t => t.position, 
         CinemaImageUp.transform.position + Vector3.down * 100, time, null);
      
      // 下条带向上移动100像素
      YanGF.Tween.Tween(CinemaImageDown.transform, t => t.position, 
         CinemaImageDown.transform.position + Vector3.up * 100, time, onComplete);
   }

   /// <summary>
   /// 电影转场效果 - 上下条带缓缓收起
   /// </summary>
   /// <param name="time">动画时间</param>
   /// <param name="onComplete">完成回调</param>
   public void CinemaOut(float time = 2f, Action onComplete = null)
   {
      // 上条带向上移动100像素
      YanGF.Tween.Tween(CinemaImageUp.transform, t => t.position, 
         CinemaImageUp.transform.position + Vector3.up * 100, time, null);
      
      // 下条带向下移动100像素
      YanGF.Tween.Tween(CinemaImageDown.transform, t => t.position, 
         CinemaImageDown.transform.position + Vector3.down * 100, time, onComplete);
   }

   /// <summary>
   /// 异步电影转场效果 - 上下条带缓缓落下
   /// </summary>
   /// <param name="time">动画时间</param>
   public async Task CinemaInAsync(float time = 2f)
   {
      // 同时执行两个动画
      var upTask = YanGF.Tween.TweenAsync(CinemaImageUp.transform, t => t.position, 
         CinemaImageUp.transform.position + Vector3.down * 100, time);
      var downTask = YanGF.Tween.TweenAsync(CinemaImageDown.transform, t => t.position, 
         CinemaImageDown.transform.position + Vector3.up * 100, time);
      
      // 等待两个动画都完成
      await Task.WhenAll(upTask, downTask);
   }

   /// <summary>
   /// 异步电影转场效果 - 上下条带缓缓收起
   /// </summary>
   /// <param name="time">动画时间</param>
   public async Task CinemaOutAsync(float time = 2f)
   {
      // 同时执行两个动画
      var upTask = YanGF.Tween.TweenAsync(CinemaImageUp.transform, t => t.position, 
         CinemaImageUp.transform.position + Vector3.up * 100, time);
      var downTask = YanGF.Tween.TweenAsync(CinemaImageDown.transform, t => t.position, 
         CinemaImageDown.transform.position + Vector3.down * 100, time);
      
      // 等待两个动画都完成
      await Task.WhenAll(upTask, downTask);
   }
   
   
}
