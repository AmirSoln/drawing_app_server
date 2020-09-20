using InfraDalContracts;
using Oracle.ManagedDataAccess.Client;

namespace InfraDal
{
    public class OracleParameterAdapter:IDbParameter
    {
        public OracleParameter Parameter { get; set; }

        public OracleParameterAdapter()
        {
            Parameter = new OracleParameter(); 
        }
        public string ParameterName { get => Parameter.ParameterName; set => Parameter.ParameterName = value; }
        public object Value { get => Parameter.Value; set => Parameter.Value = value; }
    }
}
