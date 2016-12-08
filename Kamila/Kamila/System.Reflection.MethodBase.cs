namespace System.Reflection {
    public static partial class Extensions {
        public static object Invoke ( this MethodBase MethodBase, object Object, params object[] Parameters ) {
            return MethodBase.Invoke ( Object, Parameters );
        }
    }
}
