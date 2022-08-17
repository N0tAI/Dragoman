using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Intermediate
{
    public interface IIntermediateWriter
    {
        IIntermediateWriter AddArray(string name);
        IIntermediateWriter AddStruct(string name);
        IIntermediateWriter AddIntermediateObject(object firstValue, object secondValue = default);
        IIntermediateWriter CloseCurrentContainer();
    }
}
