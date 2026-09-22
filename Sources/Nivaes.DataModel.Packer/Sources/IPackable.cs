namespace Nivaes.DataModel.Packer;

public interface IPackable<T>
{
    void Serialize(BinaryWriter writer, scoped in T? value);

    abstract static T? Deserialize(BinaryReader reader);
}
