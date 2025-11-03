using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship
{
    ShipData shipData;
    LayoutDATA layout;

    public LayoutDATA Layout => layout;
    public string Name => shipData.ship_name_string;

    public Ship(ShipData shipData)
    {
        this.shipData = shipData;
        layout = new(shipData.shape_coord);
    }

    public void Rotate(int direction)
    {
        layout = layout.Rotate(direction);
    }
}
