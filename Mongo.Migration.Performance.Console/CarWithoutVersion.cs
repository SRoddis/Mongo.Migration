using System;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.Performance.Console
{
    public class CarWithoutVersion
    {
        public ObjectId Id { get; set; }

        public int Dors { get; set; }
    }
}