using FlatBuffers;
using SerializerTests.Serializers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SerializerTests
{
    /* 
     * Howto add your own serializer:
     * 
        1. Look at Serializers directory for examples.
      
            You need to set the CreateNTestData delegate so the tester can serialize and deserialize it. 
            For the default settings you need only to override Serialize and Deserialize and call your formatter. The serializer type argument
            is used to print out the assembly version of your serializer. You can use any type of the declaring assembly.

            public class BinaryFormatter<T> : TestBase<T, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> where T : class
            {
                public BinaryFormatter(Func<int,T> testData, Action<T> toucher):base(testData, toucher)
                {
                    FormatterFactory = () => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                }

                protected override void Serialize(T obj, Stream stream)
                {
                    Formatter.Serialize(stream, obj);
                }

                protected override T Deserialize(Stream stream)
                {
                    return (T)Formatter.Deserialize(stream);
                }
           }

        2. Add your serializer to the list of serializers to be tested to the list of serializers in Deserialize, Serialize and FirstCall

        3. Recompile and run first the serialization test to create the test data on disk for the deserialization run.

        4. Publish the results on your own blog.
    }
    */


    class Program
    {
        static string Help = "SerializerTests is a serializer performance testing framework to evaluate and compare different serializers for .NET by Alois Kraus" + Environment.NewLine +
                             "SerializerTests [-Runs dd] -test [serialize, deserialize, combined, firstCall]" + Environment.NewLine +
                             " -Runs      Default is 5. The result is averaged where the first run is excluded from the average" + Environment.NewLine  +
                             " -test sc   sc can be serialize, deserialize, combined or firstcall to test a scenario for many different serializers" + Environment.NewLine +
                             " -notouch   Do not touch the deserialized objects to test lazy deserialization" + Environment.NewLine + 
                             "            To execute deserialize you must first have called the serialize to generate serialized test data on disk to be read during deserialize" + Environment.NewLine;
        private Queue<string> Args;

        List<ISerializeDeserializeTester> SerializersToTest;
        List<ISerializeDeserializeTester> StartupSerializersToTest;

        int Runs = 5;
        bool IsNGenWarn = true;
        bool IsTouch = true;


        public Program(string[] args)
        {
            Args = new Queue<string>(args);
            SerializersToTest = new List<ISerializeDeserializeTester>
            {
                new FlatBuffer<BookShelfFlat>(DataFlat, TouchFlat),
                new Bois<BookShelf>(Data, TouchBookShelf),
                new GroBuf<BookShelf>(Data, TouchBookShelf),
                new Jil<BookShelf>(Data, TouchBookShelf),
                new MessagePackSharp<BookShelf>(Data, TouchBookShelf),
                new Wire<BookShelf>(Data, TouchBookShelf),
                new Protobuf_net<BookShelf>(Data, TouchBookShelf),
                new SlimSerializer<BookShelf>(Data, TouchBookShelf),
                new ZeroFormatter<ZeroFormatterBookShelf>(DataZeroFormatter, TouchZeroFormatterShelf),
                new ServiceStack<BookShelf>(Data, TouchBookShelf),
                new FastJson<BookShelf>(Data, TouchBookShelf),
                new DataContractIndented<BookShelf>(Data, TouchBookShelf),
                new DataContractBinaryXml<BookShelf>(Data, TouchBookShelf),
                new DataContract<BookShelf>(Data, TouchBookShelf),
                new XmlSerializer<BookShelf>(Data, TouchBookShelf),
                new JsonNet<BookShelf>(Data, TouchBookShelf),
                new MsgPack_Cli<BookShelf>(Data, TouchBookShelf),
                new BinaryFormatter<BookShelf>(Data, TouchBookShelf),
            };

            StartupSerializersToTest = new List<ISerializeDeserializeTester>
            {
                new ServiceStack<BookShelf>(Data, null),
                new ServiceStack<BookShelf1>(Data1, null),
                new ServiceStack<BookShelf2>(Data2, null),
                new ServiceStack<LargeBookShelf>(DataLarge, null),

                new Bois<BookShelf>(Data, null),
                new Bois<BookShelf1>(Data1, null),
                new Bois<BookShelf2>(Data2, null),
                new Bois<LargeBookShelf>(DataLarge, null),

                new GroBuf<BookShelf>(Data, null),
                new GroBuf<BookShelf1>(Data1, null),
                new GroBuf<BookShelf2>(Data2, null),
                new GroBuf<LargeBookShelf>(DataLarge, null),

                new ZeroFormatter<ZeroFormatterBookShelf>(DataZeroFormatter, null),
                new ZeroFormatter<ZeroFormatterBookShelf1>(DataZeroFormatter1, null),
                new ZeroFormatter<ZeroFormatterBookShelf2>(DataZeroFormatter2, null),
                new ZeroFormatter<ZeroFormatterLargeBookShelf>(DataZeroFormatterLarge, null),

                new Wire<BookShelf>(Data, null),
                new Wire<BookShelf1>(Data1, null),
                new Wire<BookShelf2>(Data2, null),
                new Wire<LargeBookShelf>(DataLarge, null),

                new SlimSerializer<BookShelf>(Data, null),
                new SlimSerializer<BookShelf1>(Data1, null),
                new SlimSerializer<BookShelf2>(Data2, null),
                new SlimSerializer<LargeBookShelf>(DataLarge, null),

                new BinaryFormatter<BookShelf>(Data, null),
                new BinaryFormatter<BookShelf1>(Data1, null),
                new BinaryFormatter<BookShelf2>(Data2, null),
                new BinaryFormatter<LargeBookShelf>(DataLarge, null),

                new FastJson<BookShelf>(Data, null),
                new FastJson<BookShelf1>(Data1, null),
                new FastJson<BookShelf2>(Data2, null),
                new FastJson<LargeBookShelf>(DataLarge, null),

                new Jil<BookShelf>(Data, null),
                new Jil<BookShelf1>(Data1, null),
                new Jil<BookShelf2>(Data2, null),
                new Jil<LargeBookShelf>(DataLarge, null),

                new DataContract<BookShelf>(Data, null),
                new DataContract<BookShelf1>(Data1, null),
                new DataContract<BookShelf2>(Data2, null),
                new DataContract<LargeBookShelf>(DataLarge, null),

                new XmlSerializer<BookShelf>(Data, null),
                new XmlSerializer<BookShelf1>(Data1, null),
                new XmlSerializer<BookShelf2>(Data2, null),
                new XmlSerializer<LargeBookShelf>(DataLarge, null),

                new JsonNet<BookShelf>(Data, null),
                new JsonNet<BookShelf1>(Data1, null),
                new JsonNet<BookShelf2>(Data2, null),
                new JsonNet<LargeBookShelf>(DataLarge, null),

                new Protobuf_net<BookShelf>(Data, null),
                new Protobuf_net<BookShelf1>(Data1, null),
                new Protobuf_net<BookShelf2>(Data2, null),
                new Protobuf_net<LargeBookShelf>(DataLarge, null),

                new MessagePackSharp<BookShelf>(Data, null),
                new MessagePackSharp<BookShelf1>(Data1, null),
                new MessagePackSharp<BookShelf2>(Data2, null),
                new MessagePackSharp<LargeBookShelf>(DataLarge, null),

                new MsgPack_Cli<BookShelf>(Data, null),
                new MsgPack_Cli<BookShelf1>(Data1, null),
                new MsgPack_Cli<BookShelf2>(Data2, null),
                new MsgPack_Cli<LargeBookShelf>(DataLarge, null),
            };

        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            try
            {
                new Program(args).Run();
            }
            catch (Exception ex)
            {
                PrintHelp(ex);
            }
        }

        static void PrintHelp(Exception ex=null)
        {
            Console.WriteLine(Help);
            if( ex != null )
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        private void Run()
        {
            string testCase = null;

            while (Args.Count > 0)
            {
                string curArg = Args.Dequeue();
                string lowerArg = curArg.ToLower();

                switch (lowerArg)
                {
                    case "-runs":
                        string n = NextLower();
                        Runs = int.Parse(n);
                        break;
                    case "-notouch":
                        IsTouch = false;
                        break;
                    case "-test":
                        testCase = NextLower();
                        break;
                    case "-nongenwarn":
                        IsNGenWarn = false;
                        break;
                    default:
                        throw new NotSupportedException($"Argument {curArg} is not valid");
                }
            }

            PreChecks();


            if (testCase?.Equals("serialize") == true)
            {
                Serialize();
            }
            else if (testCase?.Equals("deserialize") == true)
            {
                Deserialize();
            }
            else if (testCase?.Equals("firstcall") == true)
            {
                FirstCall();
            }
            else if (testCase?.Equals("combined") == true)
            {
                Combined();
            }
            else
            {
                throw new NotSupportedException($"Error: Arg {testCase} is not a valid option!");
            }
        }

        private void PreChecks()
        {
            // Since XmlSerializer tries to load a pregenerated serialization assembly which will on first access read the GAC contents from the registry and cache them
            // we do this before to measure not the overhead of an failed assembly load try but only the overhead of the code gen itself.
            try
            {
                Assembly.Load("notExistingToTriggerGACPrefetch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }
            catch (FileNotFoundException)
            {
            }

            if (IsNGenWarn && !IsNGenned())
            {
                Console.WriteLine( "Warning: Not NGenned! Results may not be accurate in your target deployment.");
                Console.WriteLine(@"Please execute Ngen.cmd install to Ngen all dlls.");
                Console.WriteLine(@"To uninstall call Ngen.cmd uninstall");
                Console.WriteLine(@"The script will take care that the assemblies are really uninstalled. NGen is a bit tricky there.");
            }

            WarnIfDebug();
        }


        private void Deserialize()
        {
            var tester = new Test_O_N_Behavior(SerializersToTest);
            tester.TestDeserialize(nRuns: Runs);
        }

        private void Serialize()
        {
            var tester = new Test_O_N_Behavior(SerializersToTest);
            tester.TestSerialize(nRuns: Runs);
        }

        private void Combined()
        {
            var tester = new Test_O_N_Behavior(SerializersToTest);
            tester.TestCombined(nRuns: Runs);
        }


        /// <summary>
        /// Test for each serializer 5 different types the first call effect
        /// </summary>
        private void FirstCall()
        {
            var tester = new Test_O_N_Behavior(StartupSerializersToTest);
            tester.TestSerialize(nObjects: 1, nRuns:1);
        }

        string NextLower()
        {
            if( Args.Count > 0 )
            {
                return Args.Dequeue().ToLower();
            }

            return null;
        }

        private bool IsNGenned()
        {
            bool lret = false;
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                string file = module.ModuleName;
                if( file == "SerializerTests.ni.exe" )
                {
                    lret = true;
                }
            }

            return lret;
        }

        [Conditional("DEBUG")]
        void WarnIfDebug()
        {
            Console.WriteLine();
            Console.WriteLine("DEBUG build detected. Please recompile in Release mode before publishing your data.");
        }


        BookShelf Data(int nToCreate)
        {
            var lret = new BookShelf("private member value")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new Book { Id = i, Title = $"Book {i}" }).ToList()
            };
            return lret;
        }

        BookShelfFlat DataFlat(int nToCreate)
        {
            var builder = new FlatBufferBuilder(1024);

            Offset<BookFlat>[] books = new Offset<BookFlat>[nToCreate];

            for(int i=1;i<=nToCreate;i++)
            {
                var title = builder.CreateString($"Book {i}");
                var bookOffset = BookFlat.CreateBookFlat(builder, title, i);
                books[i - 1] = bookOffset;
            }

            var secretOffset = builder.CreateString("private member value");
            VectorOffset booksVector = builder.CreateVectorOfTables<BookFlat>(books);
            var lret = BookShelfFlat.CreateBookShelfFlat(builder, booksVector, secretOffset);
            builder.Finish(lret.Value);
            var bookshelf = BookShelfFlat.GetRootAsBookShelfFlat(builder.DataBuffer);
            return bookshelf;
        }

        /// <summary>
        /// Call all setters once to get a feeling for the deserialization overhead
        /// </summary>
        /// <param name="data"></param>
        void TouchFlat(BookShelfFlat data)
        {
           if (!IsTouch) return;

           string tmpTitle = null;
           int tmpId = 0;
           for(int i=0;i<data.BooksLength;i++)
            {
                var book = data.Books(i);
                tmpTitle = book.Value.Title;
                tmpId = book.Value.Id;
            }
        }

        ZeroFormatterBookShelf  DataZeroFormatter(int nToCreate)
        {
            var shelf = new ZeroFormatterBookShelf
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new ZeroFormatterBook { Id = i, Title = $"Book {i}" }).ToList()
            };
            return shelf;
        }

        ZeroFormatterBookShelf1 DataZeroFormatter1(int nToCreate)
        {
            var shelf = new ZeroFormatterBookShelf1
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new ZeroFormatterBook1 { Id = i, Title = $"Book {i}" }).ToList()
            };
            return shelf;
        }

        ZeroFormatterBookShelf2 DataZeroFormatter2(int nToCreate)
        {
            var shelf = new ZeroFormatterBookShelf2
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new ZeroFormatterBook2 { Id = i, Title = $"Book {i}" }).ToList()
            };
            return shelf;
        }

        ZeroFormatterLargeBookShelf DataZeroFormatterLarge(int nToCreate)
        {
            var lret = new ZeroFormatterLargeBookShelf("private member value2")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new ZeroFormatterLargeBook { Id = i, Title = $"Book {i}" }).ToList()
            };
            return lret;
        }

        void TouchZeroFormatterShelf(ZeroFormatterBookShelf data)
        {
            if (!IsTouch) return;

            string tmpTitle = null;
            int tmpId = 0;
            for(int i=0;i<data.Books.Count;i++)
            {
                tmpTitle = data.Books[i].Title;
                tmpId = data.Books[i].Id;
            }
        }


        void TouchBookShelf(BookShelf data)
        {
            if (!IsTouch) return;

            string tmpTitle = null;
            int tmpId = 0;
            for(int i=0;i<data.Books.Count;i++)
            {
                tmpTitle = data.Books[i].Title;
                tmpId = data.Books[i].Id;
            }
        }

        BookShelf1 Data1(int nToCreate)
        {
            var lret = new BookShelf1("private member value1")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new Book1 { Id = i, Title = $"Book {i}" }).ToList()
            };
            return lret;
        }

        BookShelf2 Data2(int nToCreate)
        {
            var lret = new BookShelf2("private member value2")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new Book2 { Id = i, Title = $"Book {i}" }).ToList()
            };
            return lret;
        }

        LargeBookShelf DataLarge(int nToCreate)
        {
            var lret = new LargeBookShelf("private member value2")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new LargeBook { Id = i, Title = $"Book {i}" }).ToList()
            };
            return lret;
        }

    }
}
