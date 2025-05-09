using YanGameFrameWork.UISystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace YanGameFrameWork.TutoriaSystem
{
    public class TutoriaPanel : UIPanelBase
    {
        public MaskedUI _maskedUI;



#if USE_LIBTESSDOTNET
        public void FocusOn(List<Transform> targets)
        {

            _maskedUI.SetTargets(targets);
            _maskedUI.HighlightTargets();

        }
#else

        public void FocusOn(RectTransform target)
        {
            _maskedUI.SetTarget(target);
            _maskedUI.HighlightTarget();
        }
#endif

    }




}