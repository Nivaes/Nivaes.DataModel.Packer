using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public ref struct PackerWriter<TBufferWriter>
    where TBufferWriter : IBufferWriter<byte>
{
}
