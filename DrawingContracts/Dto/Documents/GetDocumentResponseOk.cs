namespace DrawingContracts.Dto.Documents
{
    public class GetDocumentResponseOk:GetDocumentResponse
    {
        public GetDocumentResponseOk(Document doc)
        {
            Doc = doc;
        }

        public Document Doc { get; set; }
    }
}
