using System.Text;

namespace Dragoman.ConsoleTests
{
    public class DragomanConsoleTest
    {
        public static void Main(string[] args)
        {
            var testText = args.Length > 0 ? args[0] : "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+={[]}\\|;:'\",<.>/?`~";
            Console.WriteLine($"Reading string: {testText}\nString End.");
            UTF8Reader reader = new UTF8Reader();
            var textBytes = Encoding.UTF8.GetBytes(testText);
            var offset = -1;
            var textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"First text chunk 0 - 9: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Second text chunk 10 - 19: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Third text chunk 20 - 29: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Fourth text chunk 30 - 39: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Fifth text chunk 40 - 49: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Sixth text chunk 50 - 59: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Seventh text chunk 60 - 69: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Eighth text chunk 70 - 79: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Ninth text chunk 80 - 89: {Encoding.UTF8.GetString(textChunk)}");
            textChunk = reader.GetCharsToCount(textBytes, ref offset, 10);
            Console.WriteLine($"Tenth text chunk 90 - 99: {Encoding.UTF8.GetString(textChunk)}");
        }
    }

    internal struct UTF8Reader
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
            while (true)
            {
                if (data.Length <= offset + consumedBytes)
                    return;
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

        public byte[] GetCharsToCount(byte[] data, ref int offset, int count)
        {
            byte[] ResizeArray(byte[] charArray, int modifyBy)
            {
                var newArray = new byte[charArray.Length + modifyBy];
                for (int offset = 0; offset < charArray.Length; offset++)
                    newArray[offset] = charArray[offset];
                return newArray;
            }
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
                                capturedChars = ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 3;
                            break;
                        }
                    case > 0xDF:
                        {
                            var newCharSize = readingIndex + 3;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 2;
                            break;
                        }
                    case > 0xBF:
                        {
                            var newCharSize = readingIndex + 2;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ResizeArray(capturedChars, newCharSize - capturedChars.Length);
                            count += 1;
                            break;
                        }
                    default:
                        {
                            var newCharSize = readingIndex + 1;
                            if (newCharSize >= capturedChars.Length)
                                capturedChars = ResizeArray(capturedChars, newCharSize - capturedChars.Length);
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