#if NETCOREAPP3_0_OR_GREATER
#endif
using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ZeroFormatter;

namespace SerializerTests.TypesToSerialize
{
    [Serializable, MessagePackObject, ZeroFormattable, DataContract
#if NETCOREAPP3_0_OR_GREATER
#endif
    ]
    public partial class ReferenceBookShelf
    {
        [DataMember(Order = 0), ProtoMember(1), Key(0), Index(0)]
        public virtual Dictionary<DateTime, ReferenceBook> Books
        { get; set; } = new Dictionary<DateTime, ReferenceBook>();
    }

    [Serializable, MessagePackObject, ZeroFormattable, DataContract
#if NETCOREAPP3_0_OR_GREATER
#endif
    ]
    public partial class ReferenceBook
    {
        [DataMember(Order = 0), Key(0), Index(0)]
        public virtual ReferenceBookShelf Container { get; set; }  // Create object cycle if necessary

        [DataMember(Order = 1), Key(1), Index(1)]
        public virtual float Price { get; set; }

        // Protocol buffers no longer supports reference tracking only at object declaration level!
        [DataMember(Order = 2), Key(2), Index(2), ProtoMember(2)] 
        public virtual string Name { get; set; }
    }
}
