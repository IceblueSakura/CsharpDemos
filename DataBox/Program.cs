using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DataBox
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // BitDataTest();
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
        }

        private static void BitDataTest()
        {
            Console.WriteLine($"BitData 结构体的大小：{BitData.Size} 字节"); // 应输出5字节

            // 示例1：8位数据
            BitData data8Bit = new BitData(0xAA, 8); // 0xAA = 170
            Console.WriteLine($"8位数据的值（byte）：{data8Bit.Get8()}");

            // 示例2：16位数据
            BitData data16Bit = new BitData(0xABCD, 16); // 0xABCD = 43981
            Console.WriteLine($"16位数据的值（ushort）：{data16Bit.GetData<ushort>()}");

            // 示例3：20位数据
            BitData data20Bit = new BitData(0xABCDE, 20); // 0xABCDE = 703710
            Console.WriteLine($"20位数据的值（uint）：{data20Bit.GetData<uint>()}");

            // 示例4：隐式转换
            BitData fromByte = (byte)0x7F; // 127
            byte byteValue = fromByte;
            Console.WriteLine($"隐式转换后的byte值：{byteValue}");

            BitData fromUshort = (ushort)0x1234; // 4660
            ushort ushortValue = fromUshort;
            Console.WriteLine($"隐式转换后的ushort值：{ushortValue}");

            BitData fromUint = 0xABCDE; // 703710
            uint uintValue = fromUint;
            Console.WriteLine($"隐式转换后的uint值：{uintValue}");
        }
    }
}