using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RatingStuff
{
    public static class GetRatingById
    {
        [FunctionName("GetRatingById")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "zedatabasechallenge2",
                collectionName: "zedbcollection",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c")] IEnumerable<Rating> ratings,
            ILogger log)
        {
            var ratingId = req.Query["ratingId"];     
            return new JsonResult(ratings.Where(e => e.id == ratingId));
        }
    }
}
