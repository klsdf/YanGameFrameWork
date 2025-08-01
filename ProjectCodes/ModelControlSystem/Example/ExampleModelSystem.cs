using YanGameFrameWork.ModelControlSystem;
using UnityEngine;
using System;


public class ExampleModelSystem : MonoBehaviour
{
    [Serializable]
    public class GameRuntimeData : YanModelBase
    {

        private int _money;
        public int Money
        {
            get => _money;
            set
            {
                _money = value;
                NotifyDataChanged(this);
            }
        }

        public override YanModelBase Clone(YanModelBase model)
        {
            GameRuntimeData data = model as GameRuntimeData;
            this._money = data._money;
            return this;
        }

    }



    public int GetMoney()
    {
        return YanGF.Model.GetModel<GameRuntimeData>().Money;
    }

}

