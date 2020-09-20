using InfraDalContracts;
using Oracle.ManagedDataAccess.Client;

namespace InfraDal
{
    public class OracleConnectionAdapter:IDbConnection
    {
        public OracleConnection Connection { get; }
        public OracleConnectionAdapter(string strConnection)
        {
            Connection = new OracleConnection(strConnection);
        }
    }
}
