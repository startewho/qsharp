using QuickJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSharp
{

    /// <summary>
    /// Native Module Interface
    /// </summary>
    internal interface INativeModule
    {

        /// <summary>
        /// Module name,used by the export
        /// </summary>
        static string Name { get; }

        /// <summary>
        /// FunctionList
        /// exprort function list
        /// </summary>
        static abstract List<JSCFunctionListEntry> FunctionList { get; }
    }
}
