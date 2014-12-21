using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expressions.Tests
{
    public class FactAttribute : Attribute
    {
        public string Ignore { get; private set; }
        
        public FactAttribute(string ignore = null)
        {
            Ignore = ignore;
        }
    }
}
