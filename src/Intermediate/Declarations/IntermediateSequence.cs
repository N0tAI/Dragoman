using System;
using System.Collections;
using System.Collections.Generic;

namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateSequence : IIntermediateContainer
    {
        private List<IIntermediateObject> _subObjects;
        public IIntermediateObject[] SubObjects { get => _subObjects.ToArray(); }

        public string Name { get; init; }

        internal IntermediateSequence(string name)
        {
            _subObjects = new List<IIntermediateObject>();
            Name = name;
        }

        public IIntermediateContainer AddObject(IIntermediateObject obj)
        {
            if (obj.Name != default(string) || obj.Name != "")
                throw new ArgumentException("Parsed object has a Name/Value pair but is in a sequence.");
            _subObjects.Add(obj);
            return this;
        }

        public IEnumerator<IIntermediateObject> GetEnumerator()
            => _subObjects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _subObjects.GetEnumerator();
    }
}
