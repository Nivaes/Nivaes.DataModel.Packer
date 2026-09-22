namespace Nivaes.DataModel.Packer;

public interface IPackable<T>
{
    void Serialize(ref PackerWriter writer);

    abstract static T? Deserialize(ref PackerReader reader);
}
