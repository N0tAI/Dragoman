namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateDouble : IIntermediateValue<double>
    {
        public string Name { get; init; }
        public double Value { get; init; }
        internal IntermediateDouble(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
