
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
    /// It is not a real deserializer but an AVX2 enabled ultra fast JSON parser which according to its inventor Daniel Lemire is able to parse GB/s of JSON documents.
    /// See this academic paper on how the SIMD Parser works: https://arxiv.org/abs/1902.08318 (Press Download as PDF to view its contents). Fascinating!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/EgorBo/SimdJsonSharp based on https://github.com/lemire/simdjson", 
                    SerializerTypes.Json)]
    [IgnoreSerializeTimeAttribute("Utf8Json is used for serialize hence the serialize time is ignored.")]
    class SimdJsonSharpSerializer<T> : TestBase<BookShelf, ParsedJson>
    {
        static byte[] myTitle = Encoding.UTF8.GetBytes("Title");
        static byte[] myId = Encoding.UTF8.GetBytes("Id");

        ParsedJsonIterator myIterator = default;

        public SimdJsonSharpSerializer(Func<int, BookShelf> testData, Action<BookShelf> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(BookShelf obj, Stream stream)
        {
            // we generate data with UTF8Json since SimdJson is a parser and not a real serializer
            Utf8Json.JsonSerializer.Serialize(stream, obj); 
        }

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

            if (stream is MemoryStream mem)
            {
                byte[] buffer = mem.GetBuffer();
                fixed (byte* ptr = buffer)
                {
                    using (ParsedJson doc = SimdJson.ParseJson(ptr, (int)stream.Length))
                    {
                        // open iterator:
                        myIterator = new ParsedJsonIterator(doc);
                        try
                        {
                            while (myIterator.MoveForward() && myIterator.GetTokenType() == JsonTokenType.Comment)
                            {
                                if (myIterator.GetUtf16String() == "Books")
                                {
                                    if (myIterator.MoveForward() && myIterator.GetTokenType() == JsonTokenType.StartArray)
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
            ref ParsedJsonIterator refIt = ref myIterator;

            Span<sbyte> titleSpan = new Span<sbyte>(Unsafe.As<sbyte[]>(myTitle));
            Span<sbyte> idSpan = new Span<sbyte>(Unsafe.As<sbyte[]>(myId));

            while (refIt.MoveForward() && refIt.GetTokenType() == JsonTokenType.StartObject)
            {
                JsonTokenType curType = JsonTokenType.None;
                while (refIt.MoveForward() && (curType = refIt.GetTokenType()) != JsonTokenType.EndObject)
                {
                    if (refIt.IsString)
                    {
                        var currentString = new Span<sbyte>(refIt.GetUtf8String(), refIt.GetUtf8StringLength());

                        if (currentString.SequenceEqual(titleSpan))
                        {
                            if (refIt.MoveForward() && refIt.IsString)
                            {
                                bookTitle = refIt.GetUtf16String();
                                nTokens++;
                            }
                            else
                            {
                                throw new InvalidDataException($"Expected string as title but got {refIt.GetTokenType()}");
                            }
                        }

                        if (currentString.SequenceEqual(idSpan))
                        {
                            if (refIt.MoveForward() && refIt.IsInteger)
                            {
                                bookId = (int)refIt.GetInteger();
                                nTokens++;
                            }
                            else
                            {
                                throw new InvalidDataException($"Expected integers as book id but got {refIt.GetTokenType()}");
                            }
                        }
                    }
                }

                if (nTokens == 2)
                {
                    books.Add(new Book() { Id = bookId, Title = bookTitle });
                    nTokens = 0;
                }
            }
        }
    }

    static class SpanExtensions
    {
        public static bool AreEqual(this Span<sbyte> str1, Span<sbyte> str2)
        {
            return str1.SequenceEqual(str2);
        }
    }
}

#endif