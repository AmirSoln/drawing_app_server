using System;
using System.Data;
using DIContracts.Attribute;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Dto.Markers;
using DrawingContracts.Dto.Sharing;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Dto.SignUp;
using DrawingContracts.Interface;
using Microsoft.Extensions.Configuration;
using InfraDal;
using InfraDalContracts;
using IDbConnection = InfraDalContracts.IDbConnection;

//TODO:Clean all comments here
namespace DrawingDal
{
    [Register(Policy.Singleton, typeof(IDrawingDal))]
    public class DrawingDalImpl : IDrawingDal
    {
        private readonly IInfraDal _infraDal;
        private readonly IDbConnection _connection;
        private readonly IConfiguration _configuration;
        public DrawingDalImpl(IConfiguration configuration)// TODO:change to IConfiguration - COMPLETED
        {
            _configuration = configuration;
            var strConn = _configuration.GetConnectionString("mainDb");
            _infraDal = new InfraDalImpl();
            _connection = _infraDal.Connect(strConn);
        }

        public bool CreateUser(SignUpRequest request)
        {
            var email = _infraDal.GetParameter("P_EMAIL", request.Login.Email);
            var username = _infraDal.GetParameter("P_USER_NAME", request.Login.Username);

            try
            {
                _infraDal.ExecuteSpQuery(_connection, "CREATE_USER", email, username);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public DataSet GetUser(SignInRequest request)
        {
            var email = _infraDal.GetParameter("P_EMAIL", request.LoginDto.Email);
            var outParam = _infraDal.GetOutParameter("retval");

            return _infraDal.ExecuteSpQuery(_connection, "GET_USER", email, outParam);
        }

        public void RemoveUser(string userId)
        {
            var pUserId = _infraDal.GetParameter("p_USER_ID", userId);

            _infraDal.ExecuteSpQuery(_connection, "REMOVE_USER", pUserId);
        }

        public bool UploadDocument(string docId, string userId, string filePath, string documentName)
        {
            var paramDocId = _infraDal.GetParameter("p_DOC_ID", docId);
            var paramUserId = _infraDal.GetParameter("p_DOC_OWNER", userId);
            var paramDocName = _infraDal.GetParameter("p_DOC_NAME", documentName);
            var paramDocUrl = _infraDal.GetParameter("p_IMAGE_URL", filePath);

            _infraDal.ExecuteSpQuery(_connection, "CREATE_DOCUMENT",
                paramDocId, paramDocName, paramUserId, paramDocUrl);
            return true;
        }

        public DataSet GetDocumentById(string id)
        {
            var paramDocId = _infraDal.GetParameter("p_DOCUMENT_ID", id);

            var outParam = _infraDal.GetOutParameter("retval");

            return _infraDal.ExecuteSpQuery(_connection, "GET_DOCUMENT", paramDocId, outParam);
        }

        public void DeleteDocument(DeleteDocumentRequest request)
        {
            var paramDocId = _infraDal.GetParameter("p_DOCUMENT_ID", request.DocId);

            _infraDal.ExecuteSpQuery(_connection, "REMOVE_DOCUMENT", paramDocId);
        }

        public void CreateMarker(CreateMarkerRequest request)
        {
            var paramDocId = _infraDal.GetParameter("p_DOC_ID", request.Marker.DocId);
            var paramUserId = _infraDal.GetParameter("p_OWNER_USER_ID", request.Marker.OwnerUser);
            var paramMarkerId = _infraDal.GetParameter("p_MARKER_ID", request.Marker.MarkerId);
            var paramPos = _infraDal.GetParameter("p_POS", request.Marker.Position);
            var paramMarkType = _infraDal.GetParameter("p_MARK_TYPE", request.Marker.MarkerType.ToString());
            var paramColor = _infraDal.GetParameter("p_COLOR", request.Marker.Color);

            _infraDal.ExecuteSpQuery(_connection, "CREATE_MARKER",
                 paramDocId, paramMarkType, paramPos, paramColor, paramUserId, paramMarkerId);
        }

        public void DeleteMarker(string markerId)
        {
            var paramMarkerId = _infraDal.GetParameter("p_MARKER_ID", markerId);

            _infraDal.ExecuteSpQuery(_connection, "REMOVE_MARKER", paramMarkerId);
        }

        public DataSet GetAllMarkers(string documentId)
        {
            var paramDocId = _infraDal.GetParameter("p_DOC_ID", documentId);
            var outParam = _infraDal.GetOutParameter("o_RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_MARKERS", paramDocId, outParam);
        }

        public DataSet GetAllDocuments(string owner)
        {
            var paramDocOwner = _infraDal.GetParameter("P_OWNER_ID", owner);
            var outParam = _infraDal.GetOutParameter("RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_DOCUMENTS_OF_OWNER", paramDocOwner, outParam);
        }

        public DataSet GetSharedDocument(string userId)
        {
            var paramUserId = _infraDal.GetParameter("P_USER_ID", userId);
            var outParam = _infraDal.GetOutParameter("RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_SHARED_DOCUMENTS", paramUserId, outParam);
        }

        public void ShareDocument(ShareDocumentRequest request)
        {
            var paramUserId = _infraDal.GetParameter("P_USER_ID", request.UserId);
            var paramDocId = _infraDal.GetParameter("P_DOC", request.DocId);

            _infraDal.ExecuteSpQuery(_connection, "CREATE_SHARED_DOCUMENT", paramDocId, paramUserId);
        }

        public DataSet GetAllUsers()
        {
            var outParam = _infraDal.GetOutParameter("RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_USERS_FOR_SHARE", outParam);
        }

        public DataSet GetSharedUserByDocumentId(string requestDocId)
        {
            var paramDocId = _infraDal.GetParameter("P_DOC_ID", requestDocId);
            var outParam = _infraDal.GetOutParameter("RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_SHARED_USERS_OF_DOCUMENT", paramDocId, outParam);
        }

        public void EditMarkerById(EditMarkerRequest request)
        {
            var paramMarkerId = _infraDal.GetParameter("P_MARKER_ID", request.MarkerId);
            var paramColor = _infraDal.GetParameter("P_COLOR", request.Color);

            _infraDal.ExecuteSpQuery(_connection, "EDIT_MARKER", paramMarkerId,paramColor);
        }
    }
}
