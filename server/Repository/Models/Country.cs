using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Country
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public NameDetails Name { get; set; }
        public string[] Capital { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public int Population { get; set; }

        public FlagsDetails Flags { get; set; }


    }
}
