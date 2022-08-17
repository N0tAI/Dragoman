using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Internal
{
    /// <summary>
    /// Represents an exception that occurs when decoding a <see cref="byte"/> sequence into CIL values.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// An <see cref="Exception"/> that occurs when parsing a CIL value from an encoding with a <paramref name="message"/>.
        /// </summary>
        /// <param name="encodingType">The text encoding the error was throw at.</param>
        /// <param name="message">A message describing the error that occured.</param>
        public ParseException(string encodingType, string message = "Recieved an invalid byte sequence.") : base($"Exception occured while parsing from {encodingType}: {message}")
        {

        }

        /// <summary>
        /// An <see cref="Exception"/> that occurs when parsing a CIL value with a <paramref name="message"/>.
        /// </summary>
        /// <param name="message">A message describing the error that occured.</param>
        public ParseException(string message) : base(message)
        {

        }

        /// <summary>
        /// An <see cref="Exception"/> that occurs when parsing a CIL value with a <paramref name="message"/> and an internal error.
        /// </summary>
        /// <param name="message">A message describing the error that occured.</param>
        /// <param name="innerException">The inner exception of the <see cref="ParseException"/></param>
        public ParseException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
