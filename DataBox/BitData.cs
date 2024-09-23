using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataBox
{
    /// <summary>
    /// BitData结构，用于封装8到32位的数据
    /// 使用结构体，在栈中寻址快，值类型不涉及垃圾回收，生命周期结束销毁
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)] // 手动管理内存布局，对齐长度为1(最紧凑)
    public struct BitData
    {
        [FieldOffset(0)] // 使用结构体开头的16bit
        private readonly ushort _data16; // 没使用byte[] 因为数组时引用类型，频繁分配会触发GC

        [FieldOffset(0)] private readonly uint _data32; // 使用结构体开头的32bit，会覆盖_data16(类似union)

        [FieldOffset(4)] // 偏移4byte空间
        private readonly byte _bitLength;

        /// <summary>
        /// 构造函数，接受数据和位长度，在实际使用中使用操作符重载的隐式类型转换就可以
        /// </summary>
        /// <param name="data">实际数据值</param>
        /// <param name="bitLength">数据的有效位长度（8到32位）</param>
        public BitData(uint data, byte bitLength)
        {
            if (bitLength < 8 || bitLength > 20)
                throw new ArgumentOutOfRangeException(nameof(bitLength), "位长度必须在8到32位之间。");

            _bitLength = bitLength;

            if (bitLength <= 16)
            {
                _data16 = (ushort)(data & 0xFFFF); // 仅保留前16位
                _data32 = 0; // 清零余下位
            }
            else
            {
                _data32 = data; // 保留前全部32位
                _data16 = 0; // 清零，防止传入32bit低位内存值是0的情况
            }
        }

        public byte Get8()
        {
            return (byte)(_data16 & 0xFF); // 取8位
        }

        public ushort Get16()
        {
            return (ushort)(_data16 & 0xFFFF); // 取16位
        }

        public uint Get32()
        {
            return (uint)(_data32 & 0xFFFFFFFF); // 取32位
        }


        /// <summary>
        /// 获取存储的数据，并转换为指定类型，但涉及对象拆装箱，推荐使用implicit operator
        /// </summary>
        /// <typeparam name="T">目标类型（byte, ushort, uint）</typeparam>
        /// <returns>转换后的数据值</returns>
        public T GetData<T>() where T : struct // 约束T必须时值类型
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(byte) =>
                    _bitLength > 8
                        ? throw new InvalidOperationException("位长度超过8位，无法转换为byte。")
                        : (T)(object)Get8(),

                _ when typeof(T) == typeof(ushort) =>
                    _bitLength > 16
                        ? throw new InvalidOperationException("位长度超过16位，无法转换为ushort。")
                        : (T)(object)Get16(),

                _ when typeof(T) == typeof(uint) =>
                    _bitLength > 32
                        ? throw new InvalidOperationException("位长度超过32位，无法转换为uint。")
                        : (T)(object)Get32(),
                
                _ => throw new NotSupportedException($"类型 {typeof(T)} 不支持转换。")
            };
        }

        /// <summary>
        /// 获取数据的位长度
        /// </summary>
        public byte BitLength => _bitLength;

        /// <summary>
        /// 隐式转换，从BitData到基础类型（byte, ushort, uint）
        /// </summary>
        /// <param name="bitData">BitData实例</param>
        public static implicit operator byte(BitData bitData)
        {
            return bitData.Get8();
        }

        public static implicit operator ushort(BitData bitData)
        {
            return bitData.Get16();
        }

        public static implicit operator uint(BitData bitData)
        {
            return bitData.Get32();
        }

        /// <summary>
        /// 隐式转换，从基础类型（byte, ushort, uint）到BitData
        /// 默认位长度为数据类型的位数
        /// </summary>
        /// <param name="value">基础类型值</param>
        public static implicit operator BitData(byte value)
        {
            return new BitData(value, 8);
        }

        public static implicit operator BitData(ushort value)
        {
            return new BitData(value, 16);
        }

        public static implicit operator BitData(uint value)
        {
            // 默认使用32位，如果实际位数更小可以通过另一个构造函数实现
            return new BitData(value, 32);
        }
        
        /// <summary>
        /// 获取BitData结构体的大小（字节数），测试方法
        /// </summary>
        public static int Size
        {
            get
            {
                return Marshal.SizeOf(typeof(BitData));
            }
        }
    }
}