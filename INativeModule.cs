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
        /// <summary>
        /// Module's Name(Key),FunList(Value) Dic
        /// </summary>
        abstract static string Name { get; }


    }
}
