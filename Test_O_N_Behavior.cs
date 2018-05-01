using SerializerTests.Serializers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests
{
    class Test_O_N_Behavior
    {
        List<ISerializeDeserializeTester> SerializersToTest;

        // capture first time init effects by executing serialize/deserialize with N=1 two times
        static int[] ObjectCountToTest = new int[]
        {
            1, 1, 10, 100, 500, 1000, 10*1000, 50*1000, 100*1000, 200*1000, 500*1000, 800*1000, 1000*1000
        };


        public Test_O_N_Behavior(List<ISerializeDeserializeTester> serializersToTest)
        {
            SerializersToTest = serializersToTest;
        }
  
        public  void TestSerialize(int nObjects = 1000 * 1000, int nRuns = 5)
        {
            Console.WriteLine(GetHeader(Format.Serialize));
            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;

                for (int i = 0; i < ObjectCountToTest.Length && ObjectCountToTest[i] <= nObjects; i++)
                {
                    NBooks = ObjectCountToTest[i];
                    var times = formatter.TestSerialize(nTimes: nRuns, nObjectsToCreate: NBooks);
                    Print(formatter, NBooks, times);

                    if (nObjects == 1)
                    {
                        break;
                    }

                }
            }
        }

        public  void TestDeserialize(int nObjects=1000*1000, int nRuns=5)
        {
            Console.WriteLine(GetHeader(Format.Deserialize));
            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;
                for (int i = 0; i < ObjectCountToTest.Length && ObjectCountToTest[i] <= nObjects; i++)
                {
                    NBooks = ObjectCountToTest[i];
                    var times = formatter.TestDeserialize(nTimes: nRuns, nObjectsToCreate: NBooks);
                    Print(formatter, NBooks, times);
                    if (nObjects == 1)
                    {
                        break;
                    }
                }
            }
        }

        public void TestCombined(int nObjects = 1000 * 1000, int nRuns = 5)
        {
            Console.WriteLine(GetHeader(Format.Combined));
            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;

                for (int i = 0; i < ObjectCountToTest.Length && ObjectCountToTest[i] <= nObjects; i++)
                {
                    NBooks = ObjectCountToTest[i];
                    var timesSerialize = formatter.TestSerialize(nTimes: nRuns, nObjectsToCreate: NBooks);
                    var timesDeserialize = formatter.TestDeserialize(nTimes: nRuns, nObjectsToCreate: NBooks);

                    Print(formatter, NBooks, timesSerialize, timesDeserialize);
                }
            }
        }

        private void Print(ISerializeDeserializeTester formatter, int NBooks, (double firstS, double averageS, long serializedSize) times)
        {
            string typeName = formatter.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2); // omit generic arg `1 of typename
            Console.WriteLine($"{typeName}<{formatter.GetType().GetGenericArguments()[0].Name}>\t{NBooks}\t{times.averageS:F3}\t{times.serializedSize}\t{formatter.FileVersion}\t{GetNetCoreVersion() ?? System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        }

        private void Print(ISerializeDeserializeTester formatter, int NBooks, (double firstS, double averageS, long serializedSize) timesSerialize, (double firstS, double averageS, long dataSize) timesDeserialize)
        {
            string typeName = formatter.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2); // omit generic arg `1 of typename
            Console.WriteLine($"{typeName}<{formatter.GetType().GetGenericArguments()[0].Name}>\t{NBooks}\t{timesSerialize.averageS:F3}\t{timesDeserialize.averageS:F3}\t{timesSerialize.serializedSize}\t{formatter.FileVersion}\t{GetNetCoreVersion() ?? System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        }

        enum Format
        {
            Deserialize,
            Serialize,
            Combined,
        }

        string GetHeader(Format format)
        {
            switch(format)
            {
                case Format.Combined:
                    return "Serializer\tObjects\t\"Time to serialize in s\"\t\"Time to deserialize in s\"\t\"Size in bytes\"\tFileVersion\tFramework";
                case Format.Deserialize:
                    return "Serializer\tObjects\t\"Time to deserialize in s\"\t\"Size in bytes\"\tFileVersion\tFramework";
                case Format.Serialize:
                    return "Serializer\tObjects\t\"Time to serialize in s\"\t\"Size in bytes\"\tFileVersion\tFramework";
                default:
                    throw new NotSupportedException($"Output format {format} is not supported.");
            }
        }

        /// <summary>
        /// Taken from https://github.com/dotnet/BenchmarkDotNet/issues/448
        /// </summary>
        /// <returns></returns>
        public static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return ".NET Core " + assemblyPath[netCoreAppIndex + 1];
            return null;
        }
    }
}
