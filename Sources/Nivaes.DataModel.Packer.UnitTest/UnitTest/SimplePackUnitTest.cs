using System.Text;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer1.UnitTest;


public class SimplePackUnitTest
{
    public class SimpleModel : IPackable<SimpleModel>
    {
        public string String1 { get; set; }
        public string String2 { get; set; }


        void IPackable<SimpleModel>.Serialize(BinaryWriter writer, scoped in SimpleModel? value)
        {
            writer.Write(String1);
            writer.Write(String2);
        }

        static SimpleModel? IPackable<SimpleModel>.Deserialize(BinaryReader reader)
        {
            var value = new SimpleModel
            {
                String1 = reader.ReadString(),
                String2 = reader.ReadString()
            };

            return value;
        }


    }

    [Fact]
    public void SerializerTest()
    {
        var model = new SimpleModel
        {
            String1 = "test1",
            String2 = "test2",
        };

        var cache = DataModelPacker.Serializer(model);

        var copyModel = DataModelPacker.Deserializer<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }
}
