using System;
using System.Data;
using DIContracts.Attribute;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Dto.Markers;
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
            //TODO:Change to GetParameter function - COMPLETED
            var email = _infraDal.GetParameter("P_EMAIL", request.Login.Email);
            var username = _infraDal.GetParameter("P_USER_NAME", request.Login.Username);
            //var email = new OracleParameterAdapter { Value = request.Login.Email, ParameterName = "P_EMAIL" };
            //var username = new OracleParameterAdapter { Value = request.Login.Username, ParameterName = "P_USER_NAME" };

            try
            {
                //TODO:Move this try catch somewhere else... logic is not needed here
                _infraDal.ExecuteSpQuery(_connection, "CREATE_USER", email, username);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public DataSet GetUser(SignInRequest request)
        {
            var email = _infraDal.GetParameter("P_EMAIL", request.LoginDto.Email);
            //var email = new OracleParameterAdapter { Value = request.LoginDto.Email, ParameterName = "P_EMAIL" };
            var outParam = _infraDal.GetOutParameter("retval");
            //var outParam = new OracleParameterAdapter
            //{
            //    Parameter = { OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output },
            //    ParameterName = "retval"
            //};
            return _infraDal.ExecuteSpQuery(_connection, "GET_USER", email, outParam);
        }

        public void RemoveUser(string userId)
        {
            var pUserId = _infraDal.GetParameter("p_USER_ID", userId);
            //var pUserId = new OracleParameterAdapter { Value = userId, ParameterName = "p_USER_ID" };
            _infraDal.ExecuteSpQuery(_connection, "REMOVE_USER", pUserId);
        }

        public bool UploadDocument(string docId, string userId, string filePath, string documentName)
        {
            //CREATE_DOCUMENT
            var paramDocId = _infraDal.GetParameter("p_DOC_ID", docId);
            var paramUserId = _infraDal.GetParameter("p_DOC_OWNER", userId);
            var paramDocName = _infraDal.GetParameter("p_DOC_NAME", documentName);
            var paramDocUrl = _infraDal.GetParameter("p_IMAGE_URL", filePath);
            //var paramDocId = new OracleParameterAdapter() { Value = docId, ParameterName = "p_DOC_ID" };
            //var paramUserId = new OracleParameterAdapter() { Value = userId, ParameterName = "p_DOC_OWNER" };
            //var paramDocName = new OracleParameterAdapter() { Value = documentName, ParameterName = "p_DOC_NAME" };
            //var paramDocUrl = new OracleParameterAdapter() { Value = filePath, ParameterName = "p_IMAGE_URL" };

            _infraDal.ExecuteSpQuery(_connection, "CREATE_DOCUMENT",
                paramDocId, paramDocName, paramUserId, paramDocUrl);
            return true;
        }

        public DataSet GetDocumentById(string id)
        {
            var paramDocId = _infraDal.GetParameter("p_DOCUMENT_ID", id);
            //var paramDocId = new OracleParameterAdapter { Value = id, ParameterName = "p_DOCUMENT_ID" };

            var outParam = _infraDal.GetOutParameter("retval");
            //var outParam = new OracleParameterAdapter
            //{
            //    Parameter = { OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output },
            //    ParameterName = "retval"
            //};
            return _infraDal.ExecuteSpQuery(_connection, "GET_DOCUMENT", paramDocId, outParam);
        }

        public void DeleteDocument(DeleteDocumentRequest request)
        {
            var paramDocId = _infraDal.GetParameter("p_DOCUMENT_ID", request.DocId);
            //var paramDocId = new OracleParameterAdapter() { Value = request.Document.DocId, ParameterName = "p_DOCUMENT_ID" };

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
            //var paramDocId    = new OracleParameterAdapter { Value = request.Marker.DocId, ParameterName = "p_DOC_ID" };
            //var paramUserId   = new OracleParameterAdapter { Value = request.Marker.OwnerUser, ParameterName = "p_OWNER_USER_ID" };
            //var paramMarkerId = new OracleParameterAdapter { Value = request.Marker.MarkerId, ParameterName = "p_MARKER_ID" };
            //var paramPos      = new OracleParameterAdapter { Value = request.Marker.Position, ParameterName = "p_POS" };
            //var paramMarkType = new OracleParameterAdapter { Value = request.Marker.MarkerType.ToString(), ParameterName = "p_MARK_TYPE" };
            //var paramColor    = new OracleParameterAdapter { Value = request.Marker.Color, ParameterName = "p_COLOR" };


            _infraDal.ExecuteSpQuery(_connection, "CREATE_MARKER",
                 paramDocId, paramMarkType, paramPos, paramColor, paramUserId, paramMarkerId);
        }

        public void DeleteMarker(string markerId)
        {
            var paramMarkerId = _infraDal.GetParameter("p_MARKER_ID", markerId);
            //var paramMarkerId = new OracleParameterAdapter() { Value = markerId, ParameterName = "p_MARKER_ID" };

            _infraDal.ExecuteSpQuery(_connection, "REMOVE_MARKER", paramMarkerId);
        }

        public DataSet GetAllMarkers(string documentId)
        {
            var paramDocId = _infraDal.GetParameter("p_DOC_ID", documentId);
            //var paramDocId = new OracleParameterAdapter { Value = documentId, ParameterName = "p_DOC_ID" };
            var outParam = _infraDal.GetOutParameter("o_RETVAL");
            //var outParam = new OracleParameterAdapter
            //{
            //    Parameter = { OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output },
            //    ParameterName = "o_RETVAL"
            //};
            return _infraDal.ExecuteSpQuery(_connection, "GET_MARKERS", paramDocId, outParam);
        }

        public DataSet GetAllDocuments(string owner)
        {
            var paramDocOwner = _infraDal.GetParameter("P_OWNER_ID", owner);
            var outParam = _infraDal.GetOutParameter("RETVAL");

            return _infraDal.ExecuteSpQuery(_connection, "GET_DOCUMENTS_OF_OWNER", paramDocOwner, outParam);
        }
    }
}
