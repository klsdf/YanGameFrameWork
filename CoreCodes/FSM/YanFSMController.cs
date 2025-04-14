/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 状态机控制器
 *
 ****************************************************************************/

using UnityEngine;
namespace YanGameFrameWork.FSM
{
    public abstract class YanFSMController : MonoBehaviour
    {
        // protected List<YanStateBase> _states;
        protected YanStateBase _currentState;


        private void Update()
        {
            _currentState?.OnUpdate();
        }
        public void ChangeState(YanStateBase newState)
        {

            if (newState == null)
            {
                YanGF.Debug.LogError(nameof(YanFSMController), "newState is null");
                return;
            }
            _currentState?.OnExit();
            // _states.Remove(_currentState);

            _currentState = newState;
            _currentState.OnEnter();
        }
    }
}