using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TileFill
{
    public bool isTower = false;
    public bool isClear = false;
    public bool isMimic = false;

    public bool IsOccupiedBySomething()
    {
        return isTower || isClear || isMimic;
    }
}

