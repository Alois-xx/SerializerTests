using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SerializerTests
{
    /// <summary>
    /// Common testing interface to be able to put serializers with different generic type args into a common collection.
    /// </summary>
    public interface ISerializeDeserializeTester
    {
        (double firstS, double averageS, long serializedSize) TestSerialize(int nTimes, int nObjectsToCreate);
        (double firstS, double averageS, long dataSize) TestDeserialize(int nTimes, int nObjectsToCreate);

        string FileVersion { get;  }
    }

    public abstract class TestBase<T,F> : ISerializeDeserializeTester where T : class where F: class 
    {
        string FileBaseName = "Serialized_";

        protected Func<int,T> CreateNTestData;

        int ObjectsToCreate
        {
            get;
            set;
        }

        protected Func<F> FormatterFactory;

        F myFormatter;

        protected F Formatter
        {
            get
            {
                if( myFormatter == null)
                {
                    myFormatter = FormatterFactory();
                }

                return myFormatter;
            }
        }

        int ObjectsCreated = 0;

        T DefaultTestData = default(T);

        protected T TestData
        {
            get
            {
                if(DefaultTestData == null || ObjectsCreated != ObjectsToCreate)
                {
                    DefaultTestData = CreateNTestData(ObjectsToCreate);
                    ObjectsCreated = ObjectsToCreate;
                }
                return DefaultTestData;
            }
        }

        protected abstract void Serialize(T obj, Stream stream);
        protected abstract T Deserialize(Stream stream);

        protected Func<MemoryStream,T> CustomDeserialize;
        protected Action<MemoryStream> CustomSerialize;

        MemoryStream myStream;
        protected MemoryStream GetMemoryStream()
        {
            if( myStream == null)
            {
                myStream = new MemoryStream();
            }

            myStream.Position = 0;

            return myStream;
        }


        List<double> Test(int n, Action acc)
        {
            List<double> times = new List<double>();
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < n; i++)
            {
                sw.Restart();
                acc();
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
            }

            return times;
        }

        public (double firstS,double averageS, long serializedSize) TestSerialize(int nTimes, int nObjectsToCreate)
        {
            ObjectsToCreate = nObjectsToCreate;
            var times = Test(nTimes, () =>
            {
                if (CustomSerialize == null)
                {
                    Serialize(TestData, GetMemoryStream());
                }
                else
                {
                    CustomSerialize(GetMemoryStream());
                }
            });

            SaveMemoryStreamToDisk(GetMemoryStream(), nObjectsToCreate);

            return CalcTime(times, GetMemoryStream().Length);
        }

        /// <summary>
        /// To really test inside a fresh process with first time init effects we read the serialized data from a previous
        /// serialize test run from disk into memory and then use this data as input.
        /// </summary>
        /// <param name="nTimes">Let test run n times.</param>
        /// <param name="nObjectsToCreate">Select from a previous serialize test run the file with the objects created.</param>
        /// <returns></returns>
        public (double firstS, double averageS, long dataSize) TestDeserialize(int nTimes, int nObjectsToCreate)
        {
            myStream = ReadMemoryStreamFromDisk(nObjectsToCreate);

            long size = myStream.Length;
            var times = Test(nTimes, () =>
            {
                if (CustomDeserialize == null)
                {
                    var deserialized = Deserialize(GetMemoryStream());
                }
                else
                {
                    // call deserialize with custom overloads to check out e.g. XmlDictionaryWriter 
                    CustomDeserialize(GetMemoryStream());
                }
            });

            return CalcTime(times, size);
        }

 

        (double firstS, double averageS, long serializedSize) CalcTime(List<double> timesInS, long serializedSize)
        {
            // first measured time includes code generation and JITing time of the serializer/deserializer.
            // All following measurements gives us the throughput numbers if startup is of no concern.
            // Depending on your scenario/serializer you need to pay attention to first time init effects and/or throughput.
            // For many serializers you can compile the generated code into an assembly to decrease the first time init effects to nearly zero.
            // see sgen.exe for XmlSerializer or RuntimeTypeModel.Compile for protbuf-net.
            return (timesInS[0], timesInS.Count > 1 ? timesInS.Skip(1).Average() : timesInS[0], serializedSize);
        }

        string GetInputOutputFileName(int nObjectsCreated)
        {
            string typeName = this.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2);
            typeName += "_" + this.GetType().GetGenericArguments()[0].Name;
            string outputFileName = $"{FileBaseName}{typeName}_{nObjectsCreated}.bin";
            return outputFileName;
        }

        void SaveMemoryStreamToDisk(MemoryStream stream, int objectsCreated)
        {
            string outFile = GetInputOutputFileName(objectsCreated);
            File.WriteAllBytes(outFile, stream.ToArray());
        }

        MemoryStream ReadMemoryStreamFromDisk(int nObjectsCreatedAndSaved)
        {
            string file = GetInputOutputFileName(nObjectsCreatedAndSaved);
            byte[] bytes = File.ReadAllBytes(file);
            var stream = GetMemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }

        public string FileVersion
        {
            get => FileVersionInfo.GetVersionInfo(typeof(F).Assembly.Location).FileVersion;
        }
    }
}
