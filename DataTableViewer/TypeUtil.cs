
using System;

namespace DataTableViewer
{
    public static class TypeUtil
    {
        /// <summary>
        /// Checks if o1 and o2 are number types and if so, tries to make them the same type so comparison can succeed.
        /// </summary>
        public static void NormalizeTypes(ref Object o1, ref Object o2)
        {
            var type1 = o1.GetType();
            var type2 = o2.GetType();

            if(type1 == typeof(double) && type2 != typeof(double))
            {
                o2 = o2.ToDouble();
            }
            else if(type2 == typeof(double) && type1 != typeof(double))
            {
                o1 = o1.ToDouble();
            }
            else if(type1 == typeof(int) && type2 != typeof(int))
            {
                o2 = o2.ToInt();
            }
            else if(type2 == typeof(int) && type1 != typeof(int))
            {
                o1 = o1.ToInt();
            }
            else if(type1 == typeof(long) && type2 != typeof(long))
            {
                o2 = o2.ToLong();
            }
            else if(type2 == typeof(long) && type1 != typeof(long))
            {
                o1 = o1.ToLong();
            }
            else if(type1 == typeof(float) && type2 != typeof(float))
            {
                o2 = o2.ToFloat();
            }
            else if(type2 == typeof(float) && type1 != typeof(float))
            {
                o1 = o1.ToFloat();
            }
            else if(type1 == typeof(short) && type2 != typeof(short))
            {
                o2 = o2.ToShort();
            }
            else if(type2 == typeof(short) && type1 != typeof(short))
            {
                o1 = o1.ToShort();
            }
            else if(type1 == typeof(byte) && type2 != typeof(byte))
            {
                o2 = o2.ToByte();
            }
            else if(type2 == typeof(byte) && type1 != typeof(byte))
            {
                o1 = o1.ToByte();
            }
            else if(type1 == typeof(ulong) && type2 != typeof(ulong))
            {
                o2 = o2.ToULong();
            }
            else if(type2 == typeof(ulong) && type1 != typeof(ulong))
            {
                o1 = o1.ToULong();
            }
            else if(type1 == typeof(uint) && type2 != typeof(uint))
            {
                o2 = o2.ToUInt();
            }
            else if(type2 == typeof(uint) && type1 != typeof(uint))
            {
                o1 = o1.ToUInt();
            }
            else if(type1 == typeof(ushort) && type2 != typeof(ushort))
            {
                o2 = o2.ToUShort();
            }
            else if(type2 == typeof(ushort) && type1 != typeof(ushort))
            {
                o1 = o1.ToUShort();
            }
            else if(type1 == typeof(sbyte) && type2 != typeof(sbyte))
            {
                o2 = o2.ToSByte();
            }
            else if(type2 == typeof(sbyte) && type1 != typeof(sbyte))
            {
                o1 = o1.ToSByte();
            }
        }
    }
}