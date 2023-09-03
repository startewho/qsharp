using QuickJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickJs
{
    public static unsafe partial class NativeMethods
    {
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
                    __JS_FreeValue(ctx, v);
                }
            }
        }



        /// <summary>
        /// Assigns a value to a property of an object.
        /// </summary>
        /// <param name="ctx">A pointer to the JavaScript context.</param>
        /// <param name="this_obj">Object to which the property to set belongs.</param>
        /// <param name="prop">The property to set.</param>
        /// <param name="val">The value to assign to the property.</param>
        /// <returns>-1 in case of exception or TRUE (1) or FALSE (0).</returns>
        /// <remarks>
        /// Warning: <paramref name="val"/> is freed by the function.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int JS_SetProperty(JSContext* ctx, JSValue this_obj, JSAtom prop, JSValue val)
        {
            return JS_SetPropertyInternal(ctx, this_obj, prop.ToUInt32(), val, (int)JSPropertyFlags.Throw);
        }


        /// <summary>
		/// Finds a specified property and retrieve its value.
		/// </summary>
		/// <param name="ctx">A pointer to the JavaScript context.</param>
		/// <param name="this_obj">An object to search on for the property.</param>
		/// <param name="prop">The name of the property to look up.</param>
		/// <returns>
		/// The current value of the property, or <see cref="JSValue.Undefined"/> if no such property is found.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue JS_GetProperty(JSContext* ctx, [In] JSValue this_obj, JSAtom prop)
        {
            return JS_GetPropertyInternal(ctx, this_obj, prop.ToUInt32(), this_obj, (int)JSPropertyFlags.Throw);
        }



    }
}

