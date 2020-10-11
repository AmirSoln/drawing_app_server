namespace DrawingContracts.Dto.Markers
{
    public class EditMarkerResponseOk:EditMarkerResponse
    {
        public EditMarkerResponseOk(EditMarkerRequest request)
        {
            Request = request;
        }

        public EditMarkerRequest Request { get; set; }
    }
}
