using System;
using System.Data;

namespace Kamila {

    public static partial class Extensions {
        public static IDataParameter SetParameter ( this IDbCommand Command, string Name = null, int? Index = null, DbType? Type = null, ParameterDirection? Direction = null, object Value = null ) {
            if ( !string.IsNullOrWhiteSpace ( Name ) && Index.HasValue )
                throw new ArgumentException ( "Parameter cannot be referenced both by name and an index." );
            if ( Index.HasValue && Index.Value > Command.Parameters.Count )
                throw new IndexOutOfRangeException ( "Index exceeds existing parameter count by more than 1." );
            IDataParameter result = null;
            if ( !string.IsNullOrWhiteSpace ( Name ) ) {
                if ( Command.Parameters.Contains ( Name ) ) {
                    if ( Command.Parameters[Name] is IDataParameter )
                        result = Command.Parameters[Name] as IDataParameter;
                    else
                        Command.Parameters.Remove ( Name );
                }
                if ( null == result ) {
                    result = Command.CreateParameter ();
                    result.ParameterName = Name;
                    Command.Parameters.Add ( result );
                }
            } else if ( Index.HasValue ) {
                if ( Command.Parameters[Index.Value] is IDataParameter )
                    result = Command.Parameters[Index.Value] as IDataParameter;
                else
                    result = (IDataParameter) ( Command.Parameters[Index.Value] = Command.CreateParameter () );
            } else {
                result = Command.CreateParameter ();
                Command.Parameters.Add ( result );
            }
            if ( Direction.HasValue )
                result.Direction = Direction.Value;
            if ( Type.HasValue )
                result.DbType = Type.Value;
            result.Value = null != Value ? Value : DBNull.Value;
            return result;
        }
    }

}