using Microsoft.Extensions.Configuration;
using multitronikllc.Servicios;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace msTest
{
    [TestClass]
    public sealed class SrvApiService_Test
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage? response = null)
            {
                var body = new ResponsePacket() { content = "NgMAAAEgIPXnAAACgWEgbEMBAAaBcWtmeGhq" };
                _response = response ?? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(body)
                };
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(_response);
            }
        }

        private SrvApiService _service;
        private FakeHttpMessageHandler handler = new FakeHttpMessageHandler();
        private HttpClient client;

        public SrvApiService_Test()
        {

            client = new HttpClient(handler);
            var severUrl = new KeyValuePair<string, string?>("serverUrl", "http://test");
            var configData = new List<KeyValuePair<string, string?>>()
            {
                severUrl
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            _service = new SrvApiService(client, config);
        }

        [TestMethod]
        public async Task userId_Test()
        {
            _service.userId = 546;

            Assert.AreEqual(546, _service.userId, "Asignación de Id de Usuario");
        }

        [TestMethod]
        public async Task Restart_Test()
        {
            _service.userId = 546;
            await _service.Restart();

            // Assert: comprobar que se realizó una GET al endpoint esperado
            Assert.IsNotNull(handler.LastRequest, "No se recibió ninguna petición.");
            Assert.AreEqual("http://test/Challenge/restart?userId=546", handler.LastRequest.RequestUri?.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(822)]
        public async Task LeerPackete_Test(int? idPaket)
        {
            _service.userId = 546;
            var paket = await _service.LeerPackete(idPaket);
            Assert.AreEqual(546, _service.userId);
            Assert.AreEqual(822, paket.Item1.Id);
            Assert.AreEqual(1 , paket.Item1.Size);
            Assert.AreEqual(32 , paket.Item1.Checksum);
            Assert.AreEqual(" ", paket.Item2);

            if(idPaket.HasValue)
            {
                Assert.AreEqual("http://test/Challenge/retry-packet?packetId=822", handler.LastRequest?.RequestUri?.AbsoluteUri);
            }
            else
            {
                Assert.AreEqual("http://test/Challenge/get-next-packet", handler.LastRequest?.RequestUri?.AbsoluteUri);
            }            
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest?.Method);
        }
    }
}
