using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastAndFaster.Helpers
{
    internal static class IlHelper
    {
        internal static void LoadTarget(ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
        }

        internal static void LoadArguments(ILGenerator il, byte loadIndex, Type[] parameterTypes)
        {
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_S, loadIndex);
                il.Emit(OpCodes.Ldc_I4_S, i);
                il.Emit(OpCodes.Ldelem_Ref);

                if (parameterTypes[i].IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, parameterTypes[i]);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, parameterTypes[i]);
                }
            }
        }

        internal static void CreateInstance(ILGenerator il, ConstructorInfo ctorInfo)
        {
            il.Emit(OpCodes.Newobj, ctorInfo);
            il.Emit(OpCodes.Ret);
        }

        internal static void ExecuteMethod(ILGenerator il, MethodInfo methodInfo)
        {
            var needVirtualCall = methodInfo.IsAbstract || methodInfo.IsVirtual;
            var callOpCode = needVirtualCall ? OpCodes.Callvirt : OpCodes.Call;
            il.EmitCall(callOpCode, methodInfo, null);

            if (methodInfo.ReturnType != typeof(void) && methodInfo.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, methodInfo.ReturnType);
            }
            il.Emit(OpCodes.Ret);
        }
    }
}
