using Dragoman.Intermediate.Declarations;
using Dragoman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoman.Intermediate
{
    internal class IntermediateWriter : IIntermediateWriter
    {
        private readonly IIntermediateValueFactory _valueFactory;
        private readonly Stack<IIntermediateContainer> _objectStack;
        internal IntermediateWriter()
        {
            _objectStack = new Stack<IIntermediateContainer>();
            _valueFactory = new DefaultIntermediateValueFactory();
        }

        public IntermediateWriter AddArray(string name)
        {
            _objectStack.Push(new IntermediateSequence(name));
            return this;
        }

        public IntermediateWriter AddStruct(string name)
        {
            _objectStack.Push(new IntermediateStruct(name));
            return this;
        }
        public IntermediateWriter CloseCurrentContainer()
        {
            var container = _objectStack.Pop();
            return AddIntermediateObject(container);
        }
        public IntermediateWriter AddIntermediateObject(object firstValue, object secondValue = default)
        {
            var obj = _valueFactory.CreateObject(firstValue, secondValue);
            _objectStack.Peek().AddObject(obj);
            return this;
        }

        IIntermediateWriter IIntermediateWriter.AddArray(string name)
            => AddArray(name);

        IIntermediateWriter IIntermediateWriter.AddStruct(string name)
            => AddStruct(name);

        IIntermediateWriter IIntermediateWriter.AddIntermediateObject(object firstValue, object secondValue = default)
            => AddIntermediateObject(firstValue, secondValue);

        IIntermediateWriter IIntermediateWriter.CloseCurrentContainer()
            => CloseCurrentContainer();
    }
}
