
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using QuickJs;

namespace QSharp
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.UTF8;

            JSRuntime* runtime = NativeMethods.JS_NewRuntime();
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
                    var buffBytes = Utils.StringToManagedUTF8("os");
                    fixed (byte* buffer = buffBytes)
                    {
                        NativeMethods.js_init_module_os(ctx, buffer);
                    }
                    buffBytes = Utils.StringToManagedUTF8("std");
                    fixed (byte* buffer = buffBytes)
                    {
                        NativeMethods.js_init_module_std(ctx, buffer);
                    }

                    var filename = "<anonymous>";
                    var input = "function add(a,b){return a+b}; let c=add(1,2);console.log(c);";
                    var length = Encoding.UTF8.GetByteCount(input);

                    //JSValue val = NativeMethods.JS_Eval(ctx, input, (uint)length, filename, 1);
                    //if (val.tag!=2)
                    //{
                    //    val = NativeMethods.JS_GetException(ctx);
                    //    var name=  GetStringProperty(ctx,val, "name");
                    //    var msg = GetStringProperty(ctx, val, "message");
                    //    var stack= GetStringProperty(ctx, val, "stack");
                    //}
                    var input2 = File.ReadAllText("Test/test.js");
                    filename = "Test/test.js";
                    buffBytes = Utils.StringToManagedUTF8(input2);
                    var nameBytes = Utils.StringToManagedUTF8(filename);
                    var length2 = Encoding.UTF8.GetByteCount(input2);

                    fixed (byte* buffer = buffBytes)
                    fixed (byte* pfile = nameBytes)
                    {
                        var val = NativeMethods.JS_Eval(ctx, buffer, (nuint)length2, pfile, 1);
                        if (val.tag != 2)
                        {
                            val = NativeMethods.JS_GetException(ctx);
                            var name = GetStringProperty(ctx, val, "name");
                            var msg = GetStringProperty(ctx, val, "message");
                            var stack = GetStringProperty(ctx, val, "stack");
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

        public static unsafe string GetStringProperty(JSContext* ctx, JSValue value, string name)
        {

            try
            {
                var nameBytes = Utils.StringToManagedUTF8(name);
                fixed (byte* buffer = nameBytes)
                {
                    JSValue val = NativeMethods.JS_GetPropertyStr(ctx, value, buffer);
                    nuint length;
                    byte* p = NativeMethods.JS_ToCStringLen2(ctx, &length, val, 1);
                    if (p == null)
                        return null;
                    try
                    {
                        return Utils.PtrToStringUTF8(new IntPtr(p), (int)length);
                    }
                    finally
                    {
                        NativeMethods.JS_FreeCString(ctx, p);
                    }
                }


            }
            finally
            {
                // NativeMethods.free(ctx, val);
            }
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
