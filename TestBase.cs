using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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

    public abstract class TestBase<T, F> : ISerializeDeserializeTester where F : class
    {
        string FileBaseName = "Serialized_";

        protected Func<int, T> CreateNTestData;

        /// <summary>
        /// If true and serializer supports it Reference tracking is enabled for this test
        /// </summary>
        protected bool RefTracking;

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
                if (myFormatter == null)
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
                if (DefaultTestData == null || ObjectsCreated != ObjectsToCreate)
                {
                    DefaultTestData = CreateNTestData(ObjectsToCreate);
                    ObjectsCreated = ObjectsToCreate;
                }
                return DefaultTestData;
            }
        }

        // The thread and events exist only to get a context switch event when a test starts
        // so that our TestDuration thread starts waiting for an event 
        // This way we can follow the test duration easily in a profiler without any extra ETW events and dependencies
        static ManualResetEvent TestTriggerEvent = new ManualResetEvent(false);
        static ManualResetEvent DeSerializeEvent = new ManualResetEvent(false);
        static volatile bool IsSerialize = false;

        Thread DurationThread = null;
        bool bThreadExit = false;

        void StartDurationThread()
        {
            StopDurationThread();
            DurationThread = new Thread(TestDurationThread) { IsBackground = true };
            bThreadExit = false;
            DurationThread.Start(this);
        }

        void StopDurationThread()
        {
            if( DurationThread != null )
            {
                bThreadExit = true;
                TestTriggerEvent.Set();
                DeSerializeEvent.Set();
                DurationThread.Join();
            }
        }


        /// <summary>
        /// Create Test
        /// </summary>
        /// <param name="testData">Delegate to create test data to serialize</param>
        /// <param name="data">Data toucher after deserialization</param>
        /// <param name="refTracking">If true serializer is instantiated with Reference Tracking</param>
        protected TestBase(Func<int, T> testData, Action<T> data, bool refTracking=false)
        {
            RefTracking = refTracking;
            CreateNTestData = testData;
            TouchData = data;
        }

        protected Action<T> TouchData;

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected abstract void Serialize(T obj, Stream stream);
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected abstract T Deserialize(Stream stream);

        protected Func<MemoryStream, T> CustomDeserialize;
        protected Action<MemoryStream> CustomSerialize;

        MemoryStream myStream;

        protected MemoryStream GetMemoryStream()
        {
            if (myStream == null)
            {
                myStream = new MemoryStream();
            }
            myStream.Position = 0;
            return myStream;
        }

        List<double> Test(int n, Action acc)
        {
            List<double> times = new List<double>();
            StartDurationThread();
            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < n; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                ManualResetEvent ev = GetEvent(!acc.Method.Name.Contains("Deserialize"));
                sw.Restart();
                acc();
                sw.Stop();
                ev.Set(); // end waiting to get a nice context switch how long a test did really take in elapsed time
                times.Add(sw.Elapsed.TotalSeconds);
            }

            StopDurationThread();
            return times;
        }

        public (double firstS, double averageS, long serializedSize) TestSerialize(int nTimes, int nObjectsToCreate)
        {
            ObjectsToCreate = nObjectsToCreate;
            GetMemoryStream().Capacity = 100 * 1000 * 1000; // Set memory stream to largest serialized payload to prevent resizes during test
            var tmp = this.TestData;                        // Create testdata before test starts
            StartDurationThread();
            var times = Test(nTimes, () =>
            {
                var dataStream = GetMemoryStream();
                TestSerializeOnly(dataStream);
            });
            StopDurationThread();
            SaveMemoryStreamToDisk(GetMemoryStream(), nObjectsToCreate);

            return CalcTime(times, GetMemoryStream().Length);
        }

        /// <summary>
        /// Make profiling easier by putting the actual to be measured code into a not inlined method
        /// </summary>
        /// <param name="dataStream"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        void TestSerializeOnly(MemoryStream dataStream)
        {
            if (CustomSerialize == null)
            {
                Serialize(TestData, dataStream);
            }
            else
            {
                CustomSerialize(dataStream);
            }
        }

        /// <summary>
        /// To really test inside a fresh process with first time init effects we read the serialized data from a previous
        /// serialize test run from disk into memory and then use this data as input.
        /// </summary>
        /// <param name="nTimes">Let test run n times.</param>
        /// <param name="nObjectsToCreate">Select from a previous serialize test run the file with the objects created.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public (double firstS, double averageS, long dataSize) TestDeserialize(int nTimes, int nObjectsToCreate)
        {
            myStream = ReadMemoryStreamFromDisk(nObjectsToCreate);

            long size = myStream.Length;
            var times = Test(nTimes, () =>
            {
                var dataStream = GetMemoryStream();
                TestDeserializeOnlyAndTouch(dataStream, out T deserialized);
            });

            return CalcTime(times, size);
        }

        /// <summary>
        /// Make profiling easier by putting the actual to be measured code into a not inlined method
        /// </summary>
        /// <param name="dataStream"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void TestDeserializeOnlyAndTouch(MemoryStream dataStream, out T deserialized)
        {

            if (CustomDeserialize == null)
            {
                deserialized = Deserialize(dataStream);
            }
            else
            {
                // call deserialize with custom overloads to check out e.g. XmlDictionaryWriter 
                deserialized = CustomDeserialize(dataStream);
            }

            TouchDataNoInline(ref deserialized);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void TouchDataNoInline(ref T deserialized)
        {
            TouchData?.Invoke(deserialized); // touch data to test delayed deserialization
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
            Type[] genericArgs = this.GetType().GetGenericArguments();
            typeName += "_" + (genericArgs.Length > 0 ? genericArgs[0].Name : this.GetType().Name);
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

        ManualResetEvent GetEvent(bool isSerialize)
        {
            IsSerialize = isSerialize;
            TestTriggerEvent.Set();
            return DeSerializeEvent;
        }

        static void TestDurationThread(object o)
        {
            TestBase<T, F> testbase = (TestBase<T, F>)o;

            while (true)
            {
                if (testbase.bThreadExit)
                {
                    break;
                }
                TestTriggerEvent.WaitOne();
                if(testbase.bThreadExit)
                {
                    break;
                }
                if (IsSerialize)
                {
                    SerializeDuration();
                }
                else
                {
                    DeserializeDuration();
                }
                // Set events to nonsignaled so our thread waits again until first in TriggerEvent
                // and then for the De/Serialize event which is fired when the test has completed
                DeSerializeEvent.Reset();
                TestTriggerEvent.Reset();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void DeserializeDuration()
        {
            DeSerializeEvent.WaitOne();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void SerializeDuration()
        {
            DeSerializeEvent.WaitOne();
        }
    }
}
