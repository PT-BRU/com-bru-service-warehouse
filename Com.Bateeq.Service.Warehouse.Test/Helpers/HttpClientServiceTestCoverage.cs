using Com.Bateeq.Service.Warehouse.Lib.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Helpers
{
    public class HttpClientServiceTestCoverage
    {
        private const string url = "http://127.0.0.1/";
        private HttpClientService HttpClientService => new HttpClientService(new IdentityService());
        private HttpContent HttpContent => new StringContent("");

        [Fact]
        public async Task Get()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.GetAsync(url));
        }

        [Fact]
        public async Task Post()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PostAsync(url, HttpContent));
        }

        [Fact]
        public async Task Put()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PutAsync(url, HttpContent));
        }

        [Fact]
        public async Task Delete()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.DeleteAsync(url));
        }

        [Fact]
        public async Task Patch()
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => HttpClientService.PatchAsync(url, HttpContent));
        }
    }
}
