using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TDAmeritradeSharpClient;

namespace TDAmeritrade.Tests
{
    public class OrdersTests
    {
        Client _client;
        private string _testAccountId;

        [SetUp]
        public async Task Init()
        {
            // Please sign in first, following services uses the client file
            _client = new Client();
            try
            {
                await _client.RequireNotExpiredTokensAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
                throw;
            }
            Assert.IsTrue(_client.IsSignedIn);
            var userSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpClient));
            var testAccountPath = Path.Combine(userSettingsDirectory, $"TestAccount.txt");
            _testAccountId = await File.ReadAllTextAsync(testAccountPath);
        }

        [Test]
        public async Task TestGetAccount()
        {
            var account = await _client.GetAccount(_testAccountId);
            Assert.IsTrue(account.securitiesAccount.accountId == _testAccountId);
        }
        
        [Test]
        public async Task TestGetAccounts()
        {
            var accounts = await _client.GetAccounts();
            var testAccount = accounts.FirstOrDefault(x => x.securitiesAccount.accountId == _testAccountId);
            Assert.IsNotNull(testAccount);
        }
    }
}