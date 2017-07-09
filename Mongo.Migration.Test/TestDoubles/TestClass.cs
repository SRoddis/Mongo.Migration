using System;
using MongoDB.Bson;

namespace Mongo.Migration.Test.TestDoubles
{
    public class TestClass
    {
        public ObjectId Id { get; set; }

        public int Dors { get; set; }
    }
}