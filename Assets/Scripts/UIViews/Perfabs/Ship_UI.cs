using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ship_UI : UIView
{
    public override UIView currentView => this;
    public override int ID {
        get
        {
            _uid++;
            return _uid;
        }
    }

    static int _uid=10000;
    Ship current;

    public override void Init()
    {
        Task task = Task.Run(() =>
        {
            if (current == null)
                Thread.Sleep(100);
        });
        task.Wait();
        
    }
    
    public void SetShip(Ships_Enum id)
    {
        current = new(DataManager.instance.GetShipData(id));
    }
}
