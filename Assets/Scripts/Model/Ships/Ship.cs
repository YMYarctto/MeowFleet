using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship
{
    ShipData shipData;
    LayoutDATA layout;
    LayoutDATA init_layout;

    public LayoutDATA Layout => layout;
    public LayoutDATA InitLayout => init_layout;
    public int Uid => shipData.uid;

    public Ship(ShipData shipData)
    {
        this.shipData = shipData;
        init_layout = layout = new(shipData.shape_coord);
    }

    public void ResetLayout()
    {
        layout = new(init_layout);
    }

    public void Rotate(int direction)
    {
        layout = layout.Rotate(direction);
    }

    public void SetLayout(LayoutDATA layoutDATA)
    {
        layout = layoutDATA;
    }
}
