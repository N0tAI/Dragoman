namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateInteger : IIntermediateValue<ulong>
    {
        public string Name { get; init; }
        public ulong Value { get; init; }

        internal IntermediateInteger(string name, ulong value)
        {
            Name = name;
            Value = value;
        }
    }
}
