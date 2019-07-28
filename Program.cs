using FlatBuffers;
using SerializerTests.Serializers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
                             "SerializerTests [-Runs dd] -test [serialize, deserialize, combined, firstCall] [-reftracking] [-maxobj dd]" + Environment.NewLine +
                             " -Runs           Default is 5. The result is averaged where the first run is excluded from the average" + Environment.NewLine +
                             " -test xx        xx can be serialize, deserialize, combined or firstcall to test a scenario for many different serializers" + Environment.NewLine +
                             " -reftracking    If set a list with many identical references is serialized." + Environment.NewLine +
                             " -maxobj dd      Sets an upper limit how many objects are serialized. By default 1 up to 1 million objects are serialized" + Environment.NewLine +
                             " -serializer xxx Execute the test only for a specific serializer with the name xxx where multiple ones can be used with ," + Environment.NewLine +
                             " -list           List all registered serializers" + Environment.NewLine +
                             " -notouch        Do not touch the deserialized objects to test lazy deserialization" + Environment.NewLine +
                             "                 To execute deserialize you must first have called the serialize to generate serialized test data on disk to be read during deserialize" + Environment.NewLine +
                             "Examples" + Environment.NewLine +
                             "Compare protobuf against MessagePackSharp for serialize and deserialize performance" + Environment.NewLine +
                             " SerializerTests -Runs 1 -test combined -serializer protobuf,MessagePackSharp" + Environment.NewLine +
                             "Test how serializers perform when reference tracking is enabled. Currently that are BinaryFormatter,Protobuf_net and DataContract" + Environment.NewLine + 
                             " Although Json.NET claim to have but it is not completely working." + Environment.NewLine +
                             " SerializerTests -Runs 1 -test combined -reftracking" + Environment.NewLine +
                             "Speed up tests by testing only up to 300K serialized objects" + Environment.NewLine +
                             " SerializerTests -test combined -maxobj 300000" + Environment.NewLine;


        private Queue<string> Args;

        List<ISerializeDeserializeTester> SerializersToTest;
        List<ISerializeDeserializeTester> StartupSerializersToTest;
        List<ISerializeDeserializeTester> SerializersObjectReferencesToTest;

        int Runs = 5;
        bool IsNGenWarn = true;
        bool IsTouch = true;
        bool TestReferenceTracking = false;
        int MaxObjectCount = 1000 * 1000;
        string[] SerializerFilters = new string [] { "" };


        public Program(string[] args)
        {
            Args = new Queue<string>(args);
        }

        private void CreateSerializersToTest()
        {
            // used when on command line -serializer is used
            Func<ISerializeDeserializeTester, bool> filter = (s) =>
            {
                return SerializerFilters.Any(f => s.GetType().Name.IndexOf(f, StringComparison.OrdinalIgnoreCase) == 0);
            };

            SerializersToTest = new List<ISerializeDeserializeTester>
            {
                new ApexSerializer<BookShelf>(Data, Touch),
#if NETCOREAPP3_0
                new NetCoreJsonSerializer<NetCorePropertyBookShelf>(DataNetCore, Touch),
                new SimdJsonSharpSerializer<BookShelf>(Data, Touch),
                new SimdJsonSharpSerializerN<BookShelf>(Data, Touch),
#endif
				new Utf8JsonSerializer<BookShelf>(Data, Touch),
                new MessagePackSharp<BookShelf>(Data, Touch),
                new GroBuf<BookShelf>(Data, Touch),
                new FlatBuffer<BookShelfFlat>(DataFlat, TouchFlat),
#if NET472
                // Hyperion does not work on .NET Core 3.0  https://github.com/akkadotnet/Hyperion/issues/111
                // new Hyperion<BookShelf>(Data, Touch),
                // https://github.com/rogeralsing/Wire/issues/146
                // new Wire<BookShelf>(Data, Touch),
#endif
                new Bois<BookShelf>(Data, Touch),
                new Jil<BookShelf>(Data, Touch),
                new Protobuf_net<BookShelf>(Data, Touch),
                new SlimSerializer<BookShelf>(Data, Touch),


#if NET472  
                // ZeroFormatter crashes on .NET Core 3 during serialize with: System.BadImageFormatException: 'Bad IL format.'
                // new ZeroFormatter<ZeroFormatterBookShelf>(DataZeroFormatter, Touch),
#endif
                new ServiceStack<BookShelf>(Data, Touch),
                new FastJson<BookShelf>(Data, Touch),
                //new DataContractIndented<BookShelf>(Data, TouchBookShelf),
                new DataContractBinaryXml<BookShelf>(Data, Touch),
                new DataContract<BookShelf>(Data, Touch),
                new XmlSerializer<BookShelf>(Data, Touch),
                new JsonNet<BookShelf>(Data, Touch),
                new MsgPack_Cli<BookShelf>(Data, Touch),
                new BinaryFormatter<BookShelf>(Data, Touch),
            };

            // if on command line a filter was specified filter the serializers to test according to filter by type name 
            SerializersToTest = SerializersToTest.Where(filter).ToList();
            

            StartupSerializersToTest = new List<ISerializeDeserializeTester>
            {
                new ApexSerializer<BookShelf>(Data, null),
                new ApexSerializer<BookShelf1>(Data1, null),
                new ApexSerializer<BookShelf2>(Data2, null),
                new ApexSerializer<LargeBookShelf>(DataLarge, null),

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

#if NET472  
                // ZeroFormatter crashes on .NET Core 3 during serialize with: System.BadImageFormatException: 'Bad IL format.'
                //new ZeroFormatter<ZeroFormatterBookShelf>(DataZeroFormatter, null),
                //new ZeroFormatter<ZeroFormatterBookShelf1>(DataZeroFormatter1, null),
                //new ZeroFormatter<ZeroFormatterBookShelf2>(DataZeroFormatter2, null),
                //new ZeroFormatter<ZeroFormatterLargeBookShelf>(DataZeroFormatterLarge, null),
#endif


#if NET472
                // Hyperion does not work on .NET Core 3.0  https://github.com/akkadotnet/Hyperion/issues/111
                //new Hyperion<BookShelf>(Data, null),
                //new Hyperion<BookShelf1>(Data1, null),
                //new Hyperion<BookShelf2>(Data2, null),
                //new Hyperion<LargeBookShelf>(DataLarge, null),

                // Wire crashes on Deserializaiton on .NET Core 3.0 https://github.com/rogeralsing/Wire/issues/146
                //new Wire<BookShelf>(Data, null),
                //new Wire<BookShelf1>(Data1, null),
                //new Wire<BookShelf2>(Data2, null),
                //new Wire<LargeBookShelf>(DataLarge, null),
#endif
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

	            new Utf8JsonSerializer<BookShelf>(Data, null),
	            new Utf8JsonSerializer<BookShelf1>(Data1, null),
	            new Utf8JsonSerializer<BookShelf2>(Data2, null),
	            new Utf8JsonSerializer<LargeBookShelf>(DataLarge, null),
            };

            StartupSerializersToTest = StartupSerializersToTest.Where(filter).ToList();

            SerializersObjectReferencesToTest = new List<ISerializeDeserializeTester>
            {
                new ApexSerializer<ReferenceBookShelf>(DataReferenceBookShelf, null),
                // FlatBuffer does not support object references
                new MessagePackSharp<ReferenceBookShelf>(DataReferenceBookShelf, null),
                new GroBuf<ReferenceBookShelf>(DataReferenceBookShelf, null),

#if NET472
                // Hyperion does not work on .NET Core 3.0  https://github.com/akkadotnet/Hyperion/issues/111
                //new Hyperion<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking: TestReferenceTracking),
                // Wire crashes on Deserialization on .NET Core 3.0 https://github.com/rogeralsing/Wire/issues/146
                //new Wire<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking: TestReferenceTracking),
#endif
                new Bois<ReferenceBookShelf>(DataReferenceBookShelf, null),
                //new Jil<ReferenceBookShelf>(DataReferenceBookShelf, null),  // Jil does not support a dictionary with DateTime as key
                new Protobuf_net<ReferenceBookShelf>(DataReferenceBookShelf, null),  // Reference tracking in protobuf can be enabled via attributes in the types!
                new SlimSerializer<ReferenceBookShelf>(DataReferenceBookShelf, null),
#if NET472  
                // ZeroFormatter crashes on .NET Core 3 during serialize with: System.BadImageFormatException: 'Bad IL format.'
                //new ZeroFormatter<ReferenceBookShelf>(DataReferenceBookShelf, null),
#endif
                new ServiceStack<ReferenceBookShelf>(DataReferenceBookShelf, null),
                // new FastJson<ReferenceBookShelf>(DataReferenceBookShelf, null), // DateTime strings are not round trip capable because FastJSON keeps the time only until ms but the rest is not serialized!
                new DataContractIndented<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking:TestReferenceTracking),
                new DataContractBinaryXml<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking:TestReferenceTracking),
                new DataContract<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking:TestReferenceTracking),
                // new XmlSerializer<ReferenceBookShelf>(DataReferenceBookShelf, null),  // XmlSerializer does not support Dictionaries https://stackoverflow.com/questions/2911514/why-doesnt-xmlserializer-support-dictionary
                new JsonNet<ReferenceBookShelf>(DataReferenceBookShelf, null, refTracking:TestReferenceTracking),
                new MsgPack_Cli<ReferenceBookShelf>(DataReferenceBookShelf, null),
                new BinaryFormatter<ReferenceBookShelf>(DataReferenceBookShelf, null),
				new Utf8JsonSerializer<ReferenceBookShelf>(DataReferenceBookShelf, null)
            };

            SerializersObjectReferencesToTest = SerializersObjectReferencesToTest.Where(filter).ToList();
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
                    case "-reftracking":
                        MaxObjectCount = 300*1000;
                        TestReferenceTracking = true;
                        break;
                    case "-serializer":
                        string serializers = NextLower() ?? "";
                        SerializerFilters = serializers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                        break;
                    case "-list":
                        CreateSerializersToTest();
                        Console.WriteLine("Registered Serializers");
                        foreach (var test in SerializersToTest)
                        {
                            Console.WriteLine($"{test.GetType().Name.TrimEnd('1').TrimEnd('`') }");
                        }
                        return;
                    case "-notouch":
                        IsTouch = false;
                        break;
                    case "-maxobj":
                        string maxobj = NextLower();
                        MaxObjectCount = int.Parse(maxobj);
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

            CreateSerializersToTest();

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


        /// <summary>
        /// Return right set of serializers depending on requested test
        /// </summary>
        private List<ISerializeDeserializeTester> TestSerializers
        {
            get {  return TestReferenceTracking ? SerializersObjectReferencesToTest : SerializersToTest;  }
        }


        private void Deserialize()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestDeserialize(maxNObjects: MaxObjectCount, nRuns: Runs);
        }

        private void Serialize()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestSerialize(maxNObjects:MaxObjectCount, nRuns: Runs);
        }

        private void Combined()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestCombined(maxNObjects: MaxObjectCount, nRuns: Runs);
        }


        /// <summary>
        /// Test for each serializer 5 different types the first call effect
        /// </summary>
        private void FirstCall()
        {
            var tester = new Test_O_N_Behavior(StartupSerializersToTest);
            tester.TestSerialize(maxNObjects: 1, nRuns:1);
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

        NetCorePropertyBookShelf DataNetCore(int nToCreate)
        {
            var lret = new NetCorePropertyBookShelf("private member value")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i => new NetCoreBook { Id = i, Title = $"Book {i}" }).ToList()
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

        void Touch(NetCorePropertyBookShelf data)
        {
            if (IsTouch) return;

            string tmpTitle = null;
            int tmpId = 0;
            for (int i = 0; i < data.Books.Count; i++)
            {
                var book = data.Books[i];
                tmpTitle = book.Title;
                tmpId = book.Id;
            }
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

        void Touch(ZeroFormatterBookShelf data)
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


        void Touch(BookShelf data)
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

        ReferenceBookShelf DataReferenceBookShelf(int nToCreate)
        {
            var lret = new ReferenceBookShelf();
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<10;i++)
            {
                sb.Append("This is a really long string");
            }
            string largeStrSameReference = sb.ToString();

            for (int i = 1; i <= nToCreate; i++)
            {
                var book = new ReferenceBook()
                {
                    Container = null,
                    Name = largeStrSameReference,
                    Price = i
                };
                lret.Books.Add(new DateTime(DateTime.MinValue.Ticks+i, DateTimeKind.Utc), book);
            }
            return lret;
        }

    }
}
