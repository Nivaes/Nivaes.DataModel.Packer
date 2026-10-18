namespace Nivaes.DataModel.Packer;

public interface ISerializer
{
    int IdClassType { get; }
}

public interface ISerializer<T> : ISerializer
{
    void Serialize(scoped in T value, ref PackerWriter writer);
}
