using System;
using System.Collections.Generic;

namespace HnNetty.Tools
{
    public class CmdHelper
    {
        public static byte GetCRC(byte[] bytes)
        {
            var value = 0;
            for (var i = 0; i < bytes.Length; i++)
                value += bytes[i];
            return BitConverter.GetBytes(value)[0];
        }

        /// <summary>
        ///     获取BCD码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetBCD(int value)
        {
            var i = value / 10;
            var j = value % 10;
            return (byte) ((i << 4) + j);
        }

        /// <summary>
        ///     获取BCD码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetBCD(byte value)
        {
            return (value >> 4) * 10 + (value & 0xF);
        }

        /// <summary>
        ///     日期 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetBCD_Date(DateTime data)
        {
            var list = new List<byte>();
            list.Add(GetBCD(data.Year - 2000));
            list.Add(GetBCD(data.Month));
            list.Add(GetBCD(data.Day));
            list.Add(GetBCD(data.Hour));
            list.Add(GetBCD(data.Minute));
            list.Add(GetBCD(data.Second));
            return list.ToArray();
        }

        /// <summary>
        ///     日期 带周
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static DateTime GetBCD_Date(byte[] buff)
        {
            var sec = GetBCD(buff[5]);
            var min = GetBCD(buff[4]);
            var hour = GetBCD(buff[3]);
            var day = GetBCD(buff[2]);
            var month = GetBCD(buff[1]);
            var year = GetBCD(buff[0]) + 2000;
            try
            {
                return new DateTime(year, month, day, hour, min, sec);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        ///     日期 带周
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static DateTime Get_Date(byte[] buff)
        {
            var sec = buff[5];
            var min = buff[4];
            var hour = buff[3];
            var day = buff[2];
            var month = buff[1];
            var year = buff[0] + 2000;
            try
            {
                return new DateTime(year, month, day, hour, min, sec);
            }
            catch
            {
                return DateTime.Now;
            }
        }
    }
}