using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Intermediate
{
    public interface IIntermediateContainer : IIntermediateObject, IEnumerable<IIntermediateObject>
    {
        IIntermediateObject[] SubObjects { get; }
        IIntermediateContainer AddObject(IIntermediateObject obj);
    }
}
