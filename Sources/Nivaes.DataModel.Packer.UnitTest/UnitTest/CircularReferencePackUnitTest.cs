using AutoFixture.Xunit3;
using Nivaes.DataModel.Packer;
using static Nivaes.DataModel.Packer.UnitTest.AllTypesPackUnitTest;

namespace Nivaes.DataModel.Packer.UnitTest;


public class CircularReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public CircularReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;

        DataModelPacker.IdClassToType.TryAdd(10, typeof(RootModelSerializer));
        DataModelPacker.ClassToSerializerType.TryAdd(typeof(RootModel), typeof(RootModelSerializer));

        DataModelPacker.IdClassToType.TryAdd(20, typeof(ReferenceModelSerializer));
        DataModelPacker.ClassToSerializerType.TryAdd(typeof(ReferenceModel), typeof(ReferenceModelSerializer));
    }

    public class RootModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public required ReferenceModel Reference { get; set; }
    }

    public class RootModelSerializer : IPacker<RootModel>
    {
        int IPacker<RootModel>.IdClassType => 10;

        UnpackerReferences[] IPacker<RootModel>.UpackerReferences => new UnpackerReferences[2];

        ReferenceSetter[] IPacker<RootModel>.ReferenceSetters => null!;

        void IPacker<RootModel>.Serialize(scoped in RootModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(model.Reference);
        }

        RootModel? IPacker<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reader.Read<ReferenceModel, ReferenceModelSerializer>()!
            };
            return value;
        }

        public void DeserializeCircular(RootModel value, ref PackerReader reader)
        {
        }
    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public RootModel? RootReference { get; set; }
    }

    public class ReferenceModelSerializer : IPacker<ReferenceModel>
    {
        int IPacker<ReferenceModel>.IdClassType => 20;

        UnpackerReferences[] IPacker<ReferenceModel>.UpackerReferences => new UnpackerReferences[1];

        ReferenceSetter[] IPacker<ReferenceModel>.ReferenceSetters => null!;

        void IPacker<ReferenceModel>.Serialize(scoped in ReferenceModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<RootModel, RootModelSerializer>(model.RootReference);
        }

        ReferenceModel? IPacker<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2,
            };

            return value;
        }

        public void DeserializeCircular(ReferenceModel value, ref PackerReader reader)
        {
            //RootReference = reader.Read<RootModel>();
        }
    }

    [Fact]
    public void SerializerVoidDataTest()
    {
        var model = new RootModel()
        {
            String1 = null!,
            String2 = null!,
            Reference = new ReferenceModel
            {
                String1 = null!,
                String2 = null!,
            }
        };
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
    }

    [Fact]
    public void SerializerEmptyDataTest()
    {
        var model = new RootModel()
        {
            String1 = string.Empty,
            String2 = string.Empty,
            Reference = new ReferenceModel
            {
                String1 = string.Empty,
                String2 = string.Empty,
            }
        };
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
    }

    [Fact]
    public void SerializerTest()
    {
        var model = new RootModel
        {
            String1 = "test1",
            String2 = "test2",
            Reference = new ReferenceModel
            {
                String1 = "test3",
                String2 = "test4",
            }
        };
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest1(string test1, string test2, string test3, string test4)
    {
        var model = new RootModel
        {
            String1 = test1,
            String2 = test2,
            Reference = new ReferenceModel
            {
                String1 = test3,
                String2 = test4,
            }
        };
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
    }
}
