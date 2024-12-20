// Decompiled with JetBrains decompiler
// Type: BugSense.Helpers.MD5
// Assembly: BugSense-WP8.1, Version=3.6.6.0, Culture=neutral, PublicKeyToken=null
// MVID: 65EE3F6C-C42A-4906-B97F-DFE12AEFD537
// Assembly location: C:\Users\Admin\Desktop\RE\IntouchApp\BugSense-WP8.1.dll

using System;


namespace BugSense.Helpers
{
  internal class MD5
  {
    protected static readonly uint[] T = new uint[64]
    {
      3614090360U,
      3905402710U,
      606105819U,
      3250441966U,
      4118548399U,
      1200080426U,
      2821735955U,
      4249261313U,
      1770035416U,
      2336552879U,
      4294925233U,
      2304563134U,
      1804603682U,
      4254626195U,
      2792965006U,
      1236535329U,
      4129170786U,
      3225465664U,
      643717713U,
      3921069994U,
      3593408605U,
      38016083U,
      3634488961U,
      3889429448U,
      568446438U,
      3275163606U,
      4107603335U,
      1163531501U,
      2850285829U,
      4243563512U,
      1735328473U,
      2368359562U,
      4294588738U,
      2272392833U,
      1839030562U,
      4259657740U,
      2763975236U,
      1272893353U,
      4139469664U,
      3200236656U,
      681279174U,
      3936430074U,
      3572445317U,
      76029189U,
      3654602809U,
      3873151461U,
      530742520U,
      3299628645U,
      4096336452U,
      1126891415U,
      2878612391U,
      4237533241U,
      1700485571U,
      2399980690U,
      4293915773U,
      2240044497U,
      1873313359U,
      4264355552U,
      2734768916U,
      1309151649U,
      4149444226U,
      3174756917U,
      718787259U,
      3951481745U
    };
    protected uint[] X = new uint[16];
    protected Digest dgFingerPrint;
    protected byte[] m_byteInput;

    public event MD5.ValueChanging OnValueChanging;

    public event MD5.ValueChanged OnValueChanged;

    public string Value
    {
      get
      {
        char[] chArray = new char[this.m_byteInput.Length];
        for (int index = 0; index < this.m_byteInput.Length; ++index)
          chArray[index] = (char) this.m_byteInput[index];
        return new string(chArray);
      }
      set
      {
        if (this.OnValueChanging != null)
          this.OnValueChanging((object) this, new MD5ChangingEventArgs(value));
        this.m_byteInput = new byte[value.Length];
        for (int index = 0; index < value.Length; ++index)
          this.m_byteInput[index] = (byte) value[index];
        this.dgFingerPrint = this.CalculateMD5Value();
        if (this.OnValueChanged == null)
          return;
        this.OnValueChanged((object) this, new MD5ChangedEventArgs(value, this.dgFingerPrint.ToString()));
      }
    }

    public byte[] ValueAsByte
    {
      get
      {
        byte[] valueAsByte = new byte[this.m_byteInput.Length];
        for (int index = 0; index < this.m_byteInput.Length; ++index)
          valueAsByte[index] = this.m_byteInput[index];
        return valueAsByte;
      }
      set
      {
        if (this.OnValueChanging != null)
          this.OnValueChanging((object) this, new MD5ChangingEventArgs(value));
        this.m_byteInput = new byte[value.Length];
        for (int index = 0; index < value.Length; ++index)
          this.m_byteInput[index] = value[index];
        this.dgFingerPrint = this.CalculateMD5Value();
        if (this.OnValueChanged == null)
          return;
        this.OnValueChanged((object) this, new MD5ChangedEventArgs(value, this.dgFingerPrint.ToString()));
      }
    }

    public string FingerPrint => this.dgFingerPrint.ToString();

    public MD5() => this.Value = "";

    protected Digest CalculateMD5Value()
    {
      Digest md5Value = new Digest();
      byte[] paddedBuffer = this.CreatePaddedBuffer();
      uint num = (uint) (paddedBuffer.Length * 8) / 32U;
      for (uint block = 0; block < num / 16U; ++block)
      {
        this.CopyBlock(paddedBuffer, block);
        this.PerformTransformation(ref md5Value.A, ref md5Value.B, ref md5Value.C, ref md5Value.D);
      }
      return md5Value;
    }

    protected void TransF(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + MD5Helper.RotateLeft(a + (uint) ((int) b & (int) c | ~(int) b & (int) d) + this.X[(int) k] + MD5.T[(int) (i - 1U)], s);
    }

    protected void TransG(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + MD5Helper.RotateLeft(a + (uint) ((int) b & (int) d | (int) c & ~(int) d) + this.X[(int) k] + MD5.T[(int) (i - 1U)], s);
    }

    protected void TransH(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + MD5Helper.RotateLeft(a + (b ^ c ^ d) + this.X[(int) k] + MD5.T[(int) (i - 1U)], s);
    }

    protected void TransI(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i)
    {
      a = b + MD5Helper.RotateLeft(a + (c ^ (b | ~d)) + this.X[(int) k] + MD5.T[(int) (i - 1U)], s);
    }

    protected void PerformTransformation(ref uint A, ref uint B, ref uint C, ref uint D)
    {
      uint num1 = A;
      uint num2 = B;
      uint num3 = C;
      uint num4 = D;
      this.TransF(ref A, B, C, D, 0U, (ushort) 7, 1U);
      this.TransF(ref D, A, B, C, 1U, (ushort) 12, 2U);
      this.TransF(ref C, D, A, B, 2U, (ushort) 17, 3U);
      this.TransF(ref B, C, D, A, 3U, (ushort) 22, 4U);
      this.TransF(ref A, B, C, D, 4U, (ushort) 7, 5U);
      this.TransF(ref D, A, B, C, 5U, (ushort) 12, 6U);
      this.TransF(ref C, D, A, B, 6U, (ushort) 17, 7U);
      this.TransF(ref B, C, D, A, 7U, (ushort) 22, 8U);
      this.TransF(ref A, B, C, D, 8U, (ushort) 7, 9U);
      this.TransF(ref D, A, B, C, 9U, (ushort) 12, 10U);
      this.TransF(ref C, D, A, B, 10U, (ushort) 17, 11U);
      this.TransF(ref B, C, D, A, 11U, (ushort) 22, 12U);
      this.TransF(ref A, B, C, D, 12U, (ushort) 7, 13U);
      this.TransF(ref D, A, B, C, 13U, (ushort) 12, 14U);
      this.TransF(ref C, D, A, B, 14U, (ushort) 17, 15U);
      this.TransF(ref B, C, D, A, 15U, (ushort) 22, 16U);
      this.TransG(ref A, B, C, D, 1U, (ushort) 5, 17U);
      this.TransG(ref D, A, B, C, 6U, (ushort) 9, 18U);
      this.TransG(ref C, D, A, B, 11U, (ushort) 14, 19U);
      this.TransG(ref B, C, D, A, 0U, (ushort) 20, 20U);
      this.TransG(ref A, B, C, D, 5U, (ushort) 5, 21U);
      this.TransG(ref D, A, B, C, 10U, (ushort) 9, 22U);
      this.TransG(ref C, D, A, B, 15U, (ushort) 14, 23U);
      this.TransG(ref B, C, D, A, 4U, (ushort) 20, 24U);
      this.TransG(ref A, B, C, D, 9U, (ushort) 5, 25U);
      this.TransG(ref D, A, B, C, 14U, (ushort) 9, 26U);
      this.TransG(ref C, D, A, B, 3U, (ushort) 14, 27U);
      this.TransG(ref B, C, D, A, 8U, (ushort) 20, 28U);
      this.TransG(ref A, B, C, D, 13U, (ushort) 5, 29U);
      this.TransG(ref D, A, B, C, 2U, (ushort) 9, 30U);
      this.TransG(ref C, D, A, B, 7U, (ushort) 14, 31U);
      this.TransG(ref B, C, D, A, 12U, (ushort) 20, 32U);
      this.TransH(ref A, B, C, D, 5U, (ushort) 4, 33U);
      this.TransH(ref D, A, B, C, 8U, (ushort) 11, 34U);
      this.TransH(ref C, D, A, B, 11U, (ushort) 16, 35U);
      this.TransH(ref B, C, D, A, 14U, (ushort) 23, 36U);
      this.TransH(ref A, B, C, D, 1U, (ushort) 4, 37U);
      this.TransH(ref D, A, B, C, 4U, (ushort) 11, 38U);
      this.TransH(ref C, D, A, B, 7U, (ushort) 16, 39U);
      this.TransH(ref B, C, D, A, 10U, (ushort) 23, 40U);
      this.TransH(ref A, B, C, D, 13U, (ushort) 4, 41U);
      this.TransH(ref D, A, B, C, 0U, (ushort) 11, 42U);
      this.TransH(ref C, D, A, B, 3U, (ushort) 16, 43U);
      this.TransH(ref B, C, D, A, 6U, (ushort) 23, 44U);
      this.TransH(ref A, B, C, D, 9U, (ushort) 4, 45U);
      this.TransH(ref D, A, B, C, 12U, (ushort) 11, 46U);
      this.TransH(ref C, D, A, B, 15U, (ushort) 16, 47U);
      this.TransH(ref B, C, D, A, 2U, (ushort) 23, 48U);
      this.TransI(ref A, B, C, D, 0U, (ushort) 6, 49U);
      this.TransI(ref D, A, B, C, 7U, (ushort) 10, 50U);
      this.TransI(ref C, D, A, B, 14U, (ushort) 15, 51U);
      this.TransI(ref B, C, D, A, 5U, (ushort) 21, 52U);
      this.TransI(ref A, B, C, D, 12U, (ushort) 6, 53U);
      this.TransI(ref D, A, B, C, 3U, (ushort) 10, 54U);
      this.TransI(ref C, D, A, B, 10U, (ushort) 15, 55U);
      this.TransI(ref B, C, D, A, 1U, (ushort) 21, 56U);
      this.TransI(ref A, B, C, D, 8U, (ushort) 6, 57U);
      this.TransI(ref D, A, B, C, 15U, (ushort) 10, 58U);
      this.TransI(ref C, D, A, B, 6U, (ushort) 15, 59U);
      this.TransI(ref B, C, D, A, 13U, (ushort) 21, 60U);
      this.TransI(ref A, B, C, D, 4U, (ushort) 6, 61U);
      this.TransI(ref D, A, B, C, 11U, (ushort) 10, 62U);
      this.TransI(ref C, D, A, B, 2U, (ushort) 15, 63U);
      this.TransI(ref B, C, D, A, 9U, (ushort) 21, 64U);
      A += num1;
      B += num2;
      C += num3;
      D += num4;
    }

    protected byte[] CreatePaddedBuffer()
    {
      uint num1 = (uint) ((448 - this.m_byteInput.Length * 8 % 512 + 512) % 512);
      if (num1 == 0U)
        num1 = 512U;
      uint length = (uint) ((ulong) this.m_byteInput.Length + (ulong) (num1 / 8U) + 8UL);
      ulong num2 = (ulong) this.m_byteInput.Length * 8UL;
      byte[] paddedBuffer = new byte[(int) length];
      for (int index = 0; index < this.m_byteInput.Length; ++index)
        paddedBuffer[index] = this.m_byteInput[index];
      paddedBuffer[this.m_byteInput.Length] |= (byte) 128;
      for (int index = 8; index > 0; --index)
        paddedBuffer[(long) length - (long) index] = (byte) (num2 >> (8 - index) * 8 & (ulong) byte.MaxValue);
      return paddedBuffer;
    }

    protected void CopyBlock(byte[] bMsg, uint block)
    {
      block <<= 6;
      for (uint index = 0; index < 61U; index += 4U)
        this.X[(int) (index >> 2)] = (uint) ((int) bMsg[(int) (block + (index + 3U))] << 24 | (int) bMsg[(int) (block + (index + 2U))] << 16 | (int) bMsg[(int) (block + (index + 1U))] << 8) | (uint) bMsg[(int) (block + index)];
    }

    public delegate void ValueChanging(object sender, MD5ChangingEventArgs Changing);

    public delegate void ValueChanged(object sender, MD5ChangedEventArgs Changed);
  }
}
