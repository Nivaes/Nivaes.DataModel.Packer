using AutoFixture.Xunit3;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer.UnitTest;

public class CircularReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public CircularReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public class RootModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public required ReferenceModel Reference1 { get; set; }
        public required ReferenceModel Reference2 { get; set; }
    }

    public class RootModelSerializer : ISerializer<RootModel>
    {
        int ISerializer.IdClassType => 10;

        void ISerializer<RootModel>.Serialize(scoped in RootModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(model.Reference1);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(model.Reference2);
        }
    }

    public class RootModelDeserializer : IDeserializer<RootModel>
    {
        static int IDeserializer<RootModel>.IdClassType => 10;

        int IDeserializer.IdObject
        { 
            get; 
            set; 
        }

        private readonly ReferenceSetter[] _referenceSetters = new ReferenceSetter[]
        {
            new ReferenceSetter { Tag = "Root.Reference1",  Setter = ReferenceReference1!, Reference = null },
            new ReferenceSetter { Tag = "Root.Reference2", Setter = ReferenceReference2!, Reference = null },
        };

        ReferenceSetter[] IDeserializer.ReferenceSetters => _referenceSetters;

        RootModel? IDeserializer<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference1 = reader.Read<RootModel, ReferenceModel, ReferenceModelDeserializer>(((IDeserializer)this).IdObject);
            var reference2 = reader.Read<RootModel, ReferenceModel, ReferenceModelDeserializer>(((IDeserializer)this).IdObject);

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference1 = reference1!,
                Reference2 = reference2!

            };
            return value;
        }

        public static void ReferenceReference1(object value, object reference)
        {
            ((RootModel)value).Reference1 = (ReferenceModel)reference;
        }

        public static void ReferenceReference2(object value, object reference)
        {
            ((RootModel)value).Reference2 = (ReferenceModel)reference;
        }

    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public RootModel? RootReference { get; set; }
    }

    public class ReferenceModelSerializer : ISerializer<ReferenceModel>
    {
        int ISerializer.IdClassType => 20;

        void ISerializer<ReferenceModel>.Serialize(scoped in ReferenceModel model, ref PackerWriter writer)
        {
            writer.Write(model.String1);
            writer.Write(model.String2);
            writer.Write<RootModel, RootModelSerializer>(model.RootReference);
        }
    }

    public class ReferenceModelDeserializer : IDeserializer<ReferenceModel>
    {
        static int IDeserializer<ReferenceModel>.IdClassType => 20;

        int IDeserializer.IdObject { get; set; }

        private readonly ReferenceSetter[] _referenceSetters = new ReferenceSetter[]
        {
            new ReferenceSetter { Tag = "Reference.Root", Setter = ReferenceReference1!, Reference = null },
        };

        ReferenceSetter[] IDeserializer.ReferenceSetters => _referenceSetters;

        ReferenceModel? IDeserializer<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var rootReference = reader.Read<ReferenceModel, RootModel, RootModelDeserializer>(((IDeserializer)this).IdObject);

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
            Reference1 = new ReferenceModel
            {
                String1 = null!,
                String2 = null!,
            },
            Reference2 = new ReferenceModel
            {
                String1 = null!,
                String2 = null!,
            }
        };
        model.Reference1.RootReference = model;
        model.Reference2.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.RootReference.ShouldNotBeNull();
        copyModel.Reference1.RootReference.ShouldBe(copyModel);
    }

    [Fact]
    public void SerializerEmptyDataTest()
    {
        var model = new RootModel()
        {
            String1 = string.Empty,
            String2 = string.Empty,
            Reference1 = new ReferenceModel
            {
                String1 = string.Empty,
                String2 = string.Empty,
            },
            Reference2 = new ReferenceModel
            {
                String1 = string.Empty,
                String2 = string.Empty,
            },
        };
        model.Reference1.RootReference = model;
        model.Reference2.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.RootReference.ShouldNotBeNull();
        copyModel.Reference1.RootReference.ShouldBe(copyModel);

        copyModel.Reference2.RootReference.ShouldNotBeNull();
        copyModel.Reference2.RootReference.ShouldBe(copyModel);

    }

    [Fact]
    public void SerializerTest()
    {
        var model = new RootModel
        {
            String1 = "test1",
            String2 = "test2",
            Reference1 = new ReferenceModel
            {
                String1 = "test3",
                String2 = "test4",
            },
            Reference2 = new ReferenceModel
            {
                String1 = "test5",
                String2 = "test6",
            }
        };
        model.Reference1.RootReference = model;
        model.Reference2.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.RootReference.ShouldNotBeNull();
        copyModel.Reference1.RootReference.ShouldBe(copyModel);

        copyModel.Reference2.RootReference.ShouldNotBeNull();
        copyModel.Reference2.RootReference.ShouldBe(copyModel);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest1(string test1, string test2, string test3, string test4, string test5, string test6)
    {
        var model = new RootModel
        {
            String1 = test1,
            String2 = test2,
            Reference1 = new ReferenceModel
            {
                String1 = test3,
                String2 = test4,
            },
            Reference2 = new ReferenceModel
            {
                String1 = test5,
                String2 = test6,
            }
        };
        model.Reference1.RootReference = model;
        model.Reference2.RootReference = model;

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference1.ShouldNotBeNull();
        copyModel.Reference1.String1.ShouldBe(model.Reference1.String1);
        copyModel.Reference1.String2.ShouldBe(model.Reference1.String2);

        copyModel.Reference1.RootReference.ShouldNotBeNull();
        copyModel.Reference1.RootReference.ShouldBe(copyModel);

        copyModel.Reference2.RootReference.ShouldNotBeNull();
        copyModel.Reference2.RootReference.ShouldBe(copyModel);

    }
}
