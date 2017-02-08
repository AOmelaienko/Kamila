using System.ComponentModel.DataAnnotations;


namespace System.Reflection {
    public static partial class Extensions {
        public static object Invoke ( this MethodBase MethodBase, object Object, params object[] Parameters ) {
            return MethodBase.Invoke ( Object, Parameters );
        }
        private static object Invoke ( this MethodBase MethodBase, object Object, Type[] GenericParameters, object[] Parameters ) {
            if ( MethodBase.IsGenericMethodDefinition )
                return ( (MethodInfo) MethodBase ).MakeGenericMethod ( GenericParameters ).Invoke ( Object, Parameters );
            else
                return MethodBase.Invoke ( Object, Parameters );
        }
    }
}
