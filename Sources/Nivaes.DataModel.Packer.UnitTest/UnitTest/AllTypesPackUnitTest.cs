using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer1.UnitTest;

public class AllTypesPackUnitTest
{
    public class AllTypesModel : IPackable<AllTypesModel>
    {
        public required short Short1 { get; set; }
        public required short Short2 { get; set; }
        public required int Int1 { get; set; }
        public required int Int2 { get; set; }
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        void IPackable<AllTypesModel>.Serialize(ref PackerWriter writer)
        {
            writer.Write(Short1);
            writer.Write(Short2);
            writer.Write(Int1);
            writer.Write(Int2);
            writer.Write(String1);
            writer.Write(String2);
        }

        static AllTypesModel? IPackable<AllTypesModel>.Deserialize(ref PackerReader reader)
        {
            var value = new AllTypesModel
            {
                Short1 = reader.ReadInt16(),
                Short2 = reader.ReadInt16(),
                Int1 = reader.ReadInt32(),
                Int2 = reader.ReadInt32(),
                String1 = reader.ReadString(),
                String2 = reader.ReadString()
            };

            return value;
        }
    }

    [Fact]
    public void SerializerTest()
    {
        var model = new AllTypesModel
        {
            Short1 = 3844,
            Short2 = -3324,
            Int1 = 233434323,
            Int2 = -233434323,
            String1 = "test2",
            String2 = "aklsñdjfñlaskdjfalskd aslkfj a elñkjafkldja ññladsjkf ladjlekjelkjad faldsj lkadje",
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<AllTypesModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.Short1.ShouldBe(model.Short1);
        copyModel.Short2.ShouldBe(model.Short2);

        copyModel.Int1.ShouldBe(model.Int1);
        copyModel.Int1.ShouldBe(model.Int1);

        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }
}
