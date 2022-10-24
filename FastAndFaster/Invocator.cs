using FastAndFaster.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastAndFaster
{
    public static class Invocator
    {
        /// <summary>
        /// Sliding cache expiration time for delegates to invoke methods.
        /// The unit is second and the default value is 43,200 (12 hours).
        /// </summary>
        public static int SlidingExpirationInSecs { get; set; } = 12 * 3600;

        private const byte METHOD_LOAD_INDEX = 1;

        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Create a delegate to invoke a public non-static method that returns void.
        /// </summary>
        /// <param name="typeName">
        /// The assembly qualified name of the target class.
        /// </param>
        /// <param name="methodName">
        /// The name of the target method.
        /// </param>
        /// <param name="parameterTypes">
        /// The list of the method's parameter types. For a parameterless method, this value can be omitted.
        /// </param>
        /// <param name="genericInfo">
        /// The list of concrete types and index of generic parameter types in the target method.
        /// For a non-generic method, this parameter is omitted.
        /// </param>
        /// <returns>
        /// A delegate to invoke the target method. The delegate type is Action<object, object[]>.
        /// </returns>
        public static Action<object, object[]> CreateAction(
            string typeName, string methodName, Type[] parameterTypes = null, GenericInfo genericInfo = null)
        {
            if (parameterTypes is null)
            {
                parameterTypes = Type.EmptyTypes;
            }
            var key = (typeName, methodName, TypeHelper.GetParameterTypesIdentity(parameterTypes, genericInfo));

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(SlidingExpirationInSecs);

                var type = TypeHelper.GetTypeByName(typeName);
                var methodInfo = TypeHelper.GetMethodInfoByName(type, methodName, parameterTypes, genericInfo);

                var delegateParameterTypes = new[] { typeof(object), typeof(object[]) };
                var dynInvoc = new DynamicMethod(
                    $"{type.FullName}_{methodInfo.Name}_{TypeHelper.GetParameterTypesIdentity(parameterTypes, genericInfo)}_Invoc",
                    null, delegateParameterTypes, true);

                GenerateIL(dynInvoc, type, methodInfo, parameterTypes);

                return (Action<object, object[]>)dynInvoc
                    .CreateDelegate(typeof(Action<object, object[]>));
            });
        }

        /// <summary>
        /// Create a delegate to invoke a public non-static method that returns some result.
        /// </summary>
        /// <param name="typeName">
        /// The assembly qualified name of the target class.
        /// </param>
        /// <param name="methodName">
        /// The name of the target method.
        /// </param>
        /// <param name="parameterTypes">
        /// The list of the method's parameter types. For a parameterless method, this value can be omitted.
        /// </param>
        /// <param name="genericInfo">
        /// The list of concrete types and index of generic parameter types in the target method.
        /// For a non-generic method, this parameter is omitted.
        /// </param>
        /// <returns>
        /// A delegate to invoke the target method. The delegate type is Func<object, object[], object>.
        /// </returns>
        public static Func<object, object[], object> CreateFunc(
            string typeName, string methodName, Type[] parameterTypes = null, GenericInfo genericInfo = null)
        {
            if (parameterTypes is null)
            {
                parameterTypes = Type.EmptyTypes;
            }
            var key = (typeName, methodName, TypeHelper.GetParameterTypesIdentity(parameterTypes, genericInfo));

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(SlidingExpirationInSecs);

                var type = TypeHelper.GetTypeByName(typeName);
                var methodInfo = TypeHelper.GetMethodInfoByName(type, methodName, parameterTypes, genericInfo);

                var delegateParameterTypes = new[] { typeof(object), typeof(object[]) };
                var dynInvoc = new DynamicMethod(
                    $"{type.FullName}_{methodInfo.Name}_{TypeHelper.GetParameterTypesIdentity(parameterTypes, genericInfo)}_Invoc",
                    typeof(object), delegateParameterTypes, true);

                GenerateIL(dynInvoc, type, methodInfo, parameterTypes);

                return (Func<object, object[], object>)dynInvoc
                    .CreateDelegate(typeof(Func<object, object[], object>));
            });
        }

        private static void GenerateIL(
            DynamicMethod dynInvoc, Type type, MethodInfo methodInfo, Type[] parameterTypes)
        {
            var il = dynInvoc.GetILGenerator();

            if (methodInfo.IsStatic)
            {
                // For a static method, there is no class instance to load to the stack.
                // We can skip calling LoadTarget.
                // Moreover, a static method cannot be abstract nor virtual.
                // Because of that, we can emit OpCode "Call" instead of "Callvirt" 
                IlHelper.LoadArguments(il, METHOD_LOAD_INDEX, parameterTypes);
                IlHelper.ExecuteMethod(il, methodInfo);
            }
            else
            {
                IlHelper.LoadTarget(il, type);
                IlHelper.LoadArguments(il, METHOD_LOAD_INDEX, parameterTypes);
                IlHelper.ExecuteMethod(il, methodInfo);
            }
        }
    }
}
