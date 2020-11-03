using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ByteAPI
{

    ///////////////////////////////////////////////////////////////////////////////////
    //// THIS API CONTAINS FUNCTIONS RELATED TO READING AND WRITING BITS AND BYTES ////
    ///////////////////////////////////////////////////////////////////////////////////



    /// <summary>
    /// Gets a bit from a specified index of a byte of data
    /// </summary>
    /// <param name="b"> The byte that contains the bit we're looking for </param>
    /// <param name="index"> The position in the byte that we want to get the bit from (Note: first index is 0) </param>
    /// <returns> True if the bit from specified index is 1, otherwise 0 </returns>
    public static bool GetBitAtByteIndex(byte b, int index)
    {
        return (b & (1 << index)) > 0 ? true : false;
    }

    /// <summary>
    /// Create a byte from 8 bool values, where true == 1, and false == 0
    /// </summary>
    /// <param name="bit8"> The leftmost bit in the byte. </param>
    /// <param name="bit7"> The 2nd leftmost bit in the byte. </param>
    /// <param name="bit6"> The 3rd leftmost bit in the byte. </param>
    /// <param name="bit5"> The 4th leftmost bit in the byte. </param>
    /// <param name="bit4"> The 4th rightmost bit in the byte. </param>
    /// <param name="bit3"> The 3rd rightmost bit in the byte. </param>
    /// <param name="bit2"> The 2nd rightmost bit in the byte. </param>
    /// <param name="bit1"> The rightmost bit in the byte. </param>
    /// <returns> The byte made by combining all 8 bits </returns>
    public static byte GetByteFromBits(bool bit8, bool bit7, bool bit6, bool bit5, bool bit4, bool bit3, bool bit2, bool bit1)
    {
        byte b = 0b_00000000;
        if (bit8) b = (byte)(b | (1 << 7));
        if (bit7) b = (byte)(b | (1 << 6));
        if (bit6) b = (byte)(b | (1 << 5));
        if (bit5) b = (byte)(b | (1 << 4));
        if (bit4) b = (byte)(b | (1 << 3));
        if (bit3) b = (byte)(b | (1 << 2));
        if (bit2) b = (byte)(b | (1 << 1));
        if (bit1) b = (byte)(b | (1));
        return b;
    }
}
