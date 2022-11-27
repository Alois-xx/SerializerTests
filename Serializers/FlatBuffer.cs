using Azos.Data.Modeling.DataTypes;
using Google.FlatBuffers;
using SerializerTests.TypesToSerialize;
using ServiceStack.Text;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://google.github.io/flatbuffers/",
                    SerializerTypes.Binary | SerializerTypes.SupportsVersioning)]
    class FlatBuffer<TSerialize, TDeserialize> : TestBase<TSerialize, TDeserialize, ByteBuffer>
    {
        public FlatBuffer(Func<int, TSerialize> testData, Action<TDeserialize, int,int> touchAndVerify):base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            if(obj is BookShelf bookShelf)
            {
                ConvertToFlatBuffer((BookShelf)(object)obj, out BookShelfFlat bookShelfFlat);

                stream.Write(bookShelfFlat.ByteBuffer.ToFullArray(), bookShelfFlat.ByteBuffer.Position, bookShelfFlat.ByteBuffer.Length - bookShelfFlat.ByteBuffer.Position);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        void ConvertToFlatBuffer(BookShelf bookshelf, out BookShelfFlat outBooks)
        {
            var builder = new FlatBufferBuilder(1024);

            int nToCreate = bookshelf.Books.Count;
            int bookDataSize = (bookshelf.Books[0]?.BookData?.Length).GetValueOrDefault();
            Offset<BookFlat>[] books = new Offset<BookFlat>[nToCreate];

            for (int i = 1; i <= nToCreate; i++)
            {
                Book book = bookshelf.Books[i-1];
                var title = builder.CreateString(book.Title);

                builder.StartVector(1, bookDataSize, 0);
                byte[] bytes = book.BookData;
                if (bytes?.Length > 0)
                {
                    builder.Put(bytes);
                }
                VectorOffset bookbyteArrayOffset = builder.EndVector();
                var bookOffset = BookFlat.CreateBookFlat(builder, title, i, bookbyteArrayOffset);
                books[i - 1] = bookOffset;
            }

            var secretOffset = builder.CreateString("private member value");
            VectorOffset booksVector = builder.CreateVectorOfTables<BookFlat>(books);
            var lret = BookShelfFlat.CreateBookShelfFlat(builder, booksVector, secretOffset);
            builder.Finish(lret.Value);

            outBooks = BookShelfFlat.GetRootAsBookShelfFlat(builder.DataBuffer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TDeserialize Deserialize(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            // Since flatbuffers do not support memory streams we have to copy here
            stream.CopyTo(mem);
            byte[] data = mem.ToArray();
            var bookShelf = BookShelfFlat.GetRootAsBookShelfFlat(new ByteBuffer(data));
            int nBooks = bookShelf.BooksLength;

            // touch each property once so it will allocate the string objects once to pay the acutal object allocation

            int tmpid;
            string tmpTitle;
            byte[] tmpArray;
            string tmpSecet = bookShelf.Secret;
            for (int i = 0; i < nBooks; i++)
            {
                BookFlat? flatBook = bookShelf.Books(i);
                tmpid = flatBook.Value.Id;
                tmpTitle = flatBook.Value.Title; ;
                tmpArray = flatBook.Value.GetBookDataArray();
            }

            return (TDeserialize)(object) bookShelf;
        }
    }
}
