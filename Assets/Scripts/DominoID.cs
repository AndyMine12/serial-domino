using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Two-integer ID used to identify a domino piece
public class DominoID
{
    //Domino pieces' id represent them as a two-integer array: bottomvalue first, then topvalue. If horizontal: Right value first, then left value
    private int[] _id; 
    private void checkValue(int[] tryId)
    {
        foreach (var value in tryId)
        {
            if ( (value < 0)||(value > 6) )
            {
                throw new System.ArgumentOutOfRangeException(nameof(value), "Dominoes' values range from blank (0) to 6");
            }
        }
        if (tryId.Length != 2)
        {
            throw new System.ArgumentOutOfRangeException(nameof(tryId), "Dominoes are identified by a two-integer array");
        }
    }

    public DominoID (int[] id)
    {
        try 
        {
            this.checkValue(id);
            this._id = id;
        }
        catch (System.Exception e) 
        {
            Debug.Log("Invalid Domino ID. ERROR: " + e.Message);
        }
    }

    // Getters and Setters
    public int[] Value 
    {
        get => this._id;
        set 
        { 
            try
            {
                this.checkValue(value);
                this._id = value;
            }
            catch (System.Exception e)
            {
                Debug.Log("Invalid Domino ID. ERROR: " + e.Message);
            }
        }
    }
    public int ConvertInt
    {
        get 
        {
            if (this._id[0] == 0)
            {
                return (8 * (this._id[1] - 1));
            }
            else
            {
                if (this._id[1] != 0)
                {
                    return (8 * (this._id[0] - 1) + (this._id[1] + 1));
                }
                else
                {
                    return 48; //Double-blank converts to int as 48 (topmost value)
                }
            }
        }
        set
        {
            if ( (value < 48)&&(value>=0) )
            {
                int bottomVal;
                int topVal;
                int remain = value % 8;
                if (remain != 0)
                {
                    topVal = (remain) - 1;
                    bottomVal = (value - (remain))/8 + 1;
                }
                else
                {
                    topVal = value/8 + 1;
                    bottomVal = 0;
                }
                this.Value = new int[2] {bottomVal, topVal};
            }
            else if (value == 48) //Is double-blank
            {
                this.Value = new int[2] {0,0};
            }
            else
            {
                throw new System.ArgumentOutOfRangeException(nameof(value), "Dominoes are represented by integers 0 through 48");
            }
        }
    }

    public int TopValue
    {
        get => this._id[1];
    }
    public int BottomValue
    {
        get => this._id[0];
    }

    //Converts this ID to a String
    public override string ToString()
    {
        return this._id[0].ToString() + this._id[1].ToString(); 
    }
}
