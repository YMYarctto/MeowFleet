using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship
{
    ShipData shipData;
    LayoutDATA layout;
    LayoutDATA init_layout;
    int _direction = 0;

    public LayoutDATA Layout => layout;
    public LayoutDATA InitLayout => init_layout;
    public int Uid => shipData.uid;

    public Ship(ShipData shipData)
    {
        this.shipData = shipData;
        init_layout = layout = new(shipData.shape_coord);
    }

    public int ResetLayout()
    {
        layout = new(init_layout);
        int value = _direction;
        _direction = 0;
        return value;
    }

    public void Rotate(int direction)
    {
        layout = layout.Rotate(direction);
        _direction += direction;
    }
}
