namespace DrawingContracts.Dto.Markers
{
    public class CreateMarkerResponseOk:CreateMarkerResponse
    {
        public CreateMarkerResponseOk(CreateMarkerRequest request)
        {
            Request = request;
        }

        public CreateMarkerRequest Request { get; set; }
    }
}
