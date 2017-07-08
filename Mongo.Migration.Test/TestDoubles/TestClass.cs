using System;
using Mongo.Migration.Models;

namespace Mongo.Migration.Test.TestDoubles
{
    internal class TestClass : Document
    {
        public int Doors { get; set; }
    }
}