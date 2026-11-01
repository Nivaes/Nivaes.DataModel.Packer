namespace Nivaes.DataModel.Packer;

public interface IDeserializer
{
    int IdObject { get; set; }
}

public interface IDeserializer<T> : IDeserializer
{
    static abstract int IdClassType { get; }

    T? Deserialize(ref PackerReader reader);
}
