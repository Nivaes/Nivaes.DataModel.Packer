using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.Models.Test.Sources
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
}
