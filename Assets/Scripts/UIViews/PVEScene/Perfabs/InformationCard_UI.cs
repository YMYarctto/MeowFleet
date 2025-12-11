using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationCard_UI : UIView
{
    public override UIView currentView => this;
    public override int ID => _id;

    int _id = InformationBoard.CardID;

    public override void Init()
    {
        
    }
}
