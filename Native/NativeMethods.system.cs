using System;
using System.Runtime.InteropServices;

namespace QuickJs
{
    public static unsafe partial class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [DllImport("libdl", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport("libdl", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern int dlclose(IntPtr handle);


        /// <summary>
        /// Set Deault Js moudle(support Es6 module)
        /// </summary>
        /// <param name="runtime"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="EntryPointNotFoundException"></exception>
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



    }
}
