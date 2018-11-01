using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace RatingStuff
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            [CosmosDB(
                databaseName: "zedatabasechallenge2",
                collectionName: "zedbcollection",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<Rating> ratingsOut, TraceWriter log)
        {
            try
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                string stringRating = data?.rating;
                string productId = data?.productId;
                string userId = data?.userId;
                string locationName = data?.locationName;
                string userNotes = data?.userNotes;

                if (int.TryParse(stringRating, out int rating))
                {
                    if (rating < 0 || rating > 5)
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Rating was not between 0 and 5");
                    }
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Rating was not an integer");
                }

                JsonSerializer serializer = new JsonSerializer();
                using (var httpClient = new HttpClient())
                {
                    // Validate the product
                    var response = await httpClient.GetAsync($"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productid={productId}");
                    string responseString = await response.Content.ReadAsStringAsync();
                    try
                    {
                        Product p = JsonConvert.DeserializeObject<Product>(responseString);
                    }
                    catch (Exception)
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Invalid product ID");
                    }

                    // Validate the user
                    response = await httpClient.GetAsync($"https://serverlessohuser.trafficmanager.net/api/GetUser?userid={userId}");
                    responseString = await response.Content.ReadAsStringAsync();
                    try
                    {
                        User p = JsonConvert.DeserializeObject<User>(responseString);
                    }
                    catch (Exception)
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest, "Invalid user ID");
                    }
                }

                var r = new Rating
                {
                    id = Guid.NewGuid().ToString(),
                    rating = -2,
                    userId = userId,
                    productid = productId,
                    locationName = locationName,
                    userNotes = userNotes,
                    timestamp = DateTime.Now
                };

                await ratingsOut.AddAsync(r);

                return req.CreateResponse(HttpStatusCode.OK, r);
            }
            catch (Exception e)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }
    }
}
