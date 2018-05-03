using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SerializerTests.TypesToSerialize
{
    [MessagePackObject]
    public class ReferenceBookShelf
    {
        [DataMember, ProtoMember(1), Key(0)]
        public Dictionary<DateTime, ReferenceBook> Books
        { get; set; } = new Dictionary<DateTime, ReferenceBook>();
    }

    [MessagePackObject]
    public class ReferenceBook
    {
        [DataMember, ProtoMember(1), Key(0)]
        public ReferenceBookShelf Container { get; set; }  // Create object cycle if necessary

        [DataMember, ProtoMember(2), Key(1)]
        public float Price { get; set; }

        [DataMember, ProtoMember(3), Key(2)]
        public string Name { get; set; }
    }
}
