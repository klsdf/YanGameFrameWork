using YanGameFrameWork.Singleton;
using UnityEngine;
using System.Collections.Generic;
using System;
namespace YanGameFrameWork.TutoriaSystem
{

    public class TutoriaController : Singleton<TutoriaController>
    {


        private TutoriaPanel _tutoriaPanel;

#if USE_LIBTESSDOTNET
        public void FocusOn(List<Transform> targets, Action pauseGame)
        {

            _tutoriaPanel = YanGF.UI.PushPanel<TutoriaPanel>() as TutoriaPanel;
            pauseGame?.Invoke();
            _tutoriaPanel.FocusOn(targets);

        }
#else
        public void FocusOn(Transform target,Action pauseGame)
        {

                _tutoriaPanel = YanGF.UI.PushPanel<TutoriaPanel>() as TutoriaPanel;

            pauseGame?.Invoke();
            _tutoriaPanel.FocusOn(target);
        }
#endif
        public void Hide(Action resumeGame)
        {
            if (_tutoriaPanel != null)
            {
                YanGF.UI.PopPanel<TutoriaPanel>();
                resumeGame?.Invoke();
            }
        }
    }
}
