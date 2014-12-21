using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tests
{
    public class Result
    {
        public string Name { get; private set; }
        public bool Passed { get; private set; }
        public string Message { get; private set; }

        public Result(string name, bool passed, string message = null)
        {
            Name = name;
            Passed = passed;
            Message = message;
        }
    }

    public class Runner
    {
        private List<Type> m_testSuites = new List<Type>();

        public event Action<string> TestStarted;
        public event Action<Result> TestFinished;

        public void AddTestSuite(Type suiteType)
        {
            m_testSuites.Add(suiteType);
        }

        public void Run()
        {
            foreach (var suite in m_testSuites)
            {
                var testMethods = suite.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var testMethod in testMethods)
                {
                    if (testMethod.CustomAttributes.Where(a => a.AttributeType == typeof(FactAttribute)).FirstOrDefault() != null)
                    {
                        string testName = suite.FullName + '.' + testMethod.Name;
                        if (TestStarted != null)
                        {
                            TestStarted(testName);
                        }

                        Console.Write("Running {0} ... ", testName);
                        bool passed = true;
                        string message = null;

                        try
                        {
                            testMethod.Invoke(null, null);
                        }
                        catch (Exception e)
                        {
                            passed = false;
                            message = string.Format("{0}  {1}{2}{3}", Environment.NewLine, e.InnerException.Message, Environment.NewLine, e.InnerException.StackTrace);
                        }

                        if (passed)
                        {
                            Console.WriteLine("passed");
                        }
                        else
                        {
                            Console.WriteLine("failed: {0}", message);
                        }

                        if (TestFinished != null)
                        {
                            TestFinished(new Result(testName, passed, message));
                        }
                    }
                }
            }
        }
    }
}
