using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Script.Utlis
{
    public class TrackingDictonary<Tkey, Tvalue> : Dictionary<Tkey,Tvalue>
    {
        /// <summary>
        /// On Dictionary add
        /// </summary>
        public Action<Tkey,Tvalue> onAdd;
        /// <summary>
        /// On dictionary remove
        /// </summary>
        public Action<Tkey> onRemove;
        /// <summary>
        /// On Dictionary Change posiition
        /// </summary>
        public Action<Tkey,Tvalue,Tvalue> onChange;
        /// <summary>
        /// on dictionary clear
        /// </summary>
        public Action OnClear;
        public new Tvalue this[Tkey a]
        {
            get 
            {
                return base[a];
            } 
            set 
            {
                var oldvalue = base[a];
                if (onChange != null)

                    onChange(a,oldvalue,value);
                base[a] = value;
            }
        }
        public new void Add(Tkey a,Tvalue b)
        {
            base.Add(a, b);
            if (onAdd != null)  
            onAdd(a, b);
        }
        public new void Remove(Tkey a)
        {
            base.Remove(a);
            if (onRemove != null)

                onRemove(a);
        }
        public new void Clear()
        {
            base.Clear();
            if (OnClear != null)

                OnClear();
        }
    }
}
