using System.Data;
using System.Data.SqlClient;
using Backend.Fx.RandomData;

namespace MarcWittke.DockerUtil.Tests
{
    public class TestContainer : MssqlDockerContainer
    {
        public TestContainer(string dockerApiUrl, string name)
            : base(dockerApiUrl, TestRandom.NextPassword(), name)
        { }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}