namespace Nivaes.DataModel.Packer;

public interface ISerializer<T>
{
    int IdClassType { get; }

    void Serialize(scoped in T value, ref PackerWriter writer);
}
