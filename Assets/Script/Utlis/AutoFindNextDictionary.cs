using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;

namespace Assets.Script.Utlis
{
    public class AutoFindNextDictionary<V> : Dictionary<uint,V>    
    {
        uint lastElementCount = 1;
        public new void Add(uint key,V value)
        {
            base.Add(key, value);
        }
        public uint Add(V value)
        {
            Add(lastElementCount, value);
            lastElementCount++; 
            return lastElementCount;
        }
    }
}
