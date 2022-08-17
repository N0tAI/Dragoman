using Dragoman.Intermediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Utility
{
    public interface IIntermediateValueFactory
    {
        IIntermediateObject CreateObject(object firstValue, object secondValue);
    }
}
