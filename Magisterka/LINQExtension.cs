using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public static class LINQExtension  
    {
        public static int MaxIndex(this IEnumerable<int> sequence)
        {
            if (sequence.Count() == 0) throw new InvalidOperationException("Collection is empty");
            int maxValue = sequence.Max();
            int index = sequence.First(item => item == maxValue);
            return index;
        }
    }
}
