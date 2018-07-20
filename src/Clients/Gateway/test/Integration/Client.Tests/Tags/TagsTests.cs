using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Client.Tests.Tags
{
    [TestFixture]
    public class TagsTests
    {
        [Test]
        public async Task Test()
        {
            var token =
                "";

            var settings = new GatewaySettings() {TagsUrl = "http://localhost:54308"};

            var gateway = new Gateway(settings);

            var id = new Guid("5c22f3b3-f4e1-4707-aa1f-731c17e2fe3a");
            var result = await gateway.Tags.GetByIdAsync(id, token);
        }
    }
}
