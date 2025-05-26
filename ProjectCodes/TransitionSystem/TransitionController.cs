using YanGameFrameWork.SceneControlSystem;
using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.Singleton;
using System.Threading.Tasks;

public class TransitionController : Singleton<TransitionController>
{
   public Image transitionImage;

   public void FadeIn(float time=3f)
   {
      transitionImage.color = new Color(0, 0, 0, 1);
      YanGF.Tween.Tween(transitionImage, t => t.color, new Color(0, 0, 0, 0), time);
   }

   public void FadeOut(float time=2f)
   {
      transitionImage.color = new Color(0, 0, 0, 0);
      YanGF.Tween.Tween(transitionImage, t => t.color, new Color(0, 0, 0, 1), time);
   }
   
   
}
