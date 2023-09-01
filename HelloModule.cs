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

internal class HelloModule
{
    private const int COUNT = 2;

    private static IntPtr funList = IntPtr.Zero;
    public unsafe static JSModuleDef* Init(JSContext* context, string name)
    {
        var r = 0;
        var nameStr = Encoding.UTF8.GetBytes(name);
        var NAME = "world"u8;
        fixed (byte* n = nameStr)
        {
            var m = JS_NewCModule(context, n, (delegate* unmanaged[Cdecl]<JSContext*, JSModuleDef*, int>)&js_hello_module_init);
            fixed (byte* b = NAME)
            {
                r = JS_AddModuleExport(context, m, b);

                if (r == 0)
                {
                    funList = Marshal.AllocHGlobal(Marshal.SizeOf<JSCFunctionListEntry>() * COUNT);
                    var LongPtr = funList;
                    for (int I = 0; I < COUNT; I++)
                    {

                        JSCFunctionListEntry entry = new JSCFunctionListEntry()
                        {
                            name = (byte*)Marshal.StringToHGlobalAnsi($"Add{I + 1}").ToPointer(),
                            prop_flags = 6,
                            def_type = 0,
                            magic = 0,
                        };
                        entry.u.func.cfunc.generic = &Add;
                        entry.u.func.length = 2;


                        Marshal.StructureToPtr(entry, LongPtr, false);
                        LongPtr += Marshal.SizeOf<JSCFunctionListEntry>();
                    }
                    r = JS_AddModuleExportList(context, m, (JSCFunctionListEntry*)funList, COUNT);

                }

            }

            return m;

        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private unsafe static JSValue Add(JSContext* ctx, JSValue thisArg, int argc, JSValue* argv)
    {
        int a;
        int b;
        var v = new JSValue();
        v.u.int32 = 0;
        v.tag = 0;
        if (JS_ToInt32(ctx, &a, argv[0]) == 0)
        {
            if (JS_ToInt32(ctx, &b, argv[1])== 0)
            {
                var c = a + b;

                v.u.int32 = c;
            }

        }
        return v;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    static unsafe int js_hello_module_init(JSContext* ctx, JSModuleDef* m)
    {
        var NAME = "world"u8;
        var VNAME = "helloworld"u8;
        var r = 0;
        fixed (byte* b = NAME)
        {
            fixed (byte* bv = VNAME)
            {
                r = JS_SetModuleExport(ctx, m, b, JS_NewString(ctx, bv));
                if (r == 0)
                {
                    r = JS_SetModuleExportList(ctx, m, (JSCFunctionListEntry*)funList, COUNT);

                }
            }

        }

        return r;
    }




}
