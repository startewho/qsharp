using QuickJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QuickJs.NativeMethods;

namespace QSharp;

internal class DefaulModule : INativeModule
{

    public static IntPtr PFunList { get; set; }

    public unsafe static List<JSCFunctionListEntry> FunctionList => new List<JSCFunctionListEntry>();


    public static string MoudleName()
    {
        return string.Empty;
    }


    protected unsafe static JSModuleDef* Init(JSContext* context, List<string> exportValue, delegate* unmanaged[Cdecl]<JSContext*, JSModuleDef*, int> initDelegate)
    {
        var r = 0;
        var nameStr = Encoding.UTF8.GetBytes(MoudleName());
        fixed (byte* n = nameStr)
        {
            var m = JS_NewCModule(context, n, (delegate* unmanaged[Cdecl]<JSContext*, JSModuleDef*, int>)&initDelegate);

            var count = FunctionList.Count;
            if (count > 0)
            {
                PFunList = Marshal.AllocHGlobal(Marshal.SizeOf<JSCFunctionListEntry>() * count);
                var LongPtr = PFunList;
                for (int i = 0; i < count; i++)
                {
                    var entry = FunctionList[i];
                    Marshal.StructureToPtr(entry, LongPtr, false);
                    LongPtr += Marshal.SizeOf<JSCFunctionListEntry>();
                }
                r = JS_AddModuleExportList(context, m, (JSCFunctionListEntry*)PFunList, count);
                if (r != 0)
                {
                    throw new Exception($"添加导出方法出错!");
                }
            }
            exportValue.ForEach(v =>
            {
                fixed (byte* b = Utils.StringToManagedUTF8(v))
                {
                    r = JS_AddModuleExport(context, m, b);
                    if (r != 0)
                    {
                        throw new Exception($"添加{v}变量出错!");
                    }
                }
            });



            return m;

        }
    }







}
