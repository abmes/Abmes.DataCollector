using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Utils
{
    public static class MathExtensions
    {
        public static bool InRange(this int value, int minimum, int maximum)
        {
            return (value >= minimum) && (value <= maximum);
        }
    }
}
