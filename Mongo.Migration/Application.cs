using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration
{
    internal class Application : IApplication
    {
        private readonly IMongoRegistrater _registrater;

        public Application(IMongoRegistrater registrater)
        {
            _registrater = registrater;
        }

        public void Run()
        {
            _registrater.Registrate();
        }
    }
}