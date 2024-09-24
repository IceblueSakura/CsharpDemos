using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DataBox
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            BitPackerTest();
        }

        private static void BitPackerTest()
        {
            const int bitLength = 12; // 每个数据项12位
            const int count = 5; // 存储5个数据项
            var packer = new BitPacker(bitLength, count);

            // 设置数据
            packer.SetValue(0, 0xABC);
            packer.SetValue(1, 0x123);
            packer.SetValue(2, 0xFFF);
            packer.SetValue(3, 0x0F0);
            packer.SetValue(4, 0x0FF);

            // 获取数据
            for (var i = 0; i < count; i++)
            {
                var value = packer.GetValue(i);
                Console.WriteLine($"数据项 {i} 的值：0x{value:X}");
            }

            // 获取原始字节数据
            var rawData = packer.GetRawData();
            Console.WriteLine("原始字节数据： " + BitConverter.ToString(rawData));

            // 从原始数据创建新的BitPacker
            var packerFromRaw = BitPacker.FromRawData(rawData, bitLength, count);
            for (var i = 0; i < count; i++)
            {
                var value = packerFromRaw.GetValue(i);
                Console.WriteLine($"从原始数据创建的BitPacker - 数据项 {i} 的值：0x{value:X}");
            }

            var stopwatch = new Stopwatch();
            Console.WriteLine("进行操作耗时分析");
            BitPacker tmp = null;
            stopwatch.Start();
            for (var i = 0; i < 100000000; i++)
            {
                tmp = BitPacker.FromRawData(rawData, bitLength, count); // 加载之前有5组12bit数据的实例
                tmp.GetValue(3); // 获取第三组的数据值
            }

            stopwatch.Stop();
            Console.WriteLine(
                $"一万次操作耗时: {stopwatch.ElapsedMilliseconds} 毫秒"); // 存储有效数据75,000 byte，实际占用320,000 byte，如果纯用ushort存需要160,000 byte
        }
    }
}