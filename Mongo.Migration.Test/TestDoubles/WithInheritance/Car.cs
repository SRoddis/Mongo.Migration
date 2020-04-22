using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Test.TestDoubles
{

    [StartUpVersion("0.0.1")]
    internal class Car : Vehicle
    {
        public int Doors { get; set; }
    }
}
