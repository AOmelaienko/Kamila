using Kamila;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Data {
    public static partial class Extensions {
        private const string KeyCacheDataReaderGetMethods = "DataReaderGetMethods";
        private const string NameTemplateMethodGet = "Get{0}";
        private const string NameMethodGetValue = "GetValue";
        private static Dictionary<Type, MethodBase> CreateDataRecordGetMethodDictionary () {
            Dictionary<Type, MethodBase> result = new Dictionary<Type, MethodBase> ();
            Parallel.ForEach (
                typeof ( IDataReader )
                    .GetMethods ( BindingFlags.Instance | BindingFlags.Public )
                    .Where ( m =>
                        m.Name == string.Format ( NameTemplateMethodGet, m.ReturnType.Name )
                        || ( m.Name == NameMethodGetValue && m.ReturnType == typeof ( object ) )
                    )
                , m => result.Add ( m.ReturnType, m )
            );
            return result;
        }
        private static MethodBase GetDataReaderGetMethod<T> () {
            Dictionary<Type, MethodBase> result = Cache.Get ( KeyCacheDataReaderGetMethods ) as Dictionary<Type, MethodBase>;
            if ( null == result ) {
                result = CreateDataRecordGetMethodDictionary ();
                Cache.Add ( KeyCacheDataReaderGetMethods, result );
            }
            return result.ContainsKey ( typeof ( T ) ) ? result[typeof ( T )] : null;
        }
        private static bool InvokeDataRecordGetMethod<T> ( this IDataRecord DataRecord, int Ordinal, object Result ) {
            MethodBase method = GetDataReaderGetMethod<T> ();
            if ( null == method )
                return false;
            Result = method.Invoke ( DataRecord, Ordinal );
            return true;
        }
        public static TResult Get<T, TResult> ( this IDataRecord DataRecord, int Ordinal, Func<T, TResult> Formatter, TResult Default = default ( TResult ) ) {
            if ( DataRecord.IsDBNull ( Ordinal ) )
                return Default;
            object value = DataRecord.GetValue ( Ordinal );
            if ( value is T || DataRecord.InvokeDataRecordGetMethod<T> ( Ordinal, value ) )
                return Formatter ( (T) value );
            return Default;
        }
        public static TResult Get<T, TResult> ( this IDataRecord DataRecord, string Name, Func<T, TResult> Formatter, TResult Default = default ( TResult ) ) {
            return DataRecord.Get<T, TResult> ( DataRecord.GetOrdinal ( Name ), Formatter, Default );
        }
        public static T Get<T> ( this IDataRecord DataRecord, int Ordinal, T Default = default ( T ) ) {
            return DataRecord.Get<T, T> ( Ordinal, x => x, Default );
        }
        public static T Get<T> ( this IDataRecord DataRecord, string Name, T Default = default ( T ) ) {
            return DataRecord.Get<T, T> ( DataRecord.GetOrdinal ( Name ), x => x, Default );
        }
        public static string Get<T> ( this IDataRecord DataRecord, int Ordinal, IFormatProvider FormatProvider, string Format, string Default = default ( string ) ) {
            return DataRecord.Get<T, string> ( Ordinal, x => string.Format ( FormatProvider, string.Format ( "{{0:{0}}}", Format ), x ), Default );
        }
        public static string Get<T> ( this IDataRecord DataRecord, string Name, IFormatProvider FormatProvider, string Format, string Default = default ( string ) ) {
            return DataRecord.Get<T, string> ( DataRecord.GetOrdinal ( Name ), x => string.Format ( FormatProvider, string.Format ( "{{0:{0}}}", Format ), x ), Default );
        }
        public static string Get<T> ( this IDataRecord DataRecord, int Ordinal, string Format, string Default = default ( string ) ) {
            return DataRecord.Get<T, string> ( Ordinal, x => string.Format ( CultureInfo.CurrentCulture, string.Format ( "{{0:{0}}}", Format ), x ), Default );
        }
        public static string Get<T> ( this IDataRecord DataRecord, string Name, string Format, string Default = default ( string ) ) {
            return DataRecord.Get<T, string> ( DataRecord.GetOrdinal ( Name ), x => string.Format ( CultureInfo.CurrentCulture, string.Format ( "{{0:{0}}}", Format ), x ), Default );
        }
        
    }

}