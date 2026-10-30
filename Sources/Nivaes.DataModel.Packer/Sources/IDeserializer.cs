namespace Nivaes.DataModel.Packer;

public struct ReferenceSetter
{
    public string Tag;
    public Action<object, object?> Setter;
    public int IdObject;
    public object? Reference;
}

public interface IDeserializer
{
    int IdObject { get; set; }

    ReferenceSetter[]? ReferenceSetters { get => null; }

    void DeserializeCircular(object value)
    {
        if (ReferenceSetters is null)
            return;

        foreach (var setters in ReferenceSetters)
        {
            setters.Setter(value!, setters.Reference);
        }
    }
}

public interface IDeserializer<T> : IDeserializer
{
    static abstract int IdClassType { get; }

    T? Deserialize(ref PackerReader reader);
}
