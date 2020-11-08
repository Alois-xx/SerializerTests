using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.TypesToSerialize
{
    [Serializable, DataContract, ProtoContract, MessagePackObject]
    public class BookShelf
    {
        [DataMember, ProtoMember(1), Key(0)]
        public List<Book> Books
        {
            get;
            set;
        }

        [DataMember, ProtoMember(2), Key(1)]
        private string Secret;


        public BookShelf(string secret)
        {
            Secret = secret;
        }

        public BookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        {
        }
    }

    [Serializable, DataContract, ProtoContract, MessagePackObject]
    public class Book
    {
        [DataMember, ProtoMember(1), Key(0)]
        public string Title;

        [DataMember, ProtoMember(2), Key(1)]
        public int Id;

        [DataMember, ProtoMember(3), Key(2)]
        public byte[] BookData;
    }


}
