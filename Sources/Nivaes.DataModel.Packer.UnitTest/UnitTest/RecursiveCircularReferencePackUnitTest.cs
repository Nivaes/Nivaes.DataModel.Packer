using AutoFixture.Xunit3;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer1.UnitTest;

public class RecursiveCircularReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public RecursiveCircularReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public class ReferenceModel : IPackable<ReferenceModel>
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public ReferenceModel? Reference1 { get; set; }

        public ReferenceModel? Reference2 { get; set; }

        void IPackable<ReferenceModel>.Serialize(ref PackerWriter writer)
        {
            writer.Write(String1);
            writer.Write(String2);
            writer.Write(Reference1);
        }

        static ReferenceModel? IPackable<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = reader.Read<ReferenceModel>();

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2,
                Reference1 = reference
            };

            return value;
        }

        public void DeserializeCircular(ref PackerReader reader)
        {
            //Reference1 = reader.Read<ReferenceModel>();
        }
    }

    [Fact]
    public void SerializerVoidDataTest()
    {
        var model = new ReferenceModel()
        {
            String1 = null!,
            String2 = null!,
            Reference1 = new ReferenceModel
            {
                String1 = null!,
                String2 = null!,
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<ReferenceModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.Reference1.ShouldNotBeNull();
        copyModel.Reference1.Reference1.ShouldBe(copyModel);
    }

    [Fact]
    public void SerializerEmptyDataTest()
    {
        var model = new ReferenceModel()
        {
            String1 = string.Empty,
            String2 = string.Empty,
            Reference1 = new ReferenceModel
            {
                String1 = string.Empty,
                String2 = string.Empty,
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize(model);

        _output.WriteLine(Convert.ToHexString(cache));

        var copyModel = DataModelPacker.Deserialize<ReferenceModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.Reference1.ShouldNotBeNull();
        copyModel.Reference1.Reference1.ShouldBe(copyModel);
    }

    [Fact]
    public void SerializerTest()
    {
        var model = new ReferenceModel
        {
            String1 = "test1",
            String2 = "test2",
            Reference1 = new ReferenceModel
            {
                String1 = "test3",
                String2 = "test4",
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<ReferenceModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.Reference1.ShouldNotBeNull();
        copyModel.Reference1.Reference1.ShouldBe(copyModel);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest1(string test1, string test2, string test3, string test4)
    {
        var model = new ReferenceModel
        {
            String1 = test1,
            String2 = test2,
            Reference1 = new ReferenceModel
            {
                String1 = test3,
                String2 = test4,
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<ReferenceModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.Reference1.ShouldNotBeNull();
        copyModel.Reference1.Reference1.ShouldBe(copyModel);
    }
}
