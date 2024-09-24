using System.Runtime.InteropServices;

namespace DataBox;

/// <summary>
/// 数据包装类，根据传入参数的类型选择合适的隐式类型转换。仅能存储正数值
/// <br/>
/// 支持1~24bit的数据，在高兼容性前提下尽可能提升数据创建和获取效率，其次节省内存空间。
/// <br/>
/// 如果作为循环键值使可能性能降低，请考虑重载GetHashCode方法。如：设置值后预先缓存HashCode, 而不是每次在获取时生成
/// </summary>
/// <author>IceblueSakura</author>
/// <date>2024/09/24</date>
/// <version>0.1.SNAPSHOT</version>
[StructLayout(LayoutKind.Explicit)] // 手动管理内存布局，如果内存消耗太大，请添加`Pack = 1`使内存布局对齐长度为1(最紧凑)
public struct BitData : IEquatable<BitData>
{
    [FieldOffset(0)] // 0~3 byte, 24bit
    private uint _value;  // 会自动分配为 0

    [FieldOffset(3)] // 3~4 byte
    private byte _bitLength;  // 会自动分配为 0

    /// <summary>
    /// 初始化 <see cref="BitData"/> 结构的新实例，指定值和位长度。
    /// </summary>
    /// <param name="value">要存储的值。</param>
    /// <param name="bitLength">用于存储值的位数（1-24）。</param>
    public BitData(uint value, byte bitLength)
    {
        if (bitLength > 24 || bitLength < 1)
            throw new ArgumentOutOfRangeException(nameof(bitLength), bitLength, "位长度必须在1到24之间。");

        if (value >= (1u << bitLength))
            throw new ArgumentOutOfRangeException(nameof(value), value, $"值超出了指定位长度（{bitLength} 位）的允许范围。");

        _value = value & ((1u << bitLength) - 1);  // 对值进行掩码，防止奇怪的溢出错误
        _bitLength = bitLength;
    }
    
// 以下的构造函数没有对赋值进行掩码(按位并操作)，因为我认为csharp编译期已经能检查出这种错误了
    /// <summary>
    /// 使用 <see cref="byte"/> 类型的值初始化 <see cref="BitData"/> 结构的新实例。
    /// </summary>
    /// <param name="value">要存储的 <see cref="byte"/> 值。</param>
    public BitData(byte value)
    {
        _value = value;
        _bitLength = 8;
    }

    /// <summary>
    /// 使用 <see cref="ushort"/> 类型的值初始化 <see cref="BitData"/> 结构的新实例。
    /// </summary>
    /// <param name="value">要存储的 <see cref="ushort"/> 值。</param>
    public BitData(ushort value)
    {
        _value = value;
        _bitLength = 16;
    }

    /// <summary>
    /// 使用 <see cref="short"/> 类型的值初始化 <see cref="BitData"/> 结构的新实例。
    /// </summary>
    /// <param name="value">要存储的 <see cref="short"/> 值。</param>
    /// <param name="check">是否检查值的非负数情况，默认检查</param>
    public BitData(short value, bool check = true)
    {
        if (check && value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "BitData值非正数！请检查赋值或置check参数为false。");
        }
        _value = unchecked((uint)value);
        _bitLength = 16;
    }

    /// <summary>
    /// 使用 <see cref="int"/> 类型的值初始化 <see cref="BitData"/> 结构的新实例。
    /// </summary>
    /// <param name="value">要存储的 <see cref="int"/> 值。</param>
    public BitData(int value)
    {
        if (value < 0 || value >= (1 << 24))
            throw new ArgumentOutOfRangeException(nameof(value), value, "值必须在0到16777215（24位）之间。");
        _value = (uint)value;
        _bitLength = 24;
    }

    /// <summary>
    /// 获取或设置此 <see cref="BitData"/> 中存储的值。
    /// </summary>
    public uint Value
    {
        get => _value & ((1u << _bitLength) - 1);
        set
        {
            if (value >= (1u << _bitLength))  // _bitLength是允许的位数，偏移后即为这个位数支持的最大值，如8bit => 1u<<8 => 1 0000 0000b = 256
                throw new ArgumentOutOfRangeException(nameof(value), value, "值超出了当前位长度的允许范围。");
            
            // _value & ~((1u << _bitLength) - 1)：这个操作将 _value 的低 _bitLength 位清零，保留高位部分。~ 是按位取反操作，生成的掩码会清除低 _bitLength 位。
            // value & ((1u << _bitLength) - 1)：这部分代码通过掩码将 value 的低 _bitLength 位提取出来，忽略高位。
            // |：最后，使用按位或运算 (|) 将保留的高位与提取出来的低位合并，更新 _value。
            _value = (_value & ~((1u << _bitLength) - 1)) | (value & ((1u << _bitLength) - 1));
            
        }
    }

    /// <summary>
    /// 获取或设置用于存储值的位数。
    /// </summary>
    public byte BitLength
    {
        get => _bitLength;
        // set
        // {
        //     if (value > 24 || value < 1)
        //         throw new ArgumentOutOfRangeException(nameof(value), value, "位长度必须在1到24之间。");
        //     if (_value >= (1u << value))  
        //         throw new ArgumentOutOfRangeException(nameof(value), value, "当前值超出了新位长度的允许范围");
        //     _bitLength = value;
        // }
    }

    /// <summary>
    /// 隐式将 <see cref="BitData"/> 转换为 <see cref="uint"/>。
    /// </summary>
    /// <param name="data">要转换的 <see cref="BitData"/>。</param>
    public static implicit operator uint(BitData data) => data.Value;

    /// <summary>
    /// 隐式将 <see cref="byte"/> 转换为 <see cref="BitData"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="byte"/> 值。</param>
    public static implicit operator BitData(byte value) => new BitData(value);

    /// <summary>
    /// 隐式将 <see cref="ushort"/> 转换为 <see cref="BitData"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="ushort"/> 值。</param>
    public static implicit operator BitData(ushort value) => new BitData(value);

    /// <summary>
    /// 隐式将 <see cref="short"/> 转换为 <see cref="BitData"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="short"/> 值。</param>
    public static implicit operator BitData(short value) => new BitData(value);

    /// <summary>
    /// 显式将 <see cref="uint"/> 转换为 <see cref="BitData"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="uint"/> 值。</param>
    public static explicit operator BitData(uint value) => new BitData(value, 24);

    /// <summary>
    /// 显式将 <see cref="int"/> 转换为 <see cref="BitData"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="int"/> 值。</param>
    public static explicit operator BitData(int value) => new BitData(value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is BitData data && Equals(data);

    /// <inheritdoc/>
    public bool Equals(BitData other) => _value == other._value && _bitLength == other._bitLength;

    /// <inheritdoc/>
    /// <br/>
    /// 如果作为循环键值使可能性能降低，请考虑重载这个方法。如：设置值后预先缓存hashCode, 而不是每次生成
    public override int GetHashCode() => HashCode.Combine(_value, _bitLength);

    /// <summary>
    /// 返回表示当前 <see cref="BitData"/> 的字符串。
    /// </summary>
    /// <returns>表示当前 <see cref="BitData"/> 的字符串。</returns>
    public override string ToString() => $"Value: {Value}, BitLength: {BitLength}";

    /// <summary>
    /// 确定两个指定的 <see cref="BitData"/> 实例是否相等。
    /// </summary>
    /// <param name="left">要比较的第一个 <see cref="BitData"/>。</param>
    /// <param name="right">要比较的第二个 <see cref="BitData"/>。</param>
    /// <returns>如果两个 <see cref="BitData"/> 实例相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public static bool operator ==(BitData left, BitData right) => left.Equals(right);

    /// <summary>
    /// 确定两个指定的 <see cref="BitData"/> 实例是否不相等。
    /// </summary>
    /// <param name="left">要比较的第一个 <see cref="BitData"/>。</param>
    /// <param name="right">要比较的第二个 <see cref="BitData"/>。</param>
    /// <returns>如果两个 <see cref="BitData"/> 实例不相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public static bool operator !=(BitData left, BitData right) => !left.Equals(right);
}