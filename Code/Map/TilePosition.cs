using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TilePosition
{
    public int mX;
    public int mY;
    public TilePosition(int x, int y)
    {
        mX = x;
        mY = y;
    }

    public static TilePosition operator +(TilePosition left, TilePosition right)
    {
        return new TilePosition(left.mX + right.mX, left.mY + right.mY);
    }

    public static TilePosition operator -(TilePosition left, TilePosition right)
    {
        return new TilePosition(left.mX - right.mX, left.mY - right.mY);
    }

    public static bool operator ==(TilePosition left, TilePosition right)
    {
        if (left is null) { return right is null; }
        if (right is null) { return left is null; }
        return (left.mX == right.mX && left.mY == right.mY);
    }

    public static bool operator !=(TilePosition left, TilePosition right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return "(x: " + mX + " | " + "y: " + mY + ")";
    }
}

