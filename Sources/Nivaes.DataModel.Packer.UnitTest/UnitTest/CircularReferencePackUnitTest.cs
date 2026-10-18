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

        private readonly ReferenceSetter[] _referenceSetters = new ReferenceSetter[]
        {
            new ReferenceSetter { IdReference = 1, Setter = ReferenceReference1!, Reference = null },
        };

        ReferenceSetter[] IPacker<RootModel>.ReferenceSetters => _referenceSetters;

        void IPacker<RootModel>.Serialize(scoped in RootModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(1, model.Reference);
        }

        RootModel? IPacker<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = reader.Read<RootModel, ReferenceModel, RootModelSerializer>(1, ReferenceReference1);

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reference!
            };
            return value;
        }

        public static void ReferenceReference1(object value, object reference)
        {
            ((RootModel)value).Reference = (ReferenceModel)reference;
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

        private readonly ReferenceSetter[] _referenceSetters = new ReferenceSetter[]
        {
            new ReferenceSetter { IdReference = 0, Setter = ReferenceReference1!, Reference = null },
        };

        ReferenceSetter[] IPacker<ReferenceModel>.ReferenceSetters => _referenceSetters;

        void IPacker<ReferenceModel>.Serialize(scoped in ReferenceModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<RootModel, RootModelSerializer>(1, model.RootReference);
        }

        ReferenceModel? IPacker<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var rootReference = reader.Read<ReferenceModel, RootModel, ReferenceModelSerializer>(1, ReferenceReference1);

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2,
                RootReference = rootReference
            };

            return value;
        }

        public static void ReferenceReference1(object value, object reference)
        {
            ((ReferenceModel)value).RootReference = (RootModel)reference;
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
