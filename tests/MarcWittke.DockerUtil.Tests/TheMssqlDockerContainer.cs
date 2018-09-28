using System;
using System.Threading.Tasks;
using Xunit;

namespace MarcWittke.DockerUtil.Tests
{
    public class TheMssqlDockerContainer : IAsyncLifetime
    {
        private string _dockerApiUri;
        private string _containerName;
        private TestContainer _container;

        [Fact]
        public async Task CanBeUsed()
        {
            _containerName = CreateContainerName("TheMssqlDockerContainer_CanBeUsed");
            
            _container = new TestContainer(_dockerApiUri, _containerName);
            await _container.InitializeAsync();
            await _container.CreateAndStartAsync();
            Assert.False(_container.HealthCheck());
            Assert.True(_container.WaitUntilIsHealthy());
        }

        [Fact]
        public async Task CanRestore()
        {
            _containerName = CreateContainerName("TheMssqlDockerContainer_CanRestore");
            await DockerUtilities.EnsureKilledAndRemoved(_dockerApiUri, _containerName);
            _container = new TestContainer(_dockerApiUri, _containerName);

            await _container.CreateAndStartAsync();
            Assert.False(_container.HealthCheck());
            Assert.True(_container.WaitUntilIsHealthy());

            await _container.Restore("Backup.bak", "RestoredDb");
        }

        private static string CreateContainerName(string name)
        {
            if (IsTfBuild)
            {
                return $"{name}_agent{AgentId}";
            }

            return name;
        }

        public async Task InitializeAsync()
        {
            _dockerApiUri = await DockerUtilities.DetectDockerClientApi();
            if (IsTfBuild)
            {
                await DockerUtilities.KillAllOlderThan(_dockerApiUri, TimeSpan.FromMinutes(30));
            }
        }

        public async Task DisposeAsync()
        {
            await _container.EnsureKilledAsync();
        }

        private static bool IsTfBuild => global::System.Environment.GetEnvironmentVariable("TF_BUILD") == "True";
        private static string AgentId => global::System.Environment.GetEnvironmentVariable("AGENT_ID");
    }
}
