namespace DataBox
{
    /// <summary>
    /// 位打包工具类，用于在字节数组中存储和检索不同位长度的数据。实际空间占用为实际bit数+16bit(数据长度和组数)
    /// <br/>
    /// 整体结构类似二维数组，X Bit的Y 组数据
    /// <br/>
    /// 整体格式类似: [[byte][byte][byte]] -> [8bit][8bit][8bit]
    /// <br/>
    /// 如果数据是2个11bit，内存布局为：<br/>
    /// [[byte][byte][byte]]<br/>
    /// [11111111][11122222][222222]=22 bit，最后一个bit没写满,整个数组一共使用38 bit<br/>
    /// </summary>
    public class BitPacker
    {
        private readonly byte[] _data; // 原始数据
        private readonly byte _bitLength; // 每个数据项的数据位长
        private readonly byte _count; // 数据项数量


        /// <summary>
        /// 初始化一个BitPacker实例
        /// </summary>
        /// <param name="bitLength">每个数据项的位长度（8到32位）</param>
        /// <param name="count">数据项的数量</param>
        public BitPacker(int bitLength, int count)
        {
            if (bitLength is < 8 or > 32)
                throw new ArgumentOutOfRangeException(nameof(bitLength), "位长度必须在8到32位之间");

            _bitLength = Convert.ToByte(bitLength);
            _count = Convert.ToByte(count);
            var totalBits = bitLength * count;
            var totalBytes = (totalBits + 7) / 8; // 向上取整
            _data = new byte[totalBytes];
        }

        /// <summary>
        /// 设置指定索引的数据值
        /// </summary>
        /// <param name="index">数据项的索引（从0开始）</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(int index, uint value)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("索引超出范围");
            if (value >= (1u << _bitLength) && _bitLength < 32)
                throw new ArgumentOutOfRangeException(nameof(value), "值超出了指定的位长度");

            var bitPosition = index * _bitLength; // 相当于起始写入的bit下标(实际位置)
            var bytePosition = bitPosition / 8; // 相当于起始写入的byte下标(数组位置)
            var bitOffset = bitPosition % 8; // 相当于当前这个Byte内开始写入的位置(1~8)，因为数据不一定是8的倍数，这样更紧密排布

            // 写入值
            for (var i = 0; i < _bitLength; i++) // 按位写入每一行
            {
                var currentBit = bitOffset + i;
                var currentByte = bytePosition + (currentBit / 8);
                var currentBitInByte = currentBit % 8;
                var bitValue = (value & (1u << i)) != 0; // 以bool(1bit)的形式为当前位数据取与，获取1bit的值

                if (bitValue)
                    _data[currentByte] |= (byte)(1 << currentBitInByte); // 如果当前bit=1, 与原数据取或，做当前位为1操作
                else
                    _data[currentByte] &=
                        (byte)~(1 << currentBitInByte); // 按位取反~，得到一个字节，得到在currentBitInByte位为0，其他位为1的数据，再与实数取与，做清零当前位操作
            }
        }

        /// <summary>
        /// 获取指定索引的数据值。
        /// </summary>
        /// <param name="index">数据项的索引（从0开始）。</param>
        /// <returns>数据值。</returns>
        public uint GetValue(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("索引超出范围。");

            var bitPosition = index * _bitLength;
            var bytePosition = bitPosition / 8;
            var bitOffset = bitPosition % 8;
            uint value = 0;

            for (var i = 0; i < _bitLength; i++)
            {
                var currentBit = bitOffset + i;
                var currentByte = bytePosition + (currentBit / 8);
                var currentBitInByte = currentBit % 8;
                var bitValue = (_data[currentByte] & (1 << currentBitInByte)) != 0;

                if (bitValue)
                    value |= (1u << i);
            }

            return value;
        }

        /// <summary>
        /// 获取内部存储的原始字节数组
        /// </summary>
        public byte[] GetRawData()
        {
            return _data;
        }

        /// <summary>
        /// 从字节数组创建一个BitPacker实例
        /// </summary>
        /// <param name="rawData">字节数组</param>
        /// <param name="bitLength">每个数据项的位长度</param>
        /// <param name="count">数据项的数量</param>
        /// <returns>BitPacker实例。</returns>
        public static BitPacker FromRawData(byte[] rawData, int bitLength, int count)
        {
            var packer = new BitPacker(Convert.ToByte(bitLength), Convert.ToByte(count));
            if (rawData.Length != packer._data.Length)
                throw new ArgumentException("字节数组的内容长度与指定的bitLength和count不匹配。");
            Array.Copy(rawData, packer._data, rawData.Length);
            return packer;
        }
    }
}