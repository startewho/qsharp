using QuickJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QuickJs.NativeMethods;
namespace QSharp
{

    /// <summary>
    /// Native Module Interface
    /// </summary>
    internal interface INativeModule
    {
        abstract static IntPtr PFunList { get; set; }
        /// <summary>
        /// Module name,used by the export
        /// </summary>
        abstract static string MoudleName();
      

        /// <summary>
        /// FunctionList
        /// exprort function list
        /// </summary>
        abstract static List<JSCFunctionListEntry> FunctionList { get; }



    }
}
