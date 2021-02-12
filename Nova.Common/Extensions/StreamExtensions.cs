using System;
using System.IO;
using System.Text;

namespace Nova.Common.Extensions
{
    public static class StreamExtensions
    {
        public static long WriteInt64(this Stream stream, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, 8);
            return 8;
        }

        public static long ReadInt64(this Stream stream)
        {
            var bytes = new byte[8];
            stream.Read(bytes, 0, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static long WriteUInt64(this Stream stream, ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, 8);
            return 8;
        }

        public static ulong ReadUInt64(this Stream stream)
        {
            var bytes = new byte[8];
            stream.Read(bytes, 0, 8);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static long WriteInt16(this Stream stream, short value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, 2);
            return 2;
        }

        public static short ReadInt16(this Stream stream)
        {
            var bytes = new byte[2];
            stream.Read(bytes, 0, 2);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static long WriteUInt16(this Stream stream, ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, 2);
            return 2;
        }

        public static ushort ReadUInt16(this Stream stream)
        {
            var bytes = new byte[2];
            stream.Read(bytes, 0, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static long WriteBool(this Stream stream, bool value)
        {
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, 1);
            return 1;
        }

        public static bool ReadBool(this Stream stream)
        {
            var bytes = new byte[1];
            stream.Read(bytes, 0, 1);
            return BitConverter.ToBoolean(bytes, 0);
        }

        public static long WriteStringWithLength(this Stream stream, string text)
        {
            if (text == null)
            {
                stream.WriteUInt16(0);
                return 2;
            }

            if (text.Length > ushort.MaxValue)
                return 0;

            var textBytes = Encoding.ASCII.GetBytes(text);
            
            stream.WriteUInt16((ushort) textBytes.Length);
            stream.Write(textBytes, 0, textBytes.Length);

            return 2 + textBytes.Length;
        }

        public static string ReadStringWithLength(this Stream stream)
        {
            var length = stream.ReadUInt16();

            if (length == 0)
                return null;

            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);

            return Encoding.ASCII.GetString(buffer);
        }
    }
}
