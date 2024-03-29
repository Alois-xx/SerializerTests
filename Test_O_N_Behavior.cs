﻿using SerializerTests.Serializers;
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
            1, 1, 10, 100, 500, 1000, 10_000, 50_000, 100_000, 200_000, 500_000, 800_000, 1000_000
        };


        public Test_O_N_Behavior(List<ISerializeDeserializeTester> serializersToTest)
        {
            SerializersToTest = serializersToTest;
        }
  
        public  void TestSerialize(int[] nObjectsToDeSerialize, int nRuns = 5, string scenario=null, bool noHeader=false)
        {
            if (!noHeader)
            {
                Console.WriteLine(GetHeader(Format.Serialize));
            }
            nObjectsToDeSerialize = nObjectsToDeSerialize ?? ObjectCountToTest;
            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;
                int nMaxObjects = nObjectsToDeSerialize.Max();
                for (int i = 0; i < nObjectsToDeSerialize.Length; i++)
                {
                    NBooks = nObjectsToDeSerialize[i];
                    var times = formatter.TestSerialize(nTimes: nRuns, nObjectsToCreate: NBooks, nMaxObjects);
                    Print(formatter, NBooks, times, scenario);

                }
                formatter.ReleaseMemory();
            }
        }

        public  void TestDeserialize(int[] nObjectsToDeSerialize, int nRuns=5, string scenario=null, bool noHeader = false)
        {
            if (!noHeader)
            {
                Console.WriteLine(GetHeader(Format.Deserialize));
            }
            nObjectsToDeSerialize = nObjectsToDeSerialize ?? ObjectCountToTest;

            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;
                int maxObjects = nObjectsToDeSerialize.Max();
                for (int i = 0; i < nObjectsToDeSerialize.Length; i++)
                {
                    NBooks = nObjectsToDeSerialize[i];
                    var times = formatter.TestDeserialize(nTimes: nRuns, nObjectsToCreate: NBooks, maxObjects);
                    Print(formatter, NBooks, times, scenario);
                }
                formatter.ReleaseMemory();
            }
        }

        public void TestCombined(int[] nObjectsToDeSerialize, int nRuns = 5, string scenario=null, bool noHeader=false)
        {
            if (!noHeader)
            {
                Console.WriteLine(GetHeader(Format.Combined));
            }
            nObjectsToDeSerialize = nObjectsToDeSerialize ?? ObjectCountToTest;

            foreach (var formatter in SerializersToTest)
            {
                int NBooks = 0;
                int maxObjects = nObjectsToDeSerialize.Max();

                for (int i = 0; i < nObjectsToDeSerialize.Length; i++)
                {
                    NBooks = nObjectsToDeSerialize[i];
                    var timesSerialize = formatter.TestSerialize(nTimes: nRuns, nObjectsToCreate: NBooks, nMaxObjects: maxObjects);
                    var timesDeserialize = formatter.TestDeserialize(nTimes: nRuns, nObjectsToCreate: NBooks, nMaxObjects: maxObjects);

                    Print(formatter, NBooks, timesSerialize, timesDeserialize, scenario);
                }

                formatter.ReleaseMemory();
            }
        }

        private void Print(ISerializeDeserializeTester formatter, int NBooks, (double firstS, double averageS, long serializedSize) times, string scenario)
        {
            string typeName = formatter.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2); // omit generic arg `1 of typename
            Console.WriteLine($"{typeName}\t{NBooks}\t{times.averageS:F4}\t0\t{times.serializedSize}\t{formatter.FileVersion}\t{GetNetCoreVersion() ?? System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\t{GetSerializerDetails(formatter)}\t{scenario}");
        }

        private void Print(ISerializeDeserializeTester formatter, int NBooks, (double firstS, double averageS, long serializedSize) timesSerialize, (double firstS, double averageS, long dataSize) timesDeserialize, string scenario)
        {
            string typeName = formatter.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2); // omit generic arg `1 of typename
            Console.WriteLine($"{typeName}\t{NBooks}\t{timesSerialize.averageS:F4}\t{timesDeserialize.averageS:F3}\t{timesSerialize.serializedSize}\t{formatter.FileVersion}\t{GetNetCoreVersion() ?? System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}\t{GetSerializerDetails(formatter)}\t{scenario}");
        }

        /// <summary>
        /// Print Serializer details like supported data format and versioning support
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns>tab separated fields for output in CSV file</returns>
        string GetSerializerDetails(ISerializeDeserializeTester formatter)
        {
            var description = (SerializerTypeAttribute)formatter.GetType().GetCustomAttribute(typeof(SerializerTypeAttribute));
            if( description == null)
            {
                throw new MissingMemberException($"The serializer {formatter.GetType().Name} has no SerializerTypeAttribute! Please add missing metadata!");
            }
            var str = description.ProjectHomeUrl;
            string dataFormat = description.SerializerTypeDescription.HasFlag(SerializerTypes.Binary) ? "Binary" : "Text";
            string subDataFormat = description.SerializerTypeDescription.HasFlag(SerializerTypes.Json) ? "Json" :
                                  (description.SerializerTypeDescription.HasFlag(SerializerTypes.Xml) ? "Xml" : "");
            string supportsVersioning = description.SerializerTypeDescription.HasFlag(SerializerTypes.SupportsVersioning) ? "Yes" : "No";
            return $"{description.ProjectHomeUrl}\t{dataFormat}\t{subDataFormat}\t{supportsVersioning}";
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
                    return "Serializer\tObjects\t\"Time to serialize in s\"\t\"Time to deserialize in s\"\t\"Size in bytes\"\tFileVersion\tFramework\tProjectHome\tDataFormat\tFormatDetails\tSupports Versioning\tScenario";
                case Format.Deserialize:
                    return "Serializer\tObjects\t\"Time to deserialize in s\"\tNotUsed\t\"Size in bytes\"\tFileVersion\tFramework\tProjectHome\tDataFormat\tFormatDetails\tSupports Versioning\tScenario";
                case Format.Serialize:
                    return "Serializer\tObjects\t\"Time to serialize in s\"\tNotUsed\t\"Size in bytes\"\tFileVersion\tFramework\tProjectHome\tDataFormat\tFormatDetails\tSupports Versioning\tScenario";
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
