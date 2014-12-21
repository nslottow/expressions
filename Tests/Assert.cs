using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tests
{
    public class Assert
    {
        public static void Equal<T>(T actual, T expected, string message = null)
        {
            if (!actual.Equals(expected))
            {
                if (message == null)
                {
                    message = string.Format("Assert.Equal failed: Expected: {0}, Actual: {1}", expected, actual);
                }

                throw new FailureException(message);
            }
        }
    }

    public class FailureException : Exception
    {
        public FailureException(string message) : base(message)
        {
        }
    }
}
