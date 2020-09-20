namespace InfraDalContracts
{
    public interface IDbParameter
    {
        string ParameterName { get; set; }
        object Value{ get; set; }
    }
}
