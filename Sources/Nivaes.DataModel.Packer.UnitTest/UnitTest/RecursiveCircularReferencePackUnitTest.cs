using AutoFixture.Xunit3;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer.UnitTest;

public class RecursiveCircularReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public RecursiveCircularReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;

        DataModelPacker.IdClassToType.TryAdd(30, typeof(ReferenceModelSerializer));
        DataModelPacker.ClassToSerializerType.TryAdd(typeof(ReferenceModel), typeof(ReferenceModelSerializer));
    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public required ReferenceModel Reference1 { get; set; }

        public ReferenceModel? Reference2 { get; set; }
    }

    public class ReferenceModelSerializer : IPacker<ReferenceModel>
    {
        public int IdClassType => 30;

        void IPacker<ReferenceModel>.Serialize(scoped in ReferenceModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(model.Reference1);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(model.Reference2);
        }

        ReferenceModel? IPacker<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = reader.Read<ReferenceModel, ReferenceModelSerializer>();

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2,
                Reference1 = reference!
            };

            return value;
        }

        public void DeserializeCircular(ReferenceModel value, ref PackerReader reader)
        {
            value.Reference2 = reader.Read<ReferenceModel, ReferenceModelSerializer>();
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
                Reference1 = null!
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize<ReferenceModel, ReferenceModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<ReferenceModel, ReferenceModelSerializer>(cache);

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
                Reference1 = null!
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize<ReferenceModel, ReferenceModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<ReferenceModel, ReferenceModelSerializer>(cache);

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
                Reference1 = null!
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize<ReferenceModel, ReferenceModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<ReferenceModel, ReferenceModelSerializer>(cache);

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
                Reference1 = null!
            }
        };
        model.Reference1.Reference1 = model;

        var cache = DataModelPacker.Serialize<ReferenceModel, ReferenceModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<ReferenceModel, ReferenceModelSerializer>(cache);

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
