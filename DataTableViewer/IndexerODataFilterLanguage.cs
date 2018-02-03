using StringToExpression.LanguageDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using StringToExpression.GrammerDefinitions;
using System.Linq.Expressions;
using System.Linq;
using StringToExpression.Util;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataTableViewer
{


    /// <summary>
    /// Provides an OData filter that uses indexers for looking up keys rather than properties (the default ODataFilter implementation).
    /// </summary>
    public class IndexerODataFilterLanguage : ODataFilterLanguage
    {
        /// <inheritdoc />
        protected override IEnumerable<GrammerDefinition> AllDefinitions()
        {
            var allDefinitions = base.AllDefinitions();

            //Unfortunately expressions are picky about comparing types. Because all of our
            //properties are of type `Object` this makes things difficult. To ease some of
            //the issues we will cast an `Object` to be the same value as the other side of
            //the expression. This only works if the objects stored in the dictionary are
            //the same types as the constants in the expression, i.e. int, decimal,
            return allDefinitions.Select(def =>
            {
                if (!(def is BinaryOperatorDefinition binary))
                    return def;

                return new BinaryOperatorDefinition(binary.Name,
                    binary.Regex,
                    binary.OrderOfPrecedence.Value,
                    (left, right) =>
                    {
                        if (right.Type.IsValueType)
                            right = Expression.Convert(right, typeof(object));
                        else if (left.Type.IsValueType)
                            left = Expression.Convert(left, typeof(object));
                        return binary.ExpressionBuilder(new[] { left, right });

                        //if (left.Type == typeof(object) && right.Type.IsValueType)
                        //    left = Expression.Convert(left, right.Type);
                        //else if (left.Type.IsValueType && right.Type == typeof(object))
                        //    right = Expression.Convert(right, left.Type);
                        //return binary.ExpressionBuilder(new[] { left, right });
                    });
            });
        }

        /// <inheritdoc />
        protected override IEnumerable<GrammerDefinition> PropertyDefinitions()
        {
            return new[]
            {
                 //Properties
                 new OperandDefinition(
                    name:"PROPERTY_PATH",
                    regex: @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*/?)+",
                    expressionBuilder: (value, parameters) => {
                        var row = (Expression)parameters[0];
                        
                        //we will retrieve the value of the property by reading the indexer property
                        var indexProperty = row.Type.GetDefaultMembers().OfType<PropertyInfo>().Single();
                        return Expression.Call(row, indexProperty.GetMethod, Expression.Constant(value, typeof(string)));
                    }),
            };
        }

        /// <inheritdoc />
        protected override IEnumerable<GrammerDefinition> LogicalOperatorDefinitions()
        {
            var logicalOperations = base.LogicalOperatorDefinitions().ToDictionary(g => g.Name);

            logicalOperations["EQ"] = logicalOperations["EQ"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.Equals)));

            logicalOperations["NE"] = logicalOperations["NE"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.NotEquals)));
            logicalOperations["GT"] = logicalOperations["GT"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.GreaterThan)));
            logicalOperations["GE"] = logicalOperations["GE"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.GreaterThanOrEquals)));

            logicalOperations["LT"] = logicalOperations["LT"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.LessThan)));
            logicalOperations["LE"] = logicalOperations["LE"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.LessThanOrEquals)));            

            logicalOperations["AND"] = logicalOperations["AND"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => (Expression)Expression.And(Expression.Convert(left, typeof(bool)), Expression.Convert(right, typeof(bool))));
            logicalOperations["OR"]  = logicalOperations["OR"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => (Expression)Expression.Or(Expression.Convert(left, typeof(bool)), Expression.Convert(right, typeof(bool))));

            logicalOperations["CT"] = new BinaryOperatorDefinition("CT", "ct", 16, (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.Contains)));
            logicalOperations["RX"] = new BinaryOperatorDefinition("RX", "rx", 16, (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.Regex)));
            //logicalOperations["CT"].As<BinaryOperatorDefinition>().With(expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.Contains)));
            //Niceties that don't seem work because they mess up the parser (other operators are contained within them)
            //logicalOperations["NEQ"] = logicalOperations["NE"].As<BinaryOperatorDefinition>().With(name: "NEQ", regex: "neq", expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.NotEquals)));
            //logicalOperations["GTE"] = logicalOperations["GE"].As<BinaryOperatorDefinition>().With(name: "GTE", regex: "gte", expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.GreaterThanOrEquals)));
            //logicalOperations["LTE"] = logicalOperations["LE"].As<BinaryOperatorDefinition>().With(name: "LTE", regex: "lte", expressionBuilder: (left, right) => Expression.Call(CompareMethod, left, right, Expression.Constant(CompareType.LessThanOrEquals)));

            return logicalOperations.Values;
        }

        /// <inheritdoc />
        protected override IEnumerable<GrammerDefinition> TypeDefinitions()
        {
            var typeDefinitions = base.TypeDefinitions().ToDictionary(g => g.Name);

            typeDefinitions["DOUBLE"] = new OperandDefinition("DOUBLE", "\\-?\\d+\\.?\\d*", x => (Expression)Expression.Constant(double.Parse(x)));

            return typeDefinitions.Values;
        }

        /// <inheritdoc />
        protected override IEnumerable<GrammerDefinition> ArithmeticOperatorDefinitions()
        {
            //We remove all arthimetic operators, because no one needs arthimetic in a filter, 
            //and the implementation of these did not look for spaces, thus any field name which contains an operator string was broken
            return Enumerable.Empty<GrammerDefinition>();
        }

        /// <summary>
        /// Implements equality that works for various types of objects.
        /// </summary>
        /// <param name="o1">First object.</param>
        /// <param name="o2">Second object.</param>
        /// <param name="compareType">The type of comparison.</param>
        /// <returns>Whether the objects were equal.</returns>
        private static bool Compare(Object o1, Object o2, CompareType compareType)
        {
            var type1 = o1.GetType();
            
            if (type1 == typeof(string))
            {
                var s = (string) o1;
                switch(compareType)
                {
                    case CompareType.Equals:
                        return StringComparer.OrdinalIgnoreCase.Equals(s, o2);
                    case CompareType.NotEquals:
                        return !StringComparer.OrdinalIgnoreCase.Equals(s, o2);
                    case CompareType.GreaterThan:
                        return StringComparer.OrdinalIgnoreCase.Compare(s, o2) > 0;
                    case CompareType.GreaterThanOrEquals:
                        return StringComparer.OrdinalIgnoreCase.Compare(s, o2) >= 0;
                    case CompareType.LessThan:
                        return StringComparer.OrdinalIgnoreCase.Compare(s, o2) < 0;
                    case CompareType.LessThanOrEquals:
                        return StringComparer.OrdinalIgnoreCase.Compare(s, o2) <= 0;
                    case CompareType.Contains:
                        return s.IndexOf(o2.ToString(), StringComparison.OrdinalIgnoreCase) > -1;
                    case CompareType.Regex:
                        return Regex.IsMatch(s, o2.ToString(), RegexOptions.IgnoreCase);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
                }
                
            }
            else if (type1 == typeof(DateTime))
            {
                var d1 = (DateTime) o1;
                var o2WasDate = DateTime.TryParse(o2.ToString(), out var d2);

                switch (compareType)
                {
                    case CompareType.Equals:
                        return o2WasDate && d1 == d2;
                    case CompareType.NotEquals:
                        return o2WasDate && d1 != d2;
                    case CompareType.GreaterThan:
                        return o2WasDate && d1 > d2;
                    case CompareType.GreaterThanOrEquals:
                        return o2WasDate && d1 >= d2;
                    case CompareType.LessThan:
                        return o2WasDate && d1 < d2;
                    case CompareType.LessThanOrEquals:
                        return o2WasDate && d1 <= d2;
                    case CompareType.Contains:
                        return d1.ToString().IndexOf(o2.ToString(), StringComparison.OrdinalIgnoreCase) > -1;
                    case CompareType.Regex:
                        return Regex.IsMatch(d1.ToString(), d2.ToString(), RegexOptions.IgnoreCase);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
                }
            }
            else
            {
                TypeUtil.NormalizeTypes(ref o1, ref o2);
            }

            switch (compareType)
            {
                case CompareType.Equals:
                    return Comparer.Default.Compare(o1, o2) == 0;
                case CompareType.NotEquals:
                    return Comparer.Default.Compare(o1, o2) != 0;
                case CompareType.GreaterThan:
                    return Comparer.Default.Compare(o1, o2) > 0;
                case CompareType.GreaterThanOrEquals:
                    return Comparer.Default.Compare(o1, o2) >= 0;
                case CompareType.LessThan:
                    return Comparer.Default.Compare(o1, o2) < 0;
                case CompareType.LessThanOrEquals:
                    return Comparer.Default.Compare(o1, o2) <= 0;
                case CompareType.Contains:
                    return o1.ToString().IndexOf(o2.ToString(), StringComparison.OrdinalIgnoreCase) > -1;
                case CompareType.Regex:
                    return Regex.IsMatch(o1.ToString(), o2.ToString(), RegexOptions.IgnoreCase);
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }//End method Compare

        private static MethodInfo CompareMethod => _compareMethod ?? (_compareMethod = getMethodInfo(nameof(Compare)));
        private static MethodInfo _compareMethod;

        private static MethodInfo getMethodInfo(String methodName)
        {
            return typeof(IndexerODataFilterLanguage).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(Object), typeof(Object), typeof(CompareType)}, null);
        }

        private static void normalizeNumberTypes(Object o1, Object o2)
        {
            
        }

        private enum CompareType
        {
            Equals,
            NotEquals,
            GreaterThan,
            GreaterThanOrEquals,
            LessThan,
            LessThanOrEquals,
            Contains,
            Regex
        }
    }
}
