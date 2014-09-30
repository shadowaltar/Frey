using System.Collections.Generic;

namespace Algorithms.Exercises
{
    public static class Exercises
    {
        public static bool CheckParentheses(string value)
        {
            var stack = new Stack<char>();
            foreach (var c in value)
            {
                switch (c)
                {
                    case '[':
                    case '{':
                    case '(':
                        stack.Push(c);
                        break;
                    case ']':
                        {
                            var left = stack.Pop();
                            if (left != '[')
                                return false;
                            break;
                        }
                    case '}':
                        {
                            var left = stack.Pop();
                            if (left != '{')
                                return false;
                            break;
                        }
                    case ')':
                        {
                            var left = stack.Pop();
                            if (left != '(')
                                return false;
                            break;
                        }
                }
            }
            return true;
        }
    }
}