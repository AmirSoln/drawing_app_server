using System.Collections.Generic;
using System.Linq;
using DocumentService;
using DrawingContracts.Dto;
using DrawingContracts.Dto.Sharing;
using DrawingContracts.Interface;
using DrawingDal;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SharingService;
using SignUpService;
using TestingUtilities;

namespace SharingServiceTests
{
    public class Tests
    {
        private IDrawingDal _drawingDal;
        private IConfiguration _configuration;
        private ISignUpService _signUpService;
        private IDocumentService _documentService;
        private ISharingService _sharingService;

        private TestUtilitiesImpl _testUtilitiesImpl;
        private const int CreatedPlayersAmount = 2;
        private List<string> _createdUsers;

        [OneTimeSetUp]
        public void FirstSetup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("test.appsettings.json")
                .Build();

            var strConn = _configuration.GetConnectionString("mainDb");
            _testUtilitiesImpl = new TestUtilitiesImpl(strConn);
            _drawingDal = new DrawingDalImpl(_configuration);
            _documentService = new DocumentServiceImpl(_drawingDal);
            _signUpService = new SignUpServiceImpl(_drawingDal);
            _createdUsers = _testUtilitiesImpl.CreateUserDummyData(_signUpService, CreatedPlayersAmount);
        }

        [OneTimeTearDown]
        public void FinalTearDown()
        {
            _testUtilitiesImpl.DestroyUserDummyData(_createdUsers.ToArray());
        }

        [SetUp]
        public void Setup()
        {
            _sharingService = new SharingServiceImpl(_drawingDal);
        }

        [TearDown]
        public void TearDown()
        {
            _sharingService = null;
        }

        [Test]
        public void TestGetSharedDocument_GetSharedDocumentsResponseOk()
        {
            const int dataCount = 3;
            var expectedType = typeof(GetSharedDocumentsResponseOk);

            //given a database with some documents and a user that has document shared with
            var dummyDocumentData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[CreatedPlayersAmount - 1]);
            var shared = _sharingService.ShareDocument(new ShareDocumentRequest
            { DocId = dummyDocumentData[0], UserId = _createdUsers[0] });


            //when we get shared documents
            var request = new GetSharedDocumentsRequest { UserId = _createdUsers[0] };
            var result = _sharingService.GetSharedDocument(request);

            //the we get GetSharedDocumentsResponseOk
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(((GetSharedDocumentsResponseOk)result).Documents.Count(), Is.GreaterThanOrEqualTo(1));
            });

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyDocumentData, _documentService);
        }

        [TestCase("bobo")]
        [TestCase("bobo@gmail2222.com")]
        [TestCase("")]
        [TestCase(null)]
        public void TestShareDocumentWithNonExistingUser_ShareDocumentNoUserFoundResponse(string user)
        {
            const int dataCount = 5;
            var expectedType = typeof(ShareDocumentNoUserFoundResponse);

            //given a database with some documents
            var dummyDocumentData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[CreatedPlayersAmount - 1]);


            //when we share document with non existing user
            var request = new ShareDocumentRequest { DocId = dummyDocumentData[0], UserId = user };
            var result = _sharingService.ShareDocument(request);

            //the we get ShareDocumentNoUserFoundResponse
            Assert.That(result, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyDocumentData, _documentService);
        }

        [TestCase("bobo")]
        [TestCase("sdd-234-sf")]
        [TestCase("")]
        [TestCase(null)]
        public void TestShareDocumentWithNonExistingDocument_AppResponseError(string doc)
        {
            const int dataCount = 5;
            var expectedType = typeof(AppResponseError);

            //given a database with some documents
            var dummyDocumentData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[CreatedPlayersAmount - 1]);


            //when we share non existing document with existing user
            var request = new ShareDocumentRequest { DocId = doc, UserId = _createdUsers[0] };
            var result = _sharingService.ShareDocument(request);

            //the we get AppResponseError
            Assert.That(result, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyDocumentData, _documentService);
        }

        [Test]
        public void TestShareExistingDocumentWithExistingUser_ShareDocumentResponseOk()
        {
            const int dataCount = 5;
            var expectedType = typeof(ShareDocumentResponseOk);

            //given a database with some documents
            var dummyDocumentData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[CreatedPlayersAmount - 1]);


            //when we share non existing document with existing user
            var request = new ShareDocumentRequest { DocId = dummyDocumentData[0], UserId = _createdUsers[0] };
            var result = _sharingService.ShareDocument(request);

            //the we get ShareDocumentResponseOk
            Assert.That(result, Is.TypeOf(expectedType));

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyDocumentData, _documentService);
        }

        [Test]
        public void GetAllUsersForSharingWithUserToShare_ResponseOkAndAListOfUsersToShareWith()
        {
            const int dataCount = 5;
            var expectedType = typeof(GetAllUsersForSharingResponseOk);

            //given a database with some documents
            var dummyDocumentData = _testUtilitiesImpl
                .CreateDocumentDummyData(_documentService, dataCount, _createdUsers[CreatedPlayersAmount - 1]);


            //when we share non existing document with existing user
            var request = new GetAllUsersForSharingRequest { DocId = dummyDocumentData[0] };
            var result = _sharingService.GetAllUsersForSharing(request);

            //the we get GetAllUsersForSharingResponseOk
            //and we get some users
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(expectedType));
                Assert.That(((GetAllUsersForSharingResponseOk) result).Users.Count(),Is.GreaterThan(0));

            });

            _testUtilitiesImpl.DeleteDocumentDummyData(dummyDocumentData, _documentService);
        }
    }
}