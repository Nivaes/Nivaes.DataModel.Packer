namespace Nivaes.DataModel.Packer;

public interface ISerializer<T>
{
    void Serialize(scoped in T value, ref PackerWriter writer);
}
