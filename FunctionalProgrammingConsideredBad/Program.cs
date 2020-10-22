using System;

namespace FunctionalProgrammingConsideredBad
{
    class Result
    {
        public decimal Overdraft { get; set; }
    }
    class ErrorResult
    {
        public int ErrorCode { get; set; }
        public Exception ex { get; set; }
    }

    class BusinessService
    {
        public Any<Result, ErrorResult> EvaluateAccountOverdraft(int userId)
        {
            if (userId < 0)
            {
                return new Any<Result, ErrorResult>(new ErrorResult { ErrorCode = 42, ex = new ArgumentOutOfRangeException() });
            }

            return new Any<Result, ErrorResult>(new Result { Overdraft = userId * 100 });
        }
    }

    public static class MyExtensions
    {
        public static T Then<T>(this T o, Action<T> action)
        {
            action(o);

            return o;
        }
    }

    public static class AnyExtensions
    {
        public static Any<TLeftOut, TRightOut> Then<TLeft, TRight, TLeftOut, TRightOut>(this Any<TLeft, TRight> @any, Func<TLeft, TLeftOut> leftFunc, Func<TRight, TRightOut> rightFunc)
        {
            if (@any.IsLeft)
            {
                return new Any<TLeftOut, TRightOut>(leftFunc(@any.TL));
            }
            else
            {
                return new Any<TLeftOut, TRightOut>(rightFunc(@any.TR));
            }
        }
        public static T Match<TLeft, TRight, T>(this Any<TLeft, TRight> @any, Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
        {
            if (@any.IsLeft)
            {
                return leftFunc(@any.TL);
            }
            else
            {
                return rightFunc(@any.TR);
            }
        }
    }

    public sealed class Any<TLeft, TRight>
    {
        public TLeft TL { get; }
        public TRight TR { get; }

        public bool IsLeft { get; set; }

        public Any(TLeft left)
        {
            TL = left;
            IsLeft = true;
        }

        public Any(TRight right)
        {
            TR = right;
            IsLeft = false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var s = new BusinessService();

            for (var i = -1; i <= 1; ++i)
            {
                var result = s.EvaluateAccountOverdraft(i)
                    .Match(
                    success => "AccountBalance: " + success.Overdraft,
                    error => "Error: " + error.ErrorCode
                    ).Then(Console.WriteLine);

            }
        }
    }
}
