#if ( NETCOREAPP3_1 || NETCOREAPP3_0 )
using SerializerTests.TypesToSerialize;
using SimdJsonSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// This one calls into the Native SimdJson library by Daniel Lemire.
    /// It is not a real deserializer but an AVX2 enabled ultra fast JSON parser which according to its inventor Daniel Lemire is able to parse GB/s of JSON documents.
    /// See this academic paper on how the SIMD Parser works: https://arxiv.org/abs/1902.08318 (Press Download as PDF to view its contents). Fascinating!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/EgorBo/SimdJsonSharp based on https://github.com/lemire/simdjson", 
                    SerializerTypes.Json)]
    [IgnoreSerializeTimeAttribute("Utf8Json is used for serialize hence the serialize time is ignored.")]
    class SimdJsonSharpSerializerN<T> : TestBase<BookShelf, ParsedJsonN>
    {
        static byte[] myTitle = Encoding.UTF8.GetBytes("Title");
        static byte[] myId = Encoding.UTF8.GetBytes("Id");

        ParsedJsonIteratorN myIterator = default;

        public SimdJsonSharpSerializerN(Func<int, BookShelf> testData, Action<BookShelf> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(BookShelf obj, Stream stream)
        {
            // we generate data with UTF8Json since SimdJson is a parser and not a real serializer
            Utf8Json.JsonSerializer.Serialize(stream, obj); 
        }

        static readonly byte[] BooksProperty = Encoding.UTF8.GetBytes("Books");

        [MethodImpl(MethodImplOptions.NoInlining)]
        unsafe protected override BookShelf Deserialize(Stream stream)
        {
            // A small Json like {"Books":[{"Title":"Book 1","Id":1}]} 
            // will be translated to the following JsonTokenTypes by the iterator:
            //   String =     "Books"
            //   StartArray   [
            //   StartObject  { 
            //   String       "Title"
            //   String       "Book 1"
            //   String       "Id"
            //   Number        1
            //   EndObject     }
            //   EndArray      ]
            //   EndObject     }

            BookShelf lret = new BookShelf() { Books = new List<Book>() };
            ReadOnlySpan<byte> booksPropertySpan = BooksProperty;

            if (stream is MemoryStream mem)
            {
                byte[] buffer = mem.GetBuffer();
                fixed (byte* ptr = buffer)
                {
                    using (ParsedJsonN doc = SimdJsonN.ParseJson(ptr, (int)stream.Length))
                    {
                        // open iterator:
                        myIterator = new ParsedJsonIteratorN(doc);
                        try
                        {
                            while (myIterator.MoveForward() && myIterator.IsString)
                            {
                                if ( new Span<byte>(myIterator.GetUtf8String(), (int) myIterator.GetUtf8StringLength()).SequenceEqual(booksPropertySpan))
                                {
                                    if (myIterator.MoveForward() && myIterator.IsObjectOrArray && myIterator.Down())
                                    {
                                        ReadBooks(lret.Books);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            myIterator.Dispose();
                        }
                    }
                }
            }

            return lret;
        }

        unsafe private void ReadBooks(List<Book> books)
        {
            int nTokens = 0;
            int bookId = 0;
            string bookTitle = null;
            ref ParsedJsonIteratorN refIt = ref myIterator;

            Span<sbyte> titleSpan = new Span<sbyte>(Unsafe.As<sbyte[]>(myTitle));
            Span<sbyte> idSpan = new Span<sbyte>(Unsafe.As<sbyte[]>(myId));

            while (refIt.IsObject && refIt.Down())
            {
                if (refIt.IsString)
                {
                    var propName = new Span<sbyte>(refIt.GetUtf8String(), (int)refIt.GetUtf8StringLength());

                    if (propName.SequenceEqual(titleSpan))
                    {
                        if (refIt.Next() && refIt.IsString)
                        {
                            bookTitle = refIt.GetUtf16String();
                            nTokens++;
                        }
                        else
                        {
                            throw new InvalidDataException($"Expected string as title but got {refIt.CurrentType}");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException($"Expected string as title but got {propName.ToString()}");
                    }

                    if( refIt.MoveForward() && refIt.IsString )
                    {
                        propName = new Span<sbyte>(refIt.GetUtf8String(), (int)refIt.GetUtf8StringLength());

                        if (propName.SequenceEqual(idSpan) && refIt.MoveForward() && refIt.IsInteger)
                        {
                            bookId = (int)refIt.GetInteger();
                            nTokens++;
                        }
                        else
                        {
                            throw new InvalidDataException($"Expected integers as book id but got {refIt.CurrentType}");
                        }
                    }                 
                }

                if (nTokens == 2)
                {
                    books.Add(new Book() { Id = bookId, Title = bookTitle });
                    nTokens = 0;
                }

                while(refIt.Next()) // skip rest
                {
                }
                refIt.Up();
                if( !refIt.Next() )
                {
                    break;
                }
            }
        }
    }
}

#endif