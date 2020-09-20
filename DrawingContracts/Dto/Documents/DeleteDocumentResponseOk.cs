namespace DrawingContracts.Dto.Documents
{
    public class DeleteDocumentResponseOk:DeleteDocumentResponse
    {
        public DeleteDocumentResponseOk(DeleteDocumentRequest request)
        {
            Request = request;
        }

        public DeleteDocumentRequest Request { get; set; }
    }
}
