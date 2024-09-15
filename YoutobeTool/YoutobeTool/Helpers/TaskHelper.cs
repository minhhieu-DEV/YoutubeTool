using System;
using System.Collections.Generic;
using System.Linq;

namespace YoutobeTool.Helpers
{
    public class TaskHelper<T>
    {
        public static IEnumerable<T[]> SplitList(IEnumerable<T> list, int number)
        {
            int numChunks = (int)Math.Ceiling((double)list.Count() / number);
            List<T[]> subarrays = new List<T[]>();
            for (int i = 0; i < numChunks; i++)
            {
                subarrays.Add(list.Skip(i * number).Take(number).ToArray());
            }
            return subarrays;
        }
    }
}
