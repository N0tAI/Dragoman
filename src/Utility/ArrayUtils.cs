using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Utility
{
    internal static class ArrayUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte[] ResizeArray(byte[] charArray, int modifyBy)
        {
            var newArray = new byte[charArray.Length + modifyBy];
            for (int offset = 0; offset < charArray.Length; offset++)
                newArray[offset] = charArray[offset];
            return newArray;
        }
    }
}
