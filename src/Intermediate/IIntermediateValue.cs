using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Intermediate
{
    public interface IIntermediateValue<TValue> : IIntermediateObject
    {
        TValue Value { get; init; }
    }
}
