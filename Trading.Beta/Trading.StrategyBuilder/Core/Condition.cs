using System;
using System.Collections;
using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    [Equals]
    public class Condition
    {
        public string LeftStringOperand { get; set; }
        public string RightStringOperand { get; set; }

        public Condition LeftOperand { get; set; }
        public Operator Operator { get; set; }
        public Condition RightOperand { get; set; }

        public string LeftOperandValue { get { return LeftStringOperand ?? LeftOperand.ToString(); } }
        public string RightOperandValue { get { return RightStringOperand ?? RightOperand.ToString(); } }

        public string LeftOperandSqlValue { get { return LeftStringOperand ?? LeftOperand.ToWhereClause(); } }
        public string RightOperandSqlValue { get { return RightStringOperand ?? RightOperand.ToWhereClause(); } }

        public string DisplayName { get { return ToString(); } }

        public Condition()
        {
        }

        public Condition(string leftOperand, Operator @operator, string rightOperand)
        {
            LeftStringOperand = leftOperand;
            RightStringOperand = rightOperand;
            Operator = @operator;
        }

        public Condition(Condition leftOperand, Operator @operator, Condition rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        public Condition(string leftOperand, Operator @operator, Condition rightOperand)
        {
            LeftStringOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        public Condition(Condition leftOperand, Operator @operator, string rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightStringOperand = rightOperand;
        }

        public string ToWhereClause()
        {
            return LeftOperandSqlValue + " " + Operator.ToSqlLogicalOperator() + " " + RightOperandSqlValue;
        }

        public override string ToString()
        {
            return LeftOperandValue + " " + Operator.ToSymbol() + " " + RightOperandValue;
        }

        public static Condition AllTrue(IEnumerable<Condition> conditions)
        {
            var result = new Condition();
        }
    }

    public enum Operator
    {
        EqualTo,
        SmallerThan,
        SmallerThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        NotEqualTo,
        And,
        Or,
        Xor,
    }

    public static class OperatorExtensions
    {
        public static Operator FromSymbol(this string @operator)
        {
            switch (@operator)
            {
                case "==":
                    return Operator.EqualTo;
                case "<":
                    return Operator.SmallerThan;
                case "<=":
                    return Operator.SmallerThanOrEqualTo;
                case ">":
                    return Operator.GreaterThan;
                case ">=":
                    return Operator.GreaterThanOrEqualTo;

                case "!=":
                    return Operator.NotEqualTo;
                case "&&":
                    return Operator.And;
                case "||":
                    return Operator.Or;
                case "^":
                    return Operator.Xor;
            }
            throw new InvalidOperationException();
        }
        public static string ToSymbol(this Operator @operator)
        {
            switch (@operator)
            {
                case Operator.EqualTo:
                    return "==";
                case Operator.SmallerThan:
                    return "<";
                case Operator.SmallerThanOrEqualTo:
                    return "<=";
                case Operator.GreaterThan:
                    return ">";
                case Operator.GreaterThanOrEqualTo:
                    return ">=";

                case Operator.NotEqualTo:
                    return "!=";
                case Operator.And:
                    return "&&";
                case Operator.Or:
                    return "||";
                case Operator.Xor:
                    return "^";
            }
            throw new InvalidOperationException();
        }

        public static string ToSqlLogicalOperator(this Operator @operator)
        {
            switch (@operator)
            {
                case Operator.And:
                    return " AND ";
                case Operator.Or:
                    return " OR ";
            }
            throw new InvalidOperationException();
        }
    }

    public class DataProcessing
    {
    }
}