namespace DrawingContracts.Dto.SocketDto
{
    public class BaseDto
    {
        public BaseDto(string type)
        {
            DtoType = type;
        }

        public string DtoType { get; set; }
        public object Data { get; set; }

    }
}
