using DataBox;

namespace UnitTEst;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test] // 验证使用构造函数赋值数否正确
    public void Constructor_WithValidValueAndBitLength_ShouldInitializeCorrectly()
    {
        var data = new BitData(15, 4);
        Assert.AreEqual(15u, data.Value);
        Assert.AreEqual(4, data.BitLength);
    }

    [Test] // 验证使用构造函数，使用溢出值是否抛出异常
    public void Constructor_WithValueExceedingBitLength_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitData(16, 4));
    }

    [Test] // 验证使用构造函数，使用过小位数是否抛出异常
    public void Constructor_WithInvalidBitLength_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitData(1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitData(1, 25));
    }

    [Test] // 验证使用构造函数，使用byte赋值是否正常
    public void Constructor_WithByte_ShouldSetBitLengthTo8()
    {
        byte value = 200;
        var data = new BitData(value);
        Assert.AreEqual((byte)value, data.Value);
        Assert.AreEqual(8, data.BitLength);
    }


    [Test]  // 验证使用构造函数，使用ushort赋值是否正常
    public void Constructor_WithUShort_ShouldSetBitLengthTo16()
    {
        ushort value = 60000;
        var data = new BitData(value);
        Assert.AreEqual((ushort)value, data.Value);
        Assert.AreEqual(16, data.BitLength);
    }

    [Test]  // 验证使用构造函数，使用short赋值是否正常
    public void Constructor_WithShort_ShouldSetBitLengthTo16()
    {
        short value = 30000;
        var data = new BitData(value);
        Assert.AreEqual((short)value, data.Value);
        Assert.AreEqual(16, data.BitLength);
    }
    
    [Test]  // 验证使用构造函数，使用short赋值是否抛出异常
    public void Constructor_WithLowerShort_ShouldThrowException()
    {
        short value = -30000;
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitData(value));
    }

    [Test]   // 验证使用构造函数，使用24bit数据赋值是否正常
    public void Constructor_WithIntWithin24Bits_ShouldInitializeCorrectly()
    {
        int value = 5000000;
        var data = new BitData(value);
        Assert.AreEqual((uint)value, data.Value);
        Assert.AreEqual(24, data.BitLength);
    }

    [Test]   // 验证使用构造函数，使用超过24bit数据是否抛出异常
    public void Constructor_WithIntExceeding24Bits_ShouldThrowException()
    {
        int value = 20000000;
        Assert.Throws<ArgumentOutOfRangeException>(() => new BitData(value));
    }

    [Test]  // 验证重新赋值是否正常，error
    public void ValueProperty_SetValidValue_ShouldUpdateCorrectly()
    { 
        var data = new BitData(15, 4);
        Console.WriteLine(data.ToString());
        data.Value = (byte)9;
        Console.WriteLine(data.ToString());
        Assert.AreEqual(9, data.Value);
    }

    [Test]   // 验证重新赋值，但新值超过原位数是否抛出异常
    public void ValueProperty_SetInvalidValue_ShouldThrowException()
    {
        var data = new BitData(15, 4);
        Assert.Throws<ArgumentOutOfRangeException>(() => data.Value = 16);
    }

    [Test]  // 验证隐式类型转换获取uint值是否正常
    public void ImplicitConversion_ToUInt_ShouldReturnValue()
    {
        var data = new BitData(100, 8);
        uint value = data;
        Assert.AreEqual(100u, value);
    }

    [Test]  // 验证byte赋值隐式类型转换是否正常
    public void ImplicitConversion_FromByte_ShouldInitializeCorrectly()
    {
        byte value = 50;
        BitData data = value;
        Assert.AreEqual(50u, data.Value);
        Assert.AreEqual(8, data.BitLength);
    }


    [Test]  // 验证ushort赋值隐式类型转换是否正常
    public void ImplicitConversion_FromUShort_ShouldInitializeCorrectly()
    {
        ushort value = 40000;
        BitData data = value;
        Assert.AreEqual(40000u, data.Value);
        Assert.AreEqual(16, data.BitLength);
    }

    [Test]  // 验证short赋值隐式类型转换是否正常
    public void ImplicitConversion_FromShort_ShouldInitializeCorrectly()
    {
        short value = -10000;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            BitData data = value;
        });
        
    }

    [Test]  // 验证uint赋值隐式类型转换是否正常
    public void ExplicitConversion_FromUInt_ShouldInitializeCorrectly()
    {
        uint value = 1000000;
        BitData data = (BitData)value;
        Assert.AreEqual(1000000u, data.Value);
        Assert.AreEqual(24, data.BitLength);
    }

    [Test]  // 验证uint赋值隐式类型转换，超出24bit的值是否抛出异常
    public void ExplicitConversion_FromUIntExceeding24Bits_ShouldThrowException()
    {
        uint value = 20000000u;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            BitData data = (BitData)value;
        });
    }

    [Test]   // 验证int赋值隐式类型转换是否正常
    public void ExplicitConversion_FromInt_ShouldInitializeCorrectly()
    {
        int value = 5000000;
        BitData data = (BitData)value;
        Assert.AreEqual(5000000u, data.Value);
        Assert.AreEqual(24, data.BitLength);
    }

    [Test]   // 验证int赋值隐式类型转换，超出24bit的值是否抛出异常
    public void ExplicitConversion_FromIntExceeding24Bits_ShouldThrowException()
    {
        int value = 20000000;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            BitData data = (BitData)value;
        });
    }

    [Test]  // 验证数据包装类之间，数据位数相同重载==和!=是否正常
    public void EqualityOperators_WithEqualValues_ShouldReturnTrue()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(15, 4);
        Assert.IsTrue(data1 == data2);
        Assert.IsFalse(data1 != data2);
    }

    [Test]  // 验证数据包装类之间，数据位数相同重载==和!=是否正常
    public void EqualityOperators_WithDifferentValues_ShouldReturnFalse()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(9, 4);
        Assert.IsFalse(data1 == data2);
        Assert.IsTrue(data1 != data2);
    }

    [Test]  // 验证重载的Equals语句是否正常
    public void EqualsMethod_WithEqualValues_ShouldReturnTrue()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(15, 4);
        Assert.IsTrue(data1.Equals(data2));
    }

    [Test]  // 验证重载的Equals语句是否正常
    public void EqualsMethod_WithDifferentValues_ShouldReturnFalse()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(9, 4);
        Assert.IsFalse(data1.Equals(data2));
    }

    [Test]  // 验证重载的HashCode语句是否正常，这将决定此类型作为key时的性能，如果测试遍历性能，可以以此为基础扩展性能分析
    public void GetHashCode_WithEqualValues_ShouldReturnSameHashCode()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(15, 4);
        Assert.AreEqual(data1.GetHashCode(), data2.GetHashCode());
    }

    [Test]  // 验证重载的HashCode语句是否正常，这将决定此类型作为key时的性能，如果测试遍历性能，可以以此为基础扩展性能分析
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCodes()
    {
        var data1 = new BitData(15, 4);
        var data2 = new BitData(9, 4);
        Assert.AreNotEqual(data1.GetHashCode(), data2.GetHashCode());
    }

    [Test]  // 验证重载的ToString语句是否正常
    public void ToString_ShouldReturnCorrectString()
    {
        var data = new BitData(15, 4);
        string expected = "Value: 15, BitLength: 4";
        Assert.AreEqual(expected, data.ToString());
    }
}