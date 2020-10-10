using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DIContracts.Attribute;
using DIContracts.Dto;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Documents;
using DrawingContracts.Dto.Sharing;
using DrawingContracts.Dto.SignIn;
using DrawingContracts.Interface;

namespace SharingService
{
    [Register(Policy.Transient, typeof(ISharingService))]
    public class SharingServiceImpl : ISharingService
    {
        private readonly IDrawingDal _drawingDal;

        public SharingServiceImpl(IDrawingDal drawingDal)
        {
            _drawingDal = drawingDal;
        }

        public Response GetSharedDocument(GetSharedDocumentsRequest request)
        {
            Response response = new GetSharedDocumentsResponseOk(request);
            try
            {
                var results = _drawingDal.GetSharedDocument(request.UserId);
                var allDocuments = results.Tables[0].AsEnumerable()
                    .Select(ConvertRowToDocumentObject);
                ((GetSharedDocumentsResponseOk)response).Documents = allDocuments;
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        public Response ShareDocument(ShareDocumentRequest request)
        {
            Response response = new ShareDocumentResponseOk(request);
            try
            {
                var user = _drawingDal.GetUser(new SignInRequest
                { LoginDto = new LoginDto { Email = request.UserId } });
                if (user.Tables[0].Rows.Count > 0)
                {
                    _drawingDal.ShareDocument(request);
                }
                else
                {
                    response = new ShareDocumentNoUserFoundResponse(request);
                }
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        public Response GetAllUsersForSharing(GetAllUsersForSharingRequest request)
        {
            Response response = new GetAllUsersForSharingResponseOk();
            try
            {
                var results = _drawingDal.GetAllUsers();
                IEnumerable<SharingUserInfo> allUsers = results.Tables[0].AsEnumerable()
                    .Select(ConvertRowToUserObject).Where(obj=>obj.IsActive);
                if (allUsers.Any())
                {
                    var dbUsers = _drawingDal.GetSharedUserByDocumentId(request.DocId);
                    var sharedUsers = dbUsers.Tables[0].AsEnumerable()
                        .Select(ConvertRowToUserObject).Where(obj=>obj.IsActive);
                    allUsers =MarkUserAsShared(allUsers, sharedUsers);
                }
                ((GetAllUsersForSharingResponseOk)response).Users = allUsers;
            }
            catch (Exception e)
            {
                response = new AppResponseError(e.Message);
            }

            return response;
        }

        private IEnumerable<SharingUserInfo> MarkUserAsShared(IEnumerable<SharingUserInfo> allUsers, IEnumerable<SharingUserInfo> sharedUsers)
        {
            return allUsers.Select(user =>
            {
                foreach (var sharedUser in sharedUsers)
                {
                    if (user.Email != sharedUser.Email) continue;
                    user.IsSharedWith = true;
                    break;
                }
                return user;
            });
        }

        private Document ConvertRowToDocumentObject(DataRow row)
        {
            return new Document()
            {
                DocId = (string)row[0],
                DocName = (string)row[1],
                DocUrl = (string)row[2],
                DocOwner = (string)row[3]
            };
        }

        private SharingUserInfo ConvertRowToUserObject(DataRow row)
        {
            return new SharingUserInfo
            {
                Email = (string)row[0],
                Username = (string)row[1],
                IsActive = Convert.ToBoolean(row[2])
            };
        }
    }
}
