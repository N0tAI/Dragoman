using System;
using System.IO;

namespace Dragoman.Encoding
{
    /// <summary>
    /// Defines a helper object to be used for parsing a UTF-8 encoded bytes into values
    /// </summary>
    internal struct UTF8Parser : IEncodingParser
    {
        public byte[] ParseCodePoint(byte[] data, ref int offset, byte maxDigits)
        {
            var currentByte = data[++offset];
            byte digitCount = 0;
            if (currentByte == 0x2B) // +
                currentByte = data[++offset];
            while (currentByte == 0x30) // 0
            {
                digitCount++;
                currentByte = data[++offset];
            }
            byte[]? utfBytes = null;
            var firstValue = (byte)(currentByte < 0x3A ? currentByte - 0x30 : currentByte < 0x47 ? currentByte - 0x37 : currentByte - 0x57);
            if (digitCount == 0)
                utfBytes = new byte[] { 0xE0, 0x80, 0x80 };
            else if (digitCount == 1)
            {
                if (firstValue > 7)
                    utfBytes = new byte[] { 0xE0, 0x80, 0x80 };
                else
                    utfBytes = new byte[] { 0xC0, 0x80 };
            }
            else if (digitCount == 2)
            {
                if (firstValue > 7)
                    utfBytes = new byte[] { 0xC0, 0x80 };
            }
            utfBytes ??= new byte[] { 0x0 };
            int codePoint = 0;
            int whileIteration = 0;
            while (true)
            {
                whileIteration++;
                if (++digitCount > maxDigits)
                    break;
                if ((currentByte > 0x66 || currentByte < 0x61) && (currentByte > 0x46 || currentByte < 0x41) && (currentByte > 0x39 && currentByte < 0x30))
                    throw new InvalidDataException("The given data has an invalid character at a code point");
                codePoint <<= 4;
                codePoint |= (byte)(currentByte < 0x3A ? currentByte - 0x30 : currentByte < 0x47 ? currentByte - 0x37 : currentByte - 0x57);
                if (data.Length == ++offset)
                    break;
                currentByte = data[offset];
            }
            const byte TRAILING_BYTE_MASK = 0b10111111;
            switch (utfBytes.Length)
            {
                case 1:
                    utfBytes[0] = (byte)codePoint;
                    return utfBytes;
                case 2:
                    {
                        var byteValue = (byte)codePoint;
                        byteValue &= TRAILING_BYTE_MASK;
                        utfBytes[1] |= byteValue;
                        codePoint >>= 6;
                        utfBytes[0] |= (byte)codePoint;
                    }
                    return utfBytes;
                case 3:
                    {
                        var byteValue = (byte)codePoint;
                        byteValue &= TRAILING_BYTE_MASK;
                        utfBytes[2] |= byteValue;
                        byteValue = (byte)(codePoint >>= 6);
                        byteValue &= TRAILING_BYTE_MASK;
                        utfBytes[1] |= byteValue;
                        utfBytes[0] |= (byte)(codePoint >>= 6);
                    }
                    return utfBytes;
                default:
                    throw new InvalidDataException("The given data value exceeds the current legal number of UTF8 characters");
            }
        }

        public ulong ParseHexNumber(byte[] data, ref int offset)
        {
            const byte ULONG_MAX_HEX_DIGITS = 16; // ulong max is 0xffffffffffffffff
            ulong parsedNumber = 0;
            byte currentHexDigitCount = 0;
            byte currentByte = data[++offset];
            while (currentByte == 0x30) // 0
                currentByte = data[++offset];
            while (true)
            {
                if (currentByte == 0x5F) // _
                {
                    currentByte = data[++offset];
                    continue;
                }
                if ((currentByte > 0x66 || currentByte < 0x61) && (currentByte > 0x46 || currentByte < 0x41) && (currentByte > 0x39 && currentByte < 0x30))
                    return parsedNumber;
                if (++currentHexDigitCount > ULONG_MAX_HEX_DIGITS)
                    throw new OverflowException("The given hex number exceeds the maximum value of a ulong and cannot be parsed.");
                parsedNumber <<= 4;
                parsedNumber |= (byte)(currentByte < 0x3A ? currentByte - 0x30 : currentByte < 0x47 ? currentByte - 0x37 : currentByte - 0x57);
                if (data.Length == ++offset)
                    return parsedNumber;
                currentByte = data[offset];
            }
        }

        // NEEDS IMPLEMENTING
        public ulong ParseDecimalNumber(byte[] data, ref int offset)
        {
            throw new NotImplementedException();
        }

        public ulong ParseOctalNumber(byte[] data, ref int offset)
        {
            const byte ULONG_MAX_OCT_DIGITS = 22; // ulong max is 0o1777777777777777777777
            ulong parsedNumber = 0;
            byte leadingByte = 0;
            byte currentOctDigitCount = 0;
            byte currentByte = data[++offset];
            while (currentByte == 0x30) // 0
                currentByte = data[++offset];
            leadingByte = (byte)(currentByte - 0x2F);
            while (true)
            {
                if (currentByte == 0x5F) // _
                {
                    currentByte = data[++offset];
                    continue;
                }
                if (currentByte > 0x37 || currentByte < 0x30) // 0-7
                    return parsedNumber;
                if (++currentOctDigitCount > ULONG_MAX_OCT_DIGITS || (currentOctDigitCount == ULONG_MAX_OCT_DIGITS && leadingByte > 1))
                    throw new OverflowException("The given Octal number exceeds the maximum value of a ulong and cannot be parsed.");
                parsedNumber <<= 3;
                parsedNumber |= (ulong)currentByte - 0x30;
                if (data.Length == ++offset)
                    return parsedNumber;
                currentByte = data[offset];
            }
        }

        public ulong ParseBinaryNumber(byte[] data, ref int offset)
        {
            const byte ULONG_MAX_BIN_DIGITS = 64; // ulong max is 0b1111111111111111111111111111111111111111111111111111111111111111
            ulong parsedNumber = 0;
            byte currentBinDigitCount = 0;
            byte currentByte = data[++offset];
            while (currentByte == 0x30)
                currentByte = data[++offset];
            while (true)
            {
                if (currentByte == 0x5F) // _
                {
                    currentByte = data[++offset];
                    continue;
                }
                if (currentByte < 0x30 || currentByte > 0x31)
                    return parsedNumber;
                if (++currentBinDigitCount > ULONG_MAX_BIN_DIGITS)
                    throw new OverflowException("The given binary number exceeds the maximum value of a ulong and cannot be parsed.");
                parsedNumber <<= 1;
                parsedNumber |= (byte)(currentByte - 0x30);
                if (data.Length == ++offset)
                    return parsedNumber;
                currentByte = data[offset];
            }
        }
    }
}
