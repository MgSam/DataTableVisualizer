using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringToExpression.GrammerDefinitions;

namespace DataTableViewer
{
    /// <summary>
    /// Extensions.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets a dictionary of the columns and the items for this row.
        /// </summary>
        /// <param name="dataRow">The row for which to get an item array.</param>
        /// <param name="caseInvariant">Whether the dictionary should be case-invariant.</param>
        /// <returns>A dictionary of columns keyed to items for this row.</returns>
        public static Dictionary<string, object> GetItemDictionary(this DataRow dataRow, bool caseInvariant = false)
        {
            if(dataRow == null) throw new ArgumentNullException(nameof(dataRow));

            var cols = dataRow.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            var items = dataRow.ItemArray;
            var result = new Dictionary<string, object>(items.Length, caseInvariant ? StringComparer.OrdinalIgnoreCase : default(StringComparer));

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var col = cols[i];

                result[col] = item;
            }

            return result;
        }

        /// <summary>
        /// Creates a new BinaryOperatorDefinition with the given expression builder.
        /// </summary>
        /// <param name="definition">The existing BinaryOperatorDefinition.</param>
        /// <param name="name">New name.</param>
        /// <param name="regex">New regex.</param>
        /// <param name="orderOfPrecedence">New order of precedence.</param>
        /// <param name="expressionBuilder">The expression builder.</param>
        /// <returns>The new BinaryOperatorDefinition.</returns>
        public static BinaryOperatorDefinition With(this BinaryOperatorDefinition definition, 
            String name = null,
            String regex = null,
            int? orderOfPrecedence = null,
            Func<Expression, Expression, Expression> expressionBuilder = null)
        {
            return new BinaryOperatorDefinition(
                name ?? definition.Name, 
                regex ?? definition.Regex,
                orderOfPrecedence ?? definition.OrderOfPrecedence ?? -1,
                expressionBuilder);
        }

        /// <summary>
        /// Applies the as operator. Should only be used on classes to avoid boxing on structs.
        /// </summary>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="o">Object to apply 'as' to.</param>
        /// <returns>Type as the output.</returns>
        public static TOut As<TOut>(this Object o) where TOut : class
        {
            return o as TOut;
        }

        /// <summary>
        /// Converts the object to a double representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable double.</returns>
        public static double? ToDouble(this object o)
        {
            var success = double.TryParse(o.ToString(), out var result);
            return success ? result : (double?) null;
        }

        /// <summary>
        /// Converts the object to a float representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable float.</returns>
        public static float? ToFloat(this object o)
        {
            var success = float.TryParse(o.ToString(), out var result);
            return success ? result : (float?)null;
        }

        /// <summary>
        /// Converts the object to a long representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable long.</returns>
        public static long? ToLong(this object o)
        {
            var success = long.TryParse(o.ToString(), out var result);
            return success ? result : (long?)null;
        }

        /// <summary>
        /// Converts the object to a int representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable int.</returns>
        public static int? ToInt(this object o)
        {
            var success = int.TryParse(o.ToString(), out var result);
            return success ? result : (int?)null;
        }

        /// <summary>
        /// Converts the object to a short representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable short.</returns>
        public static short? ToShort(this object o)
        {
            var success = short.TryParse(o.ToString(), out var result);
            return success ? result : (short?)null;
        }

        /// <summary>
        /// Converts the object to a byte representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable byte.</returns>
        public static byte? ToByte(this object o)
        {
            var success = byte.TryParse(o.ToString(), out var result);
            return success ? result : (byte?)null;
        }

        /// <summary>
        /// Converts the object to a ulong representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable ulong.</returns>
        public static ulong? ToULong(this object o)
        {
            var success = ulong.TryParse(o.ToString(), out var result);
            return success ? result : (ulong?)null;
        }

        /// <summary>
        /// Converts the object to a uint representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable uint.</returns>
        public static uint? ToUInt(this object o)
        {
            var success = uint.TryParse(o.ToString(), out var result);
            return success ? result : (uint?)null;
        }

        /// <summary>
        /// Converts the object to a ushort representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable ushort.</returns>
        public static ushort? ToUShort(this object o)
        {
            var success = ushort.TryParse(o.ToString(), out var result);
            return success ? result : (ushort?)null;
        }

        /// <summary>
        /// Converts the object to a sbyte representation if possible.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>Type as a nullable sbyte.</returns>
        public static sbyte? ToSByte(this object o)
        {
            var success = sbyte.TryParse(o.ToString(), out var result);
            return success ? result : (sbyte?)null;
        }
    }//End class Extensions
}//End namespace
