// @Leisn (https://leisn.com , https://github.com/leisn)

namespace dCalculator.Infrastructures;

public class ExpressionExecutor
{
    private struct ExpUnit
    {
        public long Value;
        public char Operator;
        public bool IsOperator;
        public override string ToString()
        {
            return IsOperator ? Operator.ToString() : Value.ToString();
        }
    }

    public static long GetResult(string expression)
    {
        expression = expression.Replace(" ", "").Replace("<<", "<").Replace(">>", ">").Trim();
        var start = expression.LastIndexOf('(');
        while (start > -1)
        {
            var end = expression[start..].IndexOf(')');
            var subExpression = end > -1 ? expression.Substring(start + 1, end - 1) : expression[(start + 1)..];
            var subResult = GetSubResult(subExpression);
            expression = expression[..start] + subResult + expression[(start + end + 1)..];
            start = expression.LastIndexOf('(');
        }
        return GetSubResult(expression);
    }

    private static long GetSubResult(string expresssion)
    {
        Stack<ExpUnit> stack = [];

        #region split number and operators
        {
            int numberStart = -1;
            for (int i = 0; i < expresssion.Length; i++)
            {
                var ch = expresssion[i];
                if (char.IsDigit(ch))
                {
                    if (numberStart == -1)
                        numberStart = i;
                    continue;
                }
                if (numberStart != -1)
                {
                    stack.Push(new ExpUnit { Value = long.Parse(expresssion[numberStart..i]) });
                    numberStart = -1;
                }
                stack.Push(new ExpUnit { IsOperator = true, Operator = ch });
            }
            if (numberStart != -1)
                stack.Push(new ExpUnit { Value = long.Parse(expresssion[numberStart..]) });
        }
        #endregion

        Queue<ExpUnit> infixs = [];

        #region handle Unary
        {
            Stack<ExpUnit> temps = [];
            while (stack.TryPop(out var last))
            {
                if (!last.IsOperator)
                {
                    temps.Push(last);
                    continue;
                }
                if (last.Operator == '~')
                {
                    var temp = temps.Pop();
                    temps.Push(new ExpUnit { Value = ~temp.Value });
                    continue;
                }
                if (last.Operator == '-')
                {
                    bool hasNext = stack.TryPeek(out var next);
                    if ((hasNext && next.IsOperator) || !hasNext)
                    {
                        var temp = temps.Pop();
                        temps.Push(new ExpUnit { Value = -temp.Value });
                        continue;
                    }
                }
                temps.Push(last);
            }
            while (temps.TryPop(out var last))
                infixs.Enqueue(last);
        }
        #endregion

        Queue<ExpUnit> postfixs = [];
        #region infixs to postfixs
        {
            while (infixs.TryDequeue(out var unit))
            {
                if (!unit.IsOperator)
                {
                    postfixs.Enqueue(unit);
                    continue;
                }

                if (!stack.TryPeek(out var temp))
                {
                    stack.Push(unit);
                    continue;
                }

                int precedence = GetPrecedence(unit.Operator);
                if (precedence < GetPrecedence(temp.Operator))
                {
                    stack.Push(unit);
                    continue;
                }

                while (stack.TryPop(out temp))
                {
                    if (precedence >= GetPrecedence(temp.Operator))
                    {
                        postfixs.Enqueue(temp);
                    }
                    else
                    {
                        stack.Push(temp);
                        break;
                    }
                }
                stack.Push(unit);
            }
            while (stack.TryPop(out var pop))
                postfixs.Enqueue(pop);
        }
        #endregion

        //var str = string.Join(" ", postfixs);
        //Debug.WriteLine($"postfix: {str}");
        #region Calc
        while (postfixs.TryDequeue(out var unit))
        {
            if (!unit.IsOperator)
            {
                stack.Push(unit);
                continue;
            }

            var right = stack.Pop().Value;
            var left = stack.Pop().Value;
            switch (unit.Operator)
            {
                case '*':
                    stack.Push(new ExpUnit { Value = left * right });
                    break;
                case '/':
                    stack.Push(new ExpUnit { Value = left / right });
                    break;
                case '%':
                    stack.Push(new ExpUnit { Value = left % right });
                    break;
                case '+':
                    stack.Push(new ExpUnit { Value = left + right });
                    break;
                case '-':
                    stack.Push(new ExpUnit { Value = left - right });
                    break;
                case '<':
                    stack.Push(new ExpUnit { Value = (int)left << (int)right });
                    break;
                case '>':
                    stack.Push(new ExpUnit { Value = (int)left >> (int)right });
                    break;
                case '&':
                    stack.Push(new ExpUnit { Value = left & right });
                    break;
                case '^':
                    stack.Push(new ExpUnit { Value = left ^ right });
                    break;
                case '|':
                    stack.Push(new ExpUnit { Value = left | right });
                    break;
            }
        }
        #endregion

        return stack.Pop().Value;
    }

    private static int GetPrecedence(char op)
    {
        return op switch
        {
            '(' or ')' => 0,
            '~' => 1,
            '*' or '/' => 2,
            '%' => 3,
            '+' or '-' => 4,
            '<' or '>' => 5,
            '&' => 6,
            '^' => 7,
            '|' => 8,
            _ => throw new NotSupportedException($"Operator '{op}' not supported!"),
        };
    }
}
