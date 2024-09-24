
using System.Runtime.InteropServices;
// 主要考虑问题：
// 重要程度DESC排序：性能、内存、扩展性与兼容性(包括cpp)、异常检查处理
// 泛型(接收类型) + 工厂模式(创建对象)，工厂模式：public static UnsignedIntArray<T> Create<T>(T[] data) where T : struct, IConvertible
// 需要类型检查确保安全
// 可能需要策略模式(strategy)，即创建一个BaseGenerics DataType, 实现一些派生的具体位数的类，public class UInt16Strategy : IUnsignedIntStrategy<UInt16>
// 尤其策略模式，可能应该内置一些基础位运算的操作，或考虑调用其他算法的可能性？比如GPU，具体参考位运算加速
namespace DataBox
{
    // [StructLayout(LayoutKind.Explicit, Pack = 1)] // 手动管理内存布局，对齐长度为1(最紧凑)
    public struct GenericsDataBox<T> where T : struct
    {

        public  T Data { get; private set; }

        public ushort Length { get; set; } // data length, e.g. 8/10/14 and each others

        /**
         * Constructor method, init data box instance to contain DataType [T]
         * <param name="data">data value, include base type or array</param>
         * <param name="length">data type length, maybe array length?</param>
         */
        public GenericsDataBox(T data, ushort length)
        {
            // data max value and length match check
            Data = data;
            Length = length;
        }

        /**
         * Create New Instance of generics DataType [T]
         *
         * <returns>new instance as T</returns>
         */
        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}