using System;

namespace Trading.StrategyBuilder.Core
{
    [Equals]
    public class Condition
    {
        private readonly string leftStringOperand;
        private readonly string rightStringOperand;

        public Condition LeftOperand { get; private set; }
        public Operator Operator { get; private set; }
        public Condition RightOperand { get; private set; }

        public string LeftOperandValue { get { return leftStringOperand ?? LeftOperand.ToString(); } }
        public string RightOperandValue { get { return rightStringOperand ?? RightOperand.ToString(); } }

        public string LeftOperandSqlValue { get { return leftStringOperand ?? LeftOperand.ToWhereClause(); } }
        public string RightOperandSqlValue { get { return rightStringOperand ?? RightOperand.ToWhereClause(); } }

        public Condition()
        {
        }

        public Condition(string leftOperand, Operator @operator, string rightOperand)
        {
            leftStringOperand = leftOperand;
            rightStringOperand = rightOperand;
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
            leftStringOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        public Condition(Condition leftOperand, Operator @operator, string rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            rightStringOperand = rightOperand;
        }

        public string ToWhereClause()
        {
            return LeftOperandSqlValue + Operator.ToSqlLogicalOperator() + RightOperandSqlValue;
        }

        public override string ToString()
        {
            return LeftOperand + Operator.ToSymbol() + RightOperand;
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