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

internal class HelloModule : DefaulModule
{

    private static IntPtr PFunList = IntPtr.Zero;

    public unsafe static Dictionary<string, JSValue> ValueList => new Dictionary<string, JSValue>();



    public unsafe static List<JSCFunctionListEntry> FunctionList => new List<JSCFunctionListEntry>() {
        JSCFunctionListEntry.CreateFuction(new JSCFunctionType(){generic=&Add},nameof(Add),2),
        JSCFunctionListEntry.CreateFuction(new JSCFunctionType(){generic=&Sub},nameof(Sub),2),
    };

    
    public static string MoudleName()
    {
        return "hello";
    }

    public unsafe static JSModuleDef* Init(JSContext* context)
    {
        return Init(context, ValueList.Keys.ToList(), &js_hello_module_init);
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
            if (JS_ToInt32(ctx, &b, argv[1]) == 0)
            {
                var c = a + b;

                v.u.int32 = c;
            }

        }
        return v;
    }


    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private unsafe static JSValue Sub(JSContext* ctx, JSValue thisArg, int argc, JSValue* argv)
    {
        int a;
        int b;
        var v = new JSValue();
        v.u.int32 = 0;
        v.tag = 0;
        if (JS_ToInt32(ctx, &a, argv[0]) == 0)
        {
            if (JS_ToInt32(ctx, &b, argv[1]) == 0)
            {
                v.u.int32 = a - b;
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
                    r = JS_SetModuleExportList(ctx, m, (JSCFunctionListEntry*)PFunList, FunctionList.Count);

                }
            }

        }

        return r;
    }





}
