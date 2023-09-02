
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace QuickJs
{
    public unsafe partial struct JSContext
    {
        public unsafe void ThrowPendingException()
        {

            fixed (JSContext* @this = &this)
            {
                if (@this == null)
                {
                    return;
                }
                else
                {
                    JSValue exceptionVal = NativeMethods.JS_GetException(@this);
                    if (exceptionVal.Tag == JSTag.Null)
                        return;

                    try
                    {
                        if (ErrorInfo.TryCreate(@this, exceptionVal, out ErrorInfo errorInfo))
                            throw new QuickJSException(errorInfo);

                        throw new QuickJSException(exceptionVal.ToString(@this));
                    }
                    finally
                    {
                        JS_FreeValue(@this, exceptionVal);
                    }
                }

            }

        }


        /// <summary>
		/// Decrements the reference count on the specified <see cref="JSValue"/>.
		/// </summary>
		/// <param name="ctx">A pointer to the context that <see cref="JSValue"/> <paramref name="v"/> belongs to.</param>
		/// <param name="v">The <see cref="JSValue"/> to free.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void JS_FreeValue(JSContext* ctx, JSValue v)
        {
            if ((uint)v.tag >= unchecked((uint)JSTag.First))
            {
                JSRefCountHeader* p = (JSRefCountHeader*)v.u.ptr;
                if (--p->ref_count <= 0)
                {
                    //Todo Gc
                    //__JS_FreeValue(ctx, v);
                }
            }
        }
    }
}
