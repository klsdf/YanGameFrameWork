using UnityEngine.EventSystems;
using UnityEngine;
using System;
public abstract class YanToggle : MonoBehaviour,IPointerClickHandler
{
    public string groupName;



    //自己当前是否处于激活状态
    protected bool isActive = false;

    protected virtual void Start()
    {

        YanGF.Event.AddListener<string,string>("OnButtonClick",  OnButtonClick);
    }

    protected virtual void OnDestroy()
    {
        YanGF.Event.RemoveListener<string,string>("OnButtonClick",  OnButtonClick);
    }



    public virtual void OnPointerClick(PointerEventData eventData)
    {
        YanGF.Event.TriggerEvent<string,string>("OnButtonClick", groupName,name);


    }



    private void OnButtonClick(string groupName,string name)
    {

        //如果不是自己的组，则不响应
        if(groupName!=this.groupName)
        {
            return;
        }

        if (name == this.name)
        {
            TurnOn();
        }else{
            TurnOff();
        }
    }



    public Action OnClickSelf;
    public Action OnClickOther;





    public void TurnOn()
    {
        isActive = true;
        OnClickSelf();
    }

    public void TurnOff()
    {
        isActive = false;
        OnClickOther();
    }




}
