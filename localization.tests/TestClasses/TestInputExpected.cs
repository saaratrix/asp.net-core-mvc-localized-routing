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

        public TestInputExpected(object input, object expected)
        {
            Input = input;
            Expected = expected;
        }
    }
}
