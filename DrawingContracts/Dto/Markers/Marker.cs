namespace DrawingContracts.Dto.Markers
{
    public enum EMarkerType
    {
        Ellipse,
        Rectangle
    }

    public class Marker
    {
        public string DocId { get; set; }
        public string Position { get; set; }
        public string OwnerUser { get; set; }
        public EMarkerType MarkerType { get; set; }
        public string Color { get; set; }
        public string MarkerId { get; set; }
    }
}
