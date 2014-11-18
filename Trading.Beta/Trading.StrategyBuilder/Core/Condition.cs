using System;

namespace Trading.StrategyBuilder.Core
{
    [Equals]
    public class Condition
    {
        public Condition LeftOperand { get; set; }
        public Operator Operator { get; set; }
        public Condition RightOperand { get; set; }

        public Condition()
        {
        }

        public Condition(Condition leftOperand, Operator @operator, Condition rightOperand)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
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