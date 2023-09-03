
using QuickJs;
using System.Reflection;
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
                NativeMethods.js_std_init_handlers(runtime);
                JSContext* ctx = NativeMethods.JS_NewContext(runtime);
                if (ctx != null)
                {
                    byte** argv = Utils.CreateArgv(Encoding.UTF8, args);
                    try
                    {
                        NativeMethods.js_std_add_helpers(ctx, args.Length - 1, argv);
                    }
                    finally
                    {
                        Utils.ReleaseArgv(new IntPtr(argv));
                    }

                    // var intPtr = (delegate* unmanaged<JSContext*, byte*, void*, JSModuleDef*>)(&NativeMethods.js_module_loader);
                    //NativeMethods.JS_SetModuleLoaderFunc(runtime, null, &NativeMethods.js_module_loader, null);

                    JSMemoryUsage usage = new JSMemoryUsage();
                    NativeMethods.JS_ComputeMemoryUsage(runtime, &usage);

                    fixed (byte* buffer = "os"u8)
                    {
                        NativeMethods.js_init_module_os(ctx, buffer);
                    }

                    fixed (byte* buffer = "std"u8)
                    {
                        NativeMethods.js_init_module_std(ctx, buffer);
                    }

                    HelloModule.Init(ctx);

                    var filename = "<anonymous>"u8;
                    var input = "function add(a,b){return a+b}; let c=add(1,2);console.log(c);"u8;

                    fixed (byte* buffer = input)
                    {
                        fixed (byte* file = filename)
                        {
                            JSValue val = NativeMethods.JS_Eval(ctx, buffer, (nuint)input.Length, file, 0);
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
                        var val = NativeMethods.JS_Eval(ctx, buffer, (nuint)input2.Length, pfile, 1);
                        if (val.tag != 2)
                        {
                            ctx->ThrowPendingException();
                        }

                    }


                }

            }


            Console.ReadKey();
        }

        public static unsafe void SetDefaultModuleLoader(JSRuntime* runtime)
        {
            IntPtr moduleHandle, fn_js_module_loader;
            const string fnModuleLoaderName = "csbindgen_js_module_loader";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                moduleHandle = NativeMethods.GetModuleHandle("quickjs");
                if (moduleHandle == IntPtr.Zero)
                    throw new InvalidOperationException();
                fn_js_module_loader = NativeMethods.GetProcAddress(moduleHandle, fnModuleLoaderName);
            }
            else
            {
                const int RTLD_NOW = 2;
                moduleHandle = NativeMethods.dlopen("libquickjs.so", RTLD_NOW);
                if (moduleHandle == IntPtr.Zero)
                    throw new InvalidOperationException();
                fn_js_module_loader = NativeMethods.GetProcAddress(moduleHandle, fnModuleLoaderName);
                NativeMethods.dlclose(moduleHandle);
            }

            if (fn_js_module_loader == IntPtr.Zero)
                throw new EntryPointNotFoundException("Unable to find an entry point name '" + fnModuleLoaderName + "'.");
            NativeMethods.JS_SetModuleLoaderFunc(runtime, null, (delegate* unmanaged[Cdecl]<JSContext*, byte*, void*, JSModuleDef*>)fn_js_module_loader, null);
        }



        private unsafe static void PrintHello(JSContext* ctx)
        {
            JSValue globalObj = NativeMethods.JS_GetGlobalObject(ctx);
            JSValue obj = NativeMethods.JS_NewObject(ctx);
            //NativeMethods.JS_DefinePropertyValueStr(ctx, obj, "hello", new JSValue(), 7);

            GC.KeepAlive(obj);
        }

        private unsafe static JSValue Hello(JSContext ctx, JSValue thisArg, int argc, JSValue[] argv, int magic, JSValue* data)
        {
            //string name = argc > 0 ? argv[0].ToString(ctx) : "anonymous";
            //Console.WriteLine($"Hello, {name}!");
            //return JSValue.Undefined;
            return new JSValue();
        }

        private unsafe static JSValue Hello2(JSContext ctx, JSValue thisArg, int argc, JSValue[] argv)
        {
            return Hello(ctx, thisArg, argc, argv, 0, null);
        }

        private unsafe static JSValue Hello3(JSContext ctx, JSValue thisArg, JSValue[] argv, int magic, JSValue[] data)
        {
            fixed (JSValue* fnData = data)
            {
                return Hello(ctx, thisArg, argv.Length, argv, magic, fnData);
            }
        }
    }
}
