namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateNull : IIntermediateObject
    {
        public string Name { get; init; }
        internal IntermediateNull(string name)
        {
            Name = name;
        }
    }
}
