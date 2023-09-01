
using QuickJs;
using System.Reflection;
using System.Text;

namespace QSharp
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var n = NativeMethods.abs(-5);
            var m= NativeMethods.pow(n, 2);

            Console.WriteLine($"m={m},n={n}");
            Console.ReadKey();
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
