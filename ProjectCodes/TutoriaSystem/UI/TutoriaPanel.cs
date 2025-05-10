using YanGameFrameWork.UISystem;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace YanGameFrameWork.TutoriaSystem
{
    public class TutoriaPanel : UIPanelBase
    {
        public MaskedUI _maskedUI;

        public override void OnLocalize()
        {

        }

#if USE_LIBTESSDOTNET
        public void FocusOn(List<Transform> targets)
        {

            _maskedUI.SetTargets(targets);
            _maskedUI.HighlightTarget();

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