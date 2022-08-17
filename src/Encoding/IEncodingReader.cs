namespace Dragoman.Encoding
{
    /// <summary>
    /// When implemented, defines a reader object capable of iterating and navigating across a set of encoded bytes
    /// </summary>
    public interface IEncodingReader
    {
        /// <summary>
        /// When implemented, will specify to the <see cref="IEncodingReader"/> values the calling <see cref="IFormatReader"/> considers whitespace.
        /// </summary>
        /// <param name="whiteSpaceBytes">Raw byte sequence considered to be white space.</param>
        public void DefineWhitespace(params int[] whiteSpaceBytes);
        /// <summary>
        /// When implemented, will skip any designated whitespace encountered in <paramref name="data"/> until it finds the next non whitespace.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>, value will be updated to the next non whitespace position after method call.</param>
        public void SkipWhitespace(byte[] data, ref int offset);
        /// <summary>
        /// When implemented, grabs the next <paramref name="count"/> characters.
        /// </summary>
        /// <param name="data">The collected data which contains the number.</param>
        /// <param name="offset">The current location of the <see cref="IEncodingReader"/> within <paramref name="data"/>.</param>
        /// <param name="count">The number of encoded characters to grab.</param>
        /// <returns>A byte array containing every collected <see cref="char"/>.</returns>
        byte[] GetCharsToCount(byte[] data, ref int offset, int count);
    }
}
