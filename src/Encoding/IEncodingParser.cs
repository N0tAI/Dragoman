using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Encoding
{
    /// <summary>
    /// When implemented will define a parser that can iterate and convert raw text bytes into CIL values.
    /// </summary>
    public interface IEncodingParser
    {
        /// <summary>
        /// When implemented, converts a unicode codepoint into raw text bytes.
        /// </summary>
        /// <param name="data">The collected data which contains the codepoint.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <param name="maxDigits">The maximum allowed Hex digits of the unicode code point.</param>
        /// <returns>The parsed Unicode codepoint converted into it's raw encoded bytes.</returns>
        byte[] ParseCodePoint(byte[] data, ref int offset, byte maxDigits);
        /// <summary>
        /// When implemented, converts an encoded string, formatted as a hexadecimal number (0-F), into a number.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <returns>The parsed number as a <see cref="ulong"/>.</returns>
        ulong ParseHexNumber(byte[] data, ref int offset);
        /// <summary>
        /// When implemented, converts an encoded string, formatted as a decimal number (0-9), into a number.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <returns>The parsed number as a <see cref="ulong"/>.</returns>
        ulong ParseDecimalNumber(byte[] data, ref int offset);
        /// <summary>
        /// When implemented, converts an encoded string, formatted as a octadecimal number (0-7), into a number.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <returns>The parsed number as a <see cref="ulong"/>.</returns>
        ulong ParseOctalNumber(byte[] data, ref int offset);
        /// <summary>
        /// When implemented, converts an encoded string, formatted as a binary number (0-1), into a number.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <returns>The parsed number as a <see cref="ulong"/>.</returns>
        ulong ParseBinaryNumber(byte[] data, ref int offset);
    }
}
