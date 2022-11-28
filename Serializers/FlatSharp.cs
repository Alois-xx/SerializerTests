using Azos.Scripting;
using FlatSharp;
using Google.FlatBuffers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/jamescourtney/FlatSharp/",
                    SerializerTypes.Binary | SerializerTypes.SupportsVersioning)]
    class FlatSharp<TSerialize> : TestBase<TSerialize, BookShelfFlatSharp, ByteBuffer>
    {
        private BookShelf knownBookshelf;
        private BookShelfFlatSharp fsShelf;

        public FlatSharp(Func<int, TSerialize> testData, Action<BookShelfFlatSharp, int, int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            MemoryStream ms = (MemoryStream)stream;

            this.TranslateObject((BookShelf)(object)obj, ms);

            // Write to the underlying buffer.
            int bytesWritten = BookShelfFlatSharp.Serializer.Write(ms.GetBuffer(), this.fsShelf);

            // Update the buffer to indicate how many bytes were written.
            ms.Position = bytesWritten;
            ms.SetLength(bytesWritten);
        }

        /// <summary>
        /// Since FlatSharp doesn't use the common Bookshelf object, add a translation step here.
        /// Note: since this translation process is expensive (and not part of the benchmark),
        /// only do this once per unique BookShelf.
        /// </summary>
        private void TranslateObject(BookShelf obj, MemoryStream stream)
        {
            if (this.knownBookshelf == obj)
            {
                return;
            }

            this.knownBookshelf = obj;
            var books = obj.Books;

            BookShelfFlatSharp bookshelf = new BookShelfFlatSharp();
            List<BookFlatSharp> fsBooks = new List<BookFlatSharp>(books.Count);

            foreach (var book in books)
            {
                fsBooks.Add(new BookFlatSharp { BookData = book.BookData, Id = book.Id, Title = book.Title });
            }

            bookshelf.Books = fsBooks;
            this.fsShelf = bookshelf;

            // Expand size if necessary.
            stream.SetLength(BookShelfFlatSharp.Serializer.GetMaxSize(bookshelf));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override BookShelfFlatSharp Deserialize(Stream stream)
        {
            byte[] buffer = ((MemoryStream)stream).GetBuffer();
            return BookShelfFlatSharp.Serializer.Parse(buffer);
        }
    }
}
