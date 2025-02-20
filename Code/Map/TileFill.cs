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

    public TileFill()
    {

    }

    public TileFill(bool isTower, bool isClear, bool isMimic)
    {
        this.isTower = isTower;
        this.isClear = isClear;
        this.isMimic = isMimic;
    }

    public bool CanSpawnMimic()
    {
        return !isTower && !isClear && !isMimic;
    }

    public static bool operator ==(TileFill a, TileFill b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return (a.isTower == b.isTower) && (a.isClear == b.isClear) && (a.isMimic == b.isMimic);
    }

    public static bool operator !=(TileFill a, TileFill b)
    {
        return !(a == b);
    }
}

