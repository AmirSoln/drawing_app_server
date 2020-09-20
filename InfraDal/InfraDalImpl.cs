using System.Data;
using InfraDalContracts;
using Oracle.ManagedDataAccess.Client;
using IDbConnection = InfraDalContracts.IDbConnection;

namespace InfraDal
{
    public class InfraDalImpl : IInfraDal
    {
        //TODO:Move to outer package
        public IDbConnection Connect(string strConnection)
        {
            return new OracleConnectionAdapter(strConnection); //After Complete ctor complete here
        }

        private DataSet getDataSet(OracleCommand command)
        {
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter(command);
            da.Fill(ds);
            return ds;
        }

        public DataSet ExecuteSpQuery(IDbConnection connection, string spName, params IDbParameter[] parameters)
        {
            var command = new OracleCommand
            {
                CommandText = spName,
                CommandType = CommandType.StoredProcedure,
                Connection = (connection as OracleConnectionAdapter).Connection
            };


            foreach (var parameter in parameters)
                command.Parameters.Add((parameter as OracleParameterAdapter).Parameter);
            return getDataSet(command);
        }

        public DataSet ExecuteQuery(IDbConnection connection, string query)
        {
            var command = new OracleCommand
            {
                CommandText = query,
                Connection = (connection as OracleConnectionAdapter).Connection
            };
            return getDataSet(command);
        }

        public IDbParameter GetParameter(string parameterName, object value)
        {
            return new OracleParameterAdapter{ Value = value, ParameterName = parameterName };
        }

        public IDbParameter GetOutParameter(string parameterName)
        {
            var param = new OracleParameterAdapter
            {
                ParameterName = parameterName,
                Parameter = {Direction = ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor}
            };
            return param;

        }
    }
}
