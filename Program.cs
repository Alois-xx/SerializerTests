using Google.FlatBuffers;
using Google.Protobuf;
using SerializerTests.Serializers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
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

        3. Compile it in Release 
        4. Call RunAll.cmd from the source folder to execute tests for .NET Framework, .NET Core 3.1 and .NET 5.0
        5. Publish the results on your own blog.
    }
    */


    class Program
    {
        static string Help = "SerializerTests is a serializer performance testing framework to evaluate and compare different serializers for .NET by Alois Kraus" + Environment.NewLine +
                             "SerializerTests [-Runs dd] -test [serialize, deserialize, combined, firstCall] [-reftracking] [-maxobj dd] [-NoHeader]" + Environment.NewLine +
                             " -N 1,2,10000    Set the number of objects to de/serialize which is repeated -Runs times to get stable results." + Environment.NewLine +
                             " -Runs           Default is 5. The result is averaged where the first run is excluded from the average" + Environment.NewLine +
                             " -test xx        xx can be serialize, deserialize, combined or firstcall to test a scenario for many different serializers" + Environment.NewLine +
                             " -reftracking    If set a list with many identical references is serialized." + Environment.NewLine +
                             " -serializer xxx Execute the test only for a specific serializer with the name xxx. Use , to separate multiple filters. Prefix name with # to force a full string match instead of a substring match." + Environment.NewLine +
                             " -list           List all registered serializers" + Environment.NewLine +
                             " -BookDataSize d Optional byte array payload in bytes to check how good the serializer can deal with large blob payloads (e.g. images)." + Environment.NewLine +
                             " -Verify         Verify deserialized data if all contents could be read." + Environment.NewLine +
                             "                 To execute deserialize you must first have called the serialize to generate serialized test data on disk to be read during deserialize" + Environment.NewLine +
                             " -NoHeader       Do not print CSV header to enable easy append of different test runs into one file." + Environment.NewLine + 
                             "Examples" + Environment.NewLine +
                             "Compare protobuf against MessagePackSharp for serialize and deserialize performance" + Environment.NewLine +
                             " SerializerTests -Runs 1 -test combined -serializer protobuf,MessagePackSharp" + Environment.NewLine +
                             "Test how serializers perform when reference tracking is enabled. Currently that are BinaryFormatter,Protobuf_net and DataContract" + Environment.NewLine +
                             " Although Json.NET claim to have but it is not completely working." + Environment.NewLine +
                             " SerializerTests -Runs 1 -test combined -reftracking" + Environment.NewLine +
                             "Test SimdJsonSharpSerializer serializer with 3 million objects for serialize and deserialize." + Environment.NewLine +
                             " SerializerTests -test combined -N 3000000 -serializer #SimdJsonSharpSerializer" + Environment.NewLine;


        private Queue<string> Args;

        List<ISerializeDeserializeTester> SerializersToTest;
        List<ISerializeDeserializeTester> StartupSerializersToTest;
        List<ISerializeDeserializeTester> SerializersObjectReferencesToTest;

        int Runs = 5;
        public int BookDataSize = 0;
        bool IsNGenWarn = true;
        bool VerifyAndTouch = false;
        bool TestReferenceTracking = false;
        int[] NObjectsToDeSerialize = null;
        string[] SerializerFilters = new string[] { "" };

        const int StartupSerializerCount = 4;

        public Program(string[] args)
        {
            Args = new Queue<string>(args);
        }

        private void CreateSerializersToTest()
        {
            // used when on command line -serializer is used
            Func<ISerializeDeserializeTester, bool> filter = (s) =>
            {
                return SerializerFilters.Any(filterStr =>
                {
                    string simpleType = GetSimpleTypeName(s.GetType());

                    if (filterStr.StartsWith("#")) // Exact type match needed
                    {
                        return String.Equals(filterStr.Substring(1), simpleType, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        return simpleType.IndexOf(filterStr, StringComparison.OrdinalIgnoreCase) == 0;
                    }
                });
            };

            SerializersToTest = new List<ISerializeDeserializeTester>
            {

                new AllocPerf<BookShelf>(Data, null),
                // Apex Serializer works only on .NET Core 3.0! .NET Core 3.1 and 5.0 break with some internal FileNotFoundExeptions which breaks serialization/deserialization
                // InvalidOperationException: Type SerializerTests.TypesToSerialize.BookShelf was encountered during deserialization but was not marked as serializable. Use Binary.MarkSerializable before creating any serializers if this type is intended to be serialized.
#if  NETCOREAPP3_0
                new ApexSerializer<BookShelf>(Data, TouchAndVerify),
#endif

#if (NETCOREAPP3_0_OR_GREATER)
                new MemoryPack<BookShelf>(Data, TouchAndVerify),
                new SerializerTests.Serializers.BinaryPack<BookShelf>(Data, TouchAndVerify),
#endif

                new Ceras<BookShelf>(Data, TouchAndVerify),

#if NET7_0_OR_GREATER
                new SystemTextJsonSourceGen<BookShelf>(Data, TouchAndVerify),
#endif

#if NET5_0_OR_GREATER
                // .NET 5 supports public fields with the builtin serializer now
                new SystemTextJson<BookShelf>(Data, TouchAndVerify),

                // SwifterJson crashes when during deserialize due to too large memory requirements and during the "normal test" it crashes with 
                // SerializerTests.exe -test combined -N 100000,200000,500000,1000000 -serializer swifter
                // SerializerTests (13380): 2 exceptions
                //    Json deserialize failed.:   Json deserialize failed.: 1 exception
                //        Catch: instance void [Swifter.Json] Swifter.Json.JsonFormatter +< DeserializeObjectAsync > d__134`1[System.__Canon]::MoveNext()
                //            Swifter.Json.JsonDeserializer`1[Swifter.Json.JsonDeserializeModes + Verified].FastReadObject(class Swifter.RW.IDataWriter`1<value class Swifter.Tools.Ps`1<wchar>>)
			    //   FastObjectRWCreater_Book_d1913bb0372f40a5a3ca8ae5210d4a51.ReadValue(class Swifter.RW.IValueReader)
                //   Swifter.RW.ListRW`2[System.__Canon, System.__Canon].OnWriteValue(int32,class Swifter.RW.IValueReader)
                //   Swifter.Json.JsonDeserializer`1[Swifter.Json.JsonDeserializeModes+Verified].SlowReadArray(class Swifter.RW.IDataWriter`1<int32>)
                //   Swifter.Json.JsonDeserializer`1[Swifter.Json.JsonDeserializeModes+Verified].ReadArray(class Swifter.RW.IDataWriter`1<int32>)
                //   Swifter.RW.ListInterface`2[System.__Canon, System.__Canon].ReadValue(class Swifter.RW.IValueReader)
                //   FastObjectRW_BookShelf_b2eb1b0060b9498db34ca3b37cfb0ec2.OnWriteValue(value class Swifter.Tools.Ps`1<wchar>,class Swifter.RW.IValueReader)
                //   Swifter.Json.JsonDeserializer`1[Swifter.Json.JsonDeserializeModes+Verified].FastReadObject(class Swifter.RW.IDataWriter`1<value class Swifter.Tools.Ps`1<wchar>>)
                //   FastObjectRWCreater_BookShelf_ac8592457cd04421981eeb0a06342bf0.ReadValue(class Swifter.RW.IValueReader)
                //   Swifter.Json.JsonFormatter.DeserializeObject(wchar*, int32)
                //   Swifter.Json.JsonFormatter.DeserializeObject(class System.String)
                //   Swifter.Json.JsonFormatter+<DeserializeObjectAsync>d__134`1[System.__Canon].MoveNext()
                //   System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start(!!0&)
                //   System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder`1[System.__Canon].Start(!!0&)
                //   Swifter.Json.JsonFormatter.DeserializeObjectAsync(class System.IO.Stream,class System.Text.Encoding)
                //   SerializerTests.Serializers.SwifterJson`1[System.__Canon].Deserialize(class System.IO.Stream)
                //   SerializerTests.TestBase`2[System.__Canon, System.__Canon].TestDeserializeOnlyAndTouch(class System.IO.MemoryStream,int32,!0&)
                //   SerializerTests.TestBase`2+<>c__DisplayClass39_0[System.__Canon, System.__Canon].<TestDeserialize>b__0()
                //   SerializerTests.TestBase`2[System.__Canon, System.__Canon].Test(int32,class System.Action,bool)
                //   SerializerTests.TestBase`2[System.__Canon, System.__Canon].TestDeserialize(int32, int32)
                //   SerializerTests.Test_O_N_Behavior.TestCombined(int32[], int32)
                //   SerializerTests.Program.Combined()
                //   SerializerTests.Program.Run()
                //   SerializerTests.Program.Main(class System.String[])
                //new SwifterJson<BookShelf>(Data, TouchAndVerify),
#endif
#if (NETCOREAPP3_1 || NETCOREAPP3_0) && !NET5_0
                // .NET Core 3/3.1 do not support public fields so we needed to resort back to public properties
                new SystemTextJson<NetCorePropertyBookShelf>(DataNetCore, TouchAndVerify),
#endif
#if NETCOREAPP3_1_OR_GREATER
                new SimdJsonSharpSerializer<BookShelf>(Data, TouchAndVerify),
                new SpanJson<BookShelf>(Data, TouchAndVerify),
#endif
                new Utf8JsonSerializer<BookShelf>(Data, TouchAndVerify),
                new MessagePackSharp<BookShelf>(Data, TouchAndVerify),
                new GroBuf<BookShelf>(Data, TouchAndVerify),
                new FlatBuffer<BookShelfFlat>(DataFlat, TouchFlat),
#if NET472
                // Hyperion does not work on .NET Core 3.0  https://github.com/akkadotnet/Hyperion/issues/111
                // new Hyperion<BookShelf>(Data, Touch),
                // https://github.com/rogeralsing/Wire/issues/146
                // new Wire<BookShelf>(Data, Touch),
#endif
                new Bois<BookShelf>(Data, TouchAndVerify),
                new Bois_LZ4<BookShelf>(Data, TouchAndVerify),
                new Jil<BookShelf>(Data, TouchAndVerify),
                new Protobuf_net<BookShelf>(Data, TouchAndVerify),
                new GoogleProtobuf<ProtocBookShelf>(ProtocData, TouchAndVerify),
                new SlimSerializer<BookShelf>(Data, TouchAndVerify),


#if NET472
                // ZeroFormatter crashes on .NET Core 3 during serialize with: System.BadImageFormatException: 'Bad IL format.'
                // new ZeroFormatter<ZeroFormatterBookShelf>(DataZeroFormatter, Touch),
#endif
                new ServiceStack<BookShelf>(Data, TouchAndVerify),
                new FastJson<BookShelf>(Data, TouchAndVerify),
                //new DataContractIndented<BookShelf>(Data, TouchBookShelf),
                new DataContractBinaryXml<BookShelf>(Data, TouchAndVerify),
                new DataContract<BookShelf>(Data, TouchAndVerify),
                new XmlSerializer<BookShelf>(Data, TouchAndVerify),
                new JsonNet<BookShelf>(Data, TouchAndVerify),
                new MsgPack_Cli<BookShelf>(Data, TouchAndVerify),
                new BinaryFormatter<BookShelf>(Data, TouchAndVerify),
            };

            // if on command line a filter was specified filter the serializers to test according to filter by type name 
            SerializersToTest = SerializersToTest.Where(filter).ToList();


            StartupSerializersToTest = new List<ISerializeDeserializeTester>
            {
                new Ceras<BookShelf>(Data, null),
                new Ceras<BookShelf1>(Data1, null),
                new Ceras<BookShelf2>(Data2, null),
                new Ceras<LargeBookShelf>(DataLarge, null),

#if NETCOREAPP3_0
                new ApexSerializer<BookShelf>(Data, null),
                new ApexSerializer<BookShelf1>(Data1, null),
                new ApexSerializer<BookShelf2>(Data2, null),
                new ApexSerializer<LargeBookShelf>(DataLarge, null),
#endif

#if (NETCOREAPP3_0_OR_GREATER)
                new MemoryPack<BookShelf>(Data, null),
                new MemoryPack<BookShelf1>(Data1, null),
                new MemoryPack<BookShelf2>(Data2, null),
                new MemoryPack<LargeBookShelf>(DataLarge, null),

                new SerializerTests.Serializers.BinaryPack<BookShelf>(Data, null),
                new SerializerTests.Serializers.BinaryPack<BookShelf1>(Data1, null),
                new SerializerTests.Serializers.BinaryPack<BookShelf2>(Data2, null),
                new SerializerTests.Serializers.BinaryPack<LargeBookShelf>(DataLarge, null),
#endif

                new ServiceStack<BookShelf>(Data, null),
                new ServiceStack<BookShelf1>(Data1, null),
                new ServiceStack<BookShelf2>(Data2, null),
                new ServiceStack<LargeBookShelf>(DataLarge, null),

                new Bois<BookShelf>(Data, null),
                new Bois<BookShelf1>(Data1, null),
                new Bois<BookShelf2>(Data2, null),
                new Bois<LargeBookShelf>(DataLarge, null),

                new Bois_LZ4<BookShelf>(Data, null),
                new Bois_LZ4<BookShelf1>(Data1, null),
                new Bois_LZ4<BookShelf2>(Data2, null),
                new Bois_LZ4<LargeBookShelf>(DataLarge, null),

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

#if NET5_0_OR_GREATER
                new SystemTextJson<BookShelf>(Data, null),
                new SystemTextJson<BookShelf1>(Data1, null),
                new SystemTextJson<BookShelf2>(Data2, null),
                new SystemTextJson<LargeBookShelf>(DataLarge, null),
#endif

#if NET7_0_OR_GREATER
                new SystemTextJsonSourceGen<BookShelf>(Data, null),
                new SystemTextJsonSourceGen<BookShelf1>(Data1, null),
                new SystemTextJsonSourceGen<BookShelf2>(Data2, null),
                new SystemTextJsonSourceGen<LargeBookShelf>(DataLarge, null),
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

                new GoogleProtobuf<ProtocBookShelf>(ProtocData, null),
                new GoogleProtobuf<ProtocBookShelf1>(ProtocData1, null),
                new GoogleProtobuf<ProtocBookShelf2>(ProtocData2, null),
                new GoogleProtobuf<ProtocLargeBookShelf>(ProtocDataLarge, null),

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
                 // Apex Serializer works only on .NET Core 3.0 3.1 and 5.0 break with some internal FileNotFoundExeptions which apparently break serialization/deserialization
                // InvalidOperationException: Type SerializerTests.TypesToSerialize.BookShelf was encountered during deserialization but was not marked as serializable. Use Binary.MarkSerializable before creating any serializers if this type is intended to be serialized.
#if NETCOREAPP3_0
                new ApexSerializer<ReferenceBookShelf>(DataReferenceBookShelf, null),
#endif
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
                new Bois_LZ4<ReferenceBookShelf>(DataReferenceBookShelf, null),
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

        static void PrintHelp(Exception ex = null)
        {
            Console.WriteLine(Help);
            if (ex != null)
            {
#if DEBUG
                Console.WriteLine($"{ex}");
#else
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Return only the non generic type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string GetSimpleTypeName(Type type)
        {
            return type.Name.TrimEnd('1').TrimEnd('`');
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
                        TestReferenceTracking = true;
                        break;
                    case "-serializer":
                        string serializers = NextLower() ?? "";
                        SerializerFilters = serializers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "-list":
                        CreateSerializersToTest();
                        Console.WriteLine("Registered Serializers");
                        foreach (var test in SerializersToTest)
                        {
                            Console.WriteLine($"{GetSimpleTypeName(test.GetType()) }");
                        }
                        return;
                    case "-verify":
                        VerifyAndTouch = true;
                        break;
                    case "-n":
                        NObjectsToDeSerialize = NextLower()?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select(int.Parse).ToArray();
                        if (NObjectsToDeSerialize == null)
                        {
                            throw new NotSupportedException("Missing object count after -N option");
                        }
                        break;
                    case "-test":
                        testCase = NextLower();
                        break;
                    case "-bookdatasize":
                        BookDataSize = int.Parse(NextLower());
                        break;
                    case "-scenario":
                        Scenario = Next();
                        break;
                    case "-noheader":
                        NoHeader = true;
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

            // Set optional payload size to be able to generate data files with the payload size in the file name
            foreach (var x in SerializersToTest.Concat(StartupSerializersToTest).Concat(SerializersObjectReferencesToTest))
            {
                x.OptionalBytePayloadSize = BookDataSize;
            }

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
                Console.WriteLine("Warning: Not NGenned! Results may not be accurate in your target deployment.");
                Console.WriteLine(@"Please execute Ngen.cmd install as Administrator to Ngen all dlls.");
                Console.WriteLine(@"To uninstall call Ngen.cmd uninstall");
                Console.WriteLine(@"The script will take care that the assemblies are really uninstalled.");
            }

            WarnIfDebug();
        }


        /// <summary>
        /// Return right set of serializers depending on requested test
        /// </summary>
        private List<ISerializeDeserializeTester> TestSerializers
        {
            get { return TestReferenceTracking ? SerializersObjectReferencesToTest : SerializersToTest; }
        }

        public string Scenario { get; private set; }
        public bool NoHeader { get; private set; }

        private void Deserialize()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestDeserialize(NObjectsToDeSerialize, nRuns: Runs, scenario:Scenario, noHeader:NoHeader);
        }

        private void Serialize()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestSerialize(NObjectsToDeSerialize, nRuns: Runs, scenario: Scenario, noHeader: NoHeader);
        }

        private void Combined()
        {
            var tester = new Test_O_N_Behavior(TestSerializers);
            tester.TestCombined(NObjectsToDeSerialize, nRuns: Runs, scenario: Scenario, noHeader: NoHeader);
        }


        /// <summary>
        /// To measure things accurately we spawn a new process for every serializer and then create for 4 different types a serializer where some data is serialized
        /// </summary>
        private void FirstCall()
        {
            if (StartupSerializersToTest.Count == StartupSerializerCount) // we always create 4 serializer with different types for startup tests
            {
                var tester = new Test_O_N_Behavior(StartupSerializersToTest);
                tester.TestSerialize(new int[] { 1 }, nRuns: 1, scenario:Scenario, noHeader:NoHeader);
            }
            else
            {
                for (int i = 0; i < StartupSerializersToTest.Count; i += StartupSerializerCount)
                {
                    string output = null;
                    for (int warmupCount = 0; warmupCount < 3; warmupCount++) // do 2 iterations warmup to prevent hard faults which cost a lot of time and can totally skew measured values
                    {
                        var serializer = StartupSerializersToTest[i];
                        // Spawn new process for each serializer to measure each serializer overhead in isolation 
                        var startArgs = new ProcessStartInfo(Assembly.GetEntryAssembly().Location.Replace(".dll", ".exe"), String.Join(" ", Environment.GetCommandLineArgs().Skip(1)) + $" -serializer #{GetSimpleTypeName(serializer.GetType())}")
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                        };
                        Process proc = Process.Start(startArgs);
                        // trim newline of newly started process
                        output = proc.StandardOutput.ReadToEnd().Trim(Environment.NewLine.ToCharArray());
                        if (i > 0) // trim header since we need it only once
                        {
                            output = output.Substring(output.IndexOf('\n') + 1);
                        }
                        proc.WaitForExit();
                    }
                    Console.WriteLine(output);
                }
            }
        }

        string NextLower()
        {
            if (Args.Count > 0)
            {
                return Args.Dequeue().ToLower();
            }

            return null;
        }

        public string Next()
        {
            if (Args.Count > 0)
            {
                return Args.Dequeue();
            }

            return null;
        }


        private bool IsNGenned()
        {
            bool lret = false;
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                string file = module.ModuleName;
                if (file == "SerializerTests.ni.exe" || file == "coreclr.dll")
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
                Books = Enumerable.Range(1, nToCreate).Select(i => new Book
                {
                    Id = i,
                    Title = $"Book {i}",
                    BookData = CreateAndFillByteBuffer(),
                }
                ).ToList()
            };
            return lret;
        }

        NetCorePropertyBookShelf DataNetCore(int nToCreate)
        {
            var lret = new NetCorePropertyBookShelf("private member value")
            {
                Books = Enumerable.Range(1, nToCreate).Select(i =>
                new NetCoreBook
                {
                    Id = i,
                    Title = $"Book {i}",
                    BookData = CreateAndFillByteBuffer(),
                }).ToList()
            };
            return lret;
        }

        BookShelfFlat DataFlat(int nToCreate)
        {
            var builder = new FlatBufferBuilder(1024);

            Offset<BookFlat>[] books = new Offset<BookFlat>[nToCreate];

            for (int i = 1; i <= nToCreate; i++)
            {
                var title = builder.CreateString($"Book {i}");
                builder.StartVector(1, BookDataSize, 0);
                byte[] bytes = CreateAndFillByteBuffer();
                if (bytes.Length > 0)
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
            var bookshelf = BookShelfFlat.GetRootAsBookShelfFlat(builder.DataBuffer);
            return bookshelf;
        }

        ProtocBookShelf ProtocData(int nToCreate)
        {
            var bookData = ByteString.CopyFrom(CreateAndFillByteBuffer());
            var books = Enumerable.Range(1, nToCreate).Select(i =>
                new ProtocBook
                {
                    Id = i,
                    Title = $"Book {i}",
                    BookData = bookData,
                }).ToList();

            var lret = new ProtocBookShelf();
            lret.Books.AddRange(books);
            return lret;
        }

        ProtocBookShelf1 ProtocData1(int nToCreate)
        {
            var books = Enumerable.Range(1, nToCreate).Select(i =>
                new ProtocBook1
                {
                    Id = i,
                    Title = $"Book {i}",
                }).ToList();

            var lret = new ProtocBookShelf1();
            lret.Books.AddRange(books);
            return lret;
        }

        ProtocBookShelf2 ProtocData2(int nToCreate)
        {
            var books = Enumerable.Range(1, nToCreate).Select(i =>
                new ProtocBook2
                {
                    Id = i,
                    Title = $"Book {i}",
                }).ToList();

            var lret = new ProtocBookShelf2();
            lret.Books.AddRange(books);
            return lret;
        }


        ProtocLargeBookShelf ProtocDataLarge(int nToCreate)
        {
            var books = Enumerable.Range(1, nToCreate).Select(i =>
                new ProtocLargeBook
                {
                    Id = i,
                    Title = $"Book {i}",
                }).ToList();

            var lret = new ProtocLargeBookShelf();
            lret.Books.AddRange(books);
            return lret;
        }

        byte[] CreateAndFillByteBuffer()
        {
            byte[] optionalPayload = new byte[BookDataSize];

            for (int j = 0; j < optionalPayload.Length; j++)
            {
                optionalPayload[j] = (byte)(j % 26 + 'a');
            }

            return optionalPayload;
        }

        void TouchAndVerify(BookShelf data, int nExpectedBooks, int optionalPayloadDataSize)
        {
            if (!VerifyAndTouch)
            {
                return;
            }

            string tmpTitle = null;
            int tmpId = 0;

            if (nExpectedBooks != data.Books.Count)
            {
                throw new InvalidOperationException($"Number of deserialized Books was {data.Books.Count} but expected {nExpectedBooks}. This Serializer seem to have lost data.");
            }

            for (int i = 0; i < data.Books.Count; i++)
            {
                tmpTitle = data.Books[i].Title;
                tmpId = data.Books[i].Id;
                if (data.Books[i].Id != i + 1)
                {
                    throw new InvalidOperationException($"Book Id was {data.Books[i].Id} but exepcted {i + 1}");
                }
                if (optionalPayloadDataSize > 0 && data.Books[i].BookData.Length != optionalPayloadDataSize)
                {
                    throw new InvalidOperationException($"BookData length was {data.Books[i].BookData.Length} but expected {optionalPayloadDataSize}");
                }
            }
        }

        void TouchAndVerify(ProtocBookShelf data, int nExpectedBooks, int optionalPayloadDataSize)
        {
            if (!VerifyAndTouch)
            {
                return;
            }

            string tmpTitle = null;
            int tmpId = 0;

            if (nExpectedBooks != data.Books.Count)
            {
                throw new InvalidOperationException($"Number of deserialized Books was {data.Books.Count} but expected {nExpectedBooks}. This Serializer seem to have lost data.");
            }

            for (int i = 0; i < data.Books.Count; i++)
            {
                tmpTitle = data.Books[i].Title;
                tmpId = data.Books[i].Id;
                if (data.Books[i].Id != i + 1)
                {
                    throw new InvalidOperationException($"Book Id was {data.Books[i].Id} but exepcted {i + 1}");
                }
                if (optionalPayloadDataSize > 0 && data.Books[i].BookData.Length != optionalPayloadDataSize)
                {
                    throw new InvalidOperationException($"BookData length was {data.Books[i].BookData.Length} but expected {optionalPayloadDataSize}");
                }
            }
        }

        void TouchAndVerify(NetCorePropertyBookShelf data, int nExpectedBooks, int optionalPayloadDataSize)
        {
            if (!VerifyAndTouch)
            {
                return;
            }

            string tmpTitle = null;
            int tmpId = 0;

            if (nExpectedBooks != data.Books.Count)
            {
                throw new InvalidOperationException($"Number of deserialized Books was {data.Books.Count} but expected {nExpectedBooks}. This Serializer seem to have lost data.");
            }

            for (int i = 0; i < data.Books.Count; i++)
            {
                var book = data.Books[i];
                tmpTitle = book.Title;
                tmpId = book.Id;
                if (book.Id != i + 1)
                {
                    throw new InvalidOperationException($"Book Id was {book.Id} but exepcted {i + 1}");
                }
                if (optionalPayloadDataSize > 0 && book.BookData.Length != optionalPayloadDataSize)
                {
                    throw new InvalidOperationException($"BookData length was {book.BookData.Length} but expected {optionalPayloadDataSize}");
                }
            }
        }

        /// <summary>
        /// Call all setters once to get a feeling for the deserialization overhead
        /// </summary>
        /// <param name="data"></param>
        void TouchFlat(BookShelfFlat data, int nExpectedBooks, int optionalPayloadDataSize)
        {
            if (!VerifyAndTouch)
            {
                return;
            }

            string tmpTitle = null;
            int tmpId = 0;
            if (nExpectedBooks != data.BooksLength)
            {
                throw new InvalidOperationException($"Number of deserialized Books was {data.BooksLength} but expected {nExpectedBooks}. This Serializer seem to have lost data.");
            }

            for (int i = 0; i < data.BooksLength; i++)
            {
                var book = data.Books(i);
                tmpTitle = book.Value.Title;
                tmpId = book.Value.Id;
                if (tmpId != i + 1)
                {
                    throw new InvalidOperationException($"Book Id was {tmpId} but exepcted {i + 1}");
                }
                if (optionalPayloadDataSize > 0 && book.Value.BookDataLength != optionalPayloadDataSize)
                {
                    throw new InvalidOperationException($"BookData length was {book.Value.BookDataLength} but expected {optionalPayloadDataSize}");
                }
            }
        }

        ZeroFormatterBookShelf DataZeroFormatter(int nToCreate)
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
            if (!VerifyAndTouch) return;

            string tmpTitle = null;
            int tmpId = 0;
            for (int i = 0; i < data.Books.Count; i++)
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
            for (int i = 0; i < 10; i++)
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
                lret.Books.Add(new DateTime(DateTime.MinValue.Ticks + i, DateTimeKind.Utc), book);
            }
            return lret;
        }

    }
}
