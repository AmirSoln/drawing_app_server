using DIContracts.Dto;

namespace DrawingContracts.Dto
{
    public class AppResponseError:Response
    {
        public AppResponseError(string msg)
        {
            Message = msg; 
        }
        public string Message { get; }
    }
}
