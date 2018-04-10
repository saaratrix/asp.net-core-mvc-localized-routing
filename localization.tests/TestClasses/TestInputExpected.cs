using System;
using System.Collections.Generic;
using System.Text;

namespace localization.tests.TestClasses
{
    class TestInputExpected
    {
        public object Input { get; set; }
        public object Expected { get; set; }

        public TestInputExpected()
        {
           
        }

        public TestInputExpected(object a_input, object a_expected)
        {
            Input = a_input;
            Expected = a_expected;
        }
    }
}
