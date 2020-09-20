using System.Data;

namespace InfraDalContracts
{
    public interface IInfraDal
    {
        IDbConnection Connect(string strConnection);

        DataSet ExecuteSpQuery(IDbConnection connection, string spName,
            params IDbParameter[] parameters);

        DataSet ExecuteQuery(IDbConnection connection, string query);

        IDbParameter GetParameter(string parameterName, object value);

        IDbParameter GetOutParameter(string parameterName);
    }
}
