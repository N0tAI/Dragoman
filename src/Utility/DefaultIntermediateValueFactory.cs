using Dragoman.Intermediate;
using Dragoman.Intermediate.Declarations;
using System;

namespace Dragoman.Utility
{
    public sealed class DefaultIntermediateValueFactory : IIntermediateValueFactory
    {
        public IIntermediateObject CreateObject(object firstValue, object secondValue)
        {
            if (firstValue is string name && secondValue != null)
                return GetObjectByType(secondValue, name);
            else if (secondValue != null)
                throw new Exception("Invalid JSON format, no string in identified name - value set");
            else
                return GetObjectByType(firstValue, "");

        }

        private IIntermediateObject GetObjectByType(object value, string name)
        {
            if (value is double @double)
                return new IntermediateDouble(name, @double);
            else if (value is float @float)
                return new IntermediateFloat(name, @float);
            else if (value is ulong @int)
                return new IntermediateInteger(name, @int);
            else if (value is string @string)
                return new IntermediateString(name, @string);
            else if (value is bool @bool)
                return new IntermediateBoolean(name, @bool);
            else if(value is object && name != "")
                return new IntermediateNull(name);
            throw new Exception("Invalid JSON value");

        }
    }
}
