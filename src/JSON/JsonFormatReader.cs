using Dragoman.Intermediate;
using Dragoman.Encoding;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

// Parse decimal numbers
// Parse doubles
//  Parse exponent numbers
//  Parse non exponents
// Parse unicode escape characters

namespace Dragoman.JSON
{
    /// <summary>
    /// Defines a controller object, capable of directing encoding parsers and reader implementations to read and parse JSON files to CIL objects
    /// </summary>
    // This is the middleman application bridging the public parser type and the low level byte to object readers
    public struct JsonFormatReader : IFormatReader
    {
        private static readonly byte[] _bom = new byte[] { 0xEF, 0xBB, 0xBF }; // Abstract bom detection to a type of itself for automatic file detection

        private readonly IIntermediateWriter _intermediateWriter;
        private readonly IEncodingParser _textParser;
        private readonly IEncodingReader _encodingReader;

        public JsonFormatReader(IIntermediateWriter intermediateWriter, IEncodingParser valueParser, IEncodingReader encodingReader)
        {
            _intermediateWriter = intermediateWriter;
            _textParser = valueParser;
            _encodingReader= encodingReader;
        }

        public IIntermediateWriter ReadBytes(byte[] bytes, int offset)
        {
            object[] objects = new object[2];
            byte currentObjectIndex = 0;
            if (bytes.Length > 2)
                if (bytes[0] == _bom[0] && bytes[1] == _bom[1] && bytes[2] == _bom[2])
                    offset += 3;
            // TODO
            // Check if pair and store appropriately
            while (offset < bytes.Length)
            {
                offset = SkipWhitespace(bytes, offset);
                var currentByte = bytes[offset];
                switch (currentByte)
                {
                    case 0x5B: // [
                        string aName = objects[0] is string strOne ? strOne : "";
                        _intermediateWriter.AddArray(aName);
                        continue;
                    case 0x7B: // {
                        string sName = objects[0] is string strTwo ? strTwo : "";
                        _intermediateWriter.AddStruct(sName);
                        continue;
                    case 0x5D: // ]
                    case 0x7D: // }
                        _intermediateWriter.AddIntermediateObject(UnpackParsedObjects(objects));
                        _intermediateWriter.CloseCurrentContainer();
                        goto clearProcessedObjects;
                    case 0x3A: // :
                    case 0x2C: // ,
                        _intermediateWriter.AddIntermediateObject(UnpackParsedObjects(objects));
                        goto clearProcessedObjects;
                    case 0x22: // "
                        objects[currentObjectIndex] = ReadString(bytes, ref offset);
                        currentObjectIndex++;
                        continue;
                    case 0x2B: // +
                    case 0x2D: // -
                    case 0x30: // 0
                    case 0x31: // 1
                    case 0x32: // 2
                    case 0x33: // 3
                    case 0x34: // 4
                    case 0x35: // 5
                    case 0x36: // 6
                    case 0x37: // 7
                    case 0x38: // 8
                    case 0x39: // 9
                    case 0x5C: // \
                        objects[currentObjectIndex] = ReadNumber(bytes, ref offset);
                        currentObjectIndex++;
                        continue;
                    case 0x46: // F
                    case 0x54: // T
                    case 0x66: // f
                    case 0x74: // t
                        objects[currentObjectIndex] = ReadBoolean(bytes, ref offset);
                        currentObjectIndex++;
                        continue;
                    case 0x4E: // N
                    case 0x6E: // n
                        objects[currentObjectIndex] = ReadNull(bytes, ref offset);
                        currentObjectIndex++;
                        continue;
                    default:
                        throw new Exception("Invalid JSON character");
                        clearProcessedObjects:
                        objects = new object[2];
                        currentObjectIndex = 0;
                        continue;
                }

            }
            return _intermediateWriter;
        }

        private string ReadString(byte[] data, ref int offset)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool isEscaped = false;
            byte[] currentCharacters;
            byte currentByte = data[++offset];
            while(currentByte != 0x22 && !isEscaped)    // 0x22 = "
            {
                if (currentByte >= 0xF0)
                    currentCharacters = UTF8.GetCharsToCount(data, ref offset, 4);
                else if (currentByte >= 0xE0)
                    currentCharacters = UTF8.GetCharsToCount(data, ref offset, 3);
                else if (currentByte >= 0xC0)
                    currentCharacters = UTF8.GetCharsToCount(data, ref offset, 2);
                else
                {
                    if (currentByte == 0x5C)
                        currentCharacters = ReadEscapeCharacter(data, ref offset);
                    currentCharacters = UTF8.GetCharsToCount(data, ref offset, 1);
                } 
                stringBuilder.Append(Encoding.UTF8.GetChars(currentCharacters)); // Investigate for something potentially higher in performance
                currentByte = data[offset];
            }
            return stringBuilder.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] ReadEscapeCharacter(byte[] data, ref int offset) // Abstract to an interface
        {
            var currentByte = data[++offset];
            switch(currentByte)
            {
                case 0x22:  // "
                    return new[] { (byte)0x22 };
                case 0x27:  // '
                    return new[] { (byte)0x27 };
                case 0x2F:  // /
                    return new[] { (byte)0x2F };
                case 0x5C:  // \
                    return new[] { (byte)0x5C };
                case 0x62:  // b
                case 0x42:  // B
                    return new[] { (byte)0x08 };    // backspace
                case 0x66:  // f
                case 0x46:  // F
                    return new[] { (byte)0x0C };    // Form Feed
                case 0x6E:  // n
                case 0x4E:  // N
                    return new[] { (byte)0x0A };    // New line
                case 0x72:  // r
                case 0x52:  // R
                    return new[] { (byte)0x0D };    // Carriage return
                case 0x74:  // t
                case 0x54:  // T
                    return new[] { (byte)0x09 };    // Horizontal tab
                case 0x75: // u
                case 0x55: // U
                    return UTF8.ReadCodePoint(data, ref offset, 4);
                case 0x76:  // v
                case 0x56:  // V
                    return new[] { (byte)0x0B };    // Vertical tab
                default:
                    throw new ArgumentOutOfRangeException("Invalid escape character in string");
            }
        }

        private object ReadNumber(byte[] data, ref int offset) // Abstract this to an interface
        {
            bool isFloatingPoint = false;
            bool isNegative = false;
            ulong parsedNumber = 0;
            var currentByte = data[++offset];
            if (currentByte == 0x2D) // -
            {
                isNegative = true;
                parsedNumber = 0x1000000000000000;
            }
            if (currentByte == 0x2E) // . 
            {
                throw new ArgumentException("Invalid JSON token, '.' has no leading zero and has been interpreted as a number");
                //currentByte = data[offset++];
            }
            else if(currentByte == 0x2B) // +
                currentByte = data[offset];
            while(currentByte == 0x30) // 0
                currentByte = data[++offset];
            if (currentByte == 0x58 || currentByte == 0x78)         // 'X'
                parsedNumber = _textParser.ParseHexNumber(data, ref offset);
            else if (currentByte == 0x4F || currentByte == 0x6F)    // 'O'
                parsedNumber = _textParser.ParseOctalNumber(data, ref offset);
            else if (currentByte == 0x42 || currentByte == 0x62)    // 'B'
                parsedNumber = _textParser.ParseBinaryNumber(data, ref offset);
            else
                // Requires checking and parsing for a floating point value
                parsedNumber = _textParser.ParseDecimalNumber(data, ref offset);
            if (isNegative)
                parsedNumber |= 0x8000000000000000; 
            return parsedNumber;
        }

        

        private bool ReadBoolean(byte[] data, ref int offset) // Abstract this to an interface
        {
            byte[] bytes;
            int tempOffset = offset;
            if (data[offset] == 0x54 || data[offset] == 0x74)
            {
                bytes = _textParser.GetCharsToCount(data, ref tempOffset, 4);
                if (bytes[1] == 0x52 || bytes[1] == 0x72)
                    if (bytes[2] == 0x55 || bytes[2] == 0x75)
                        if (bytes[3] == 0x45 || bytes[3] == 0x65)
                        {
                            offset = tempOffset;
                            return true;
                        }
            }
            else if (data[offset] == 0x46 || data[offset] == 0x66)
            {
                bytes = _textParser.GetCharsToCount(data, ref tempOffset, 5);
                if (bytes[1] == 0x41 || bytes[1] == 0x61)
                    if (bytes[2] == 0x4C || bytes[2] == 0x6C)
                        if (bytes[3] == 0x53 || bytes[3] == 0x73)
                            if (bytes[4] == 0x45 || bytes[4] == 0x65)
                            {
                                offset = tempOffset;
                                return false;
                            }
            }
            throw new Exception("Invalid JSON token");
        }

        private object ReadNull(byte[] data, ref int offset) // Abstract this to an interface
        {
            int tempOffset = offset;
            var bytes = GetCharsToCount(data, ref tempOffset, 4);
            if(bytes[0] == 0x4E || bytes[0] == 0x6E)
                if(bytes[1] == 0x55 || bytes[1] == 0x75)
                    if(bytes[2] == 0x4C || bytes[2] == 0x6C)
                        if (bytes[3] == 0x4C || bytes[3] == 0x6C)
                        {
                            offset = tempOffset;
                            return default(object);
                        }
            throw new Exception("Invalid token");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhitespace(byte[] data, int offset) // Abstract this to an interface
        {
            for(int i = offset; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case 0x09: // Horizontal Tab
                    case 0x0A: // New Line
                    case 0x0B: // Vertical Tab
                    case 0x0C: // Form Feed
                    case 0x0D: // Carriage Return
                    case 0x20: // Space
                        continue;
                    case 0x2F:
                        i = SkipComment(data, i);
                        continue;
                    // Add other whitespace types in UTF-8?
                    default:
                        return i;
                }
            }
            return data.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipComment(byte[] data, int offset) // Abstract this to an interface by using a read to
        {
            if (data[offset + 1] == 0xAF)
                while (offset < data.Length && data[offset] != 0x0A)
                    offset++;
            return offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (object, object) UnpackParsedObjects(object[] objects) // Keep
        {
            var firstObject = objects[0];
            var secondObject = default(object);
            if (objects.Length > 1)
                secondObject = objects[1];
            return (firstObject, secondObject);
        }

        public IIntermediateWriter ReadStream(Stream data, int offset)
        {
            return _intermediateWriter;
        }
    }
}