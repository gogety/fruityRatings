using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System;

namespace RatingStuff
{
        public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "zedatabasechallenge2",
                collectionName: "zedbcollection",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Rating> ratings,
            ILogger log)
        {
            var userId = req.Query["userId"];
            return new JsonResult(ratings.Where(e => e.userId == userId));
        }
    }
}
