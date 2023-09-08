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

internal class DefaultModuleManager
{

    // Module's Name(Key),FunList(Value) Dic
    public static Dictionary<string, nint> ModuleList { get; private set; }

    static DefaultModuleManager()
    {
        ModuleList = new Dictionary<string, nint>();
    }



    public unsafe static JSModuleDef* Init(JSContext* context, string moduleName, List<JSCFunctionListEntry> funcList, List<string> exportValue, delegate* unmanaged[Cdecl]<JSContext*, JSModuleDef*, int> initDelegate)
    {
        var r = 0;

        if (ModuleList.ContainsKey(moduleName))
        {
            throw new Exception($"{moduleName}has inited,disallow init twice!");
        }
        else
        {
            var nameStr = Encoding.UTF8.GetBytes(moduleName);
            fixed (byte* n = nameStr)
            {
                var m = JS_NewCModule(context, n, initDelegate);
               
                var count = funcList.Count;
                if (count > 0)
                {
                    var size = Marshal.SizeOf<JSCFunctionListEntry>();
                    var pFucList = Marshal.AllocHGlobal(size * count);
                    var start = pFucList;
                    for (int i = 0; i < count; i++)
                    {
                        var entry = funcList[i];
                        Marshal.StructureToPtr(entry, start, false);
                        start += size;
                    }
                    r = JS_AddModuleExportList(context, m, (JSCFunctionListEntry*)pFucList, count);
                    if (r != 0)
                    {
                        throw new Exception($"添加导出方法出错!");
                    }
                    ModuleList.TryAdd(moduleName, pFucList);
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







}
