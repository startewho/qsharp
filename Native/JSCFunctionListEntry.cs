using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickJs
{
    public unsafe partial struct JSCFunctionListEntry
    {

        /// <summary>
        /// Create Function
        /// </summary>
        /// <param name="jSCFunctionType"></param>
        /// <param name="name"></param>
        /// <param name="paraLength"></param>
        /// <param name="magic"></param>
        /// <returns></returns>
        public static JSCFunctionListEntry CreateFuction(JSCFunctionType jSCFunctionType, string name, byte paraLength, int magic = 0)
        {
            var entry = new JSCFunctionListEntry()
            {
                name = (byte*)Marshal.StringToHGlobalAnsi(name).ToPointer(),
                prop_flags = 6,
                def_type = 0,
                magic = 0,
            };
            entry.u.func.cfunc = jSCFunctionType;
            entry.u.func.length = paraLength;
            return entry;
        }
    }
}
