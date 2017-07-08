using System;
using Mongo.Migration.Models;

namespace Mongo.Migration.Test.TestDoubles
{
    internal class TestDocument : Document
    {
        public int Doors { get; set; }
    }
}