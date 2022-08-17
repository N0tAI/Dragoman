using Dragoman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Encoding
{
    internal struct UTF8Reader : IEncodingReader
    {
        private int[] _whitespaceBytes;
        public UTF8Reader()
        {
            _whitespaceBytes = new int[0];
        }
        public void DefineWhitespace(params int[] whitespaceBytes)
        {
            var whitespaceBytesTotal = whitespaceBytes.Length + _whitespaceBytes.Length;
            int[] newWhitespaceBytes = new int[whitespaceBytesTotal];
            int currentIndex = 0;
            for (; currentIndex < _whitespaceBytes.Length; currentIndex++)
                newWhitespaceBytes[currentIndex] = _whitespaceBytes[currentIndex];
            for (; currentIndex < whitespaceBytes.Length; currentIndex++)
                newWhitespaceBytes[currentIndex] = whitespaceBytes[currentIndex];
            _whitespaceBytes = newWhitespaceBytes;
        }

        public void SkipWhitespace(byte[] data, ref int offset)
        {
            if (data.Length <= ++offset)
                return;
            uint currentReadingBytes = 0;
            byte consumedBytes = 0;
            IEnumerable<int> whitespaces = _whitespaceBytes;
            while (data.Length <= offset + consumedBytes)
            {
                currentReadingBytes |= data[offset + consumedBytes++];
                var comparisonValue = currentReadingBytes << (4 - consumedBytes) * 8;
                whitespaces = whitespaces.Where(whitespace => (whitespace << (4 - consumedBytes) * 8) == comparisonValue);
                if (whitespaces.Any(whitespace => whitespace == currentReadingBytes))
                {
                    currentReadingBytes = 0;
                    offset += consumedBytes;
                    consumedBytes = 0;
                    whitespaces = _whitespaceBytes;
                }
                else if (consumedBytes == 4)
                    return;
                else
                    currentReadingBytes <<= 8;
            }
        }

        public byte[] GetNextCharsToCount(byte[] data, ref int offset, int count)
        {
            byte[] capturedChars = new byte[count];
            byte currentByte;
            var readingIndex = 0;
            while (count != 0)
            {
                if (data.Length == ++offset)
                    return capturedChars;
                currentByte = data[offset];
                switch (currentByte)
                {
                    case > 0xEF:
                        {
                            var newCharSize = readingIndex + 4;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ArrayUtils.ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 3;
                            break;
                        }
                    case > 0xDF:
                        {
                            var newCharSize = readingIndex + 3;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ArrayUtils.ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 2;
                            break;
                        }
                    case > 0xBF:
                        {
                            var newCharSize = readingIndex + 2;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ArrayUtils.ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 1;
                            break;
                        }
                    default:
                        {
                            var newCharSize = readingIndex + 1;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ArrayUtils.ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            break;
                        }
                }
                capturedChars[readingIndex++] = currentByte;
                count--;
            }
            return capturedChars;
        }

    }
}
