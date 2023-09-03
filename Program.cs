
using QuickJs;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static QuickJs.NativeMethods;

namespace QSharp
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.UTF8;

            JSRuntime* runtime = JS_NewRuntime();
            if (runtime != null)
            {

                SetDefaultModuleLoader(runtime);
                js_std_init_handlers(runtime);
                JSContext* ctx = JS_NewContext(runtime);
                if (ctx != null)
                {
                    byte** argv = Utils.CreateArgv(Encoding.UTF8, args);
                    try
                    {
                        js_std_add_helpers(ctx, args.Length - 1, argv);
                    }
                    finally
                    {
                        Utils.ReleaseArgv(new IntPtr(argv));
                    }


                    JSMemoryUsage usage = new JSMemoryUsage();
                    JS_ComputeMemoryUsage(runtime, &usage);

                    fixed (byte* buffer = "os"u8)
                    {
                        js_init_module_os(ctx, buffer);
                    }

                    fixed (byte* buffer = "std"u8)
                    {
                        js_init_module_std(ctx, buffer);
                    }

                    PrintHello(ctx);

                    HelloModule.Init(ctx);



                    var filename = "<anonymous>"u8;
                    var input = "function add(a,b){return a+b}; let c=add(1,2);console.log(c);console.log(Hello())"u8;

                    fixed (byte* buffer = input)
                    {
                        fixed (byte* file = filename)
                        {
                            JSValue val = JS_Eval(ctx, buffer, (nuint)input.Length, file, 0);
                            if (val.tag != 2)
                            {
                                ctx->ThrowPendingException();
                            }
                        }
                    }

                    var input2 = File.ReadAllBytes("Test/test.js");//UTF8编码
                    filename = "Test/test.js"u8;

                    fixed (byte* buffer = input2)
                    fixed (byte* pfile = filename)
                    {
                        var val = JS_Eval(ctx, buffer, (nuint)input2.Length, pfile, 1);
                        if (val.tag != 2)
                        {
                            ctx->ThrowPendingException();
                        }

                    }


                }

            }


            Console.ReadKey();
        }



        private unsafe static void PrintHello(JSContext* ctx)
        {
            JSValue globalObj = JS_GetGlobalObject(ctx);
            JSValue obj = JS_NewObject(ctx);
            var fun1 = JS_NewCFunctionData(ctx, &Hello, 0, 0, 0, null);
            fixed (byte* name = Utils.StringToManagedUTF8(nameof(Hello)))
            {
                JS_DefinePropertyValueStr(ctx, globalObj, name, fun1, (int)JSPropertyFlags.CWE);

            }


            GC.KeepAlive(obj);
        }


        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private unsafe static JSValue Hello(JSContext* ctx, JSValue thisArg, int argc, JSValue* argv, int magic, JSValue* data)
        {

            return JSValue.Create(ctx, "Hello,GlobalThis");
        }

        //private unsafe static JSValue Hello2(JSContext ctx, JSValue thisArg, int argc, JSValue* argv)
        //{
        //    return Hello(ctx, thisArg, argc, argv, 0, null);
        //}

        //private unsafe static JSValue Hello3(JSContext* ctx, JSValue thisArg, JSValue* argv, int magic, JSValue[] data)
        //{
        //    fixed (JSValue* fnData = data)
        //    {
        //        return Hello(ctx, thisArg, argv.Length, argv, magic, fnData);
        //    }
        //}
    }
}
