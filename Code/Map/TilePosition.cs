using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TilePosition
{
    public int mCol;
    public int mRow;
    public TilePosition(int col, int row)
    {
        mCol = col;
        mRow = row;
    }

    public static TilePosition operator +(TilePosition left, TilePosition right)
    {
        return new TilePosition(left.mCol + right.mCol, left.mRow + right.mRow);
    }

    public static TilePosition operator -(TilePosition left, TilePosition right)
    {
        return new TilePosition(left.mCol - right.mCol, left.mRow - right.mRow);
    }

    public static bool operator ==(TilePosition left, TilePosition right)
    {
        if (left is null) { return right is null; }
        if (right is null) { return left is null; }
        return (left.mCol == right.mCol && left.mRow == right.mRow);
    }

    public static bool operator !=(TilePosition left, TilePosition right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return "(column: " + mCol + " | " + "row: " + mRow + ")";
    }

    public int GetModuleDistance()
    {
        return Math.Abs(mCol) + Math.Abs(mRow);
    }
}

