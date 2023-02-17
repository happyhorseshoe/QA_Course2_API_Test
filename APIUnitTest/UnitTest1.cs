using static System.Net.WebRequestMethods;
using FluentAssertions;
using Xunit.Abstractions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using QA_MID;
using System.Security.Cryptography.X509Certificates;

namespace QA_MID
{
    public class API_Tests
    {
        private ITestOutputHelper output;                //since not a console app, there's no console writeline.
                                                          //Use logger instead or output

        public API_Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task GetSimpleBooksStatus()
        {
            string url = "https://simple-books-api.glitch.me/status";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://simple-books-api.glitch.me");

            var result = await client.GetAsync("/status");
            var content = await result.Content.ReadAsStringAsync();
            output.WriteLine(content);                                 //remember logger notes

            result.IsSuccessStatusCode.Should().BeTrue();            //calling api works
        }

        [Fact]
        public async Task GetListOfBooksShouldNotBeNull()
        {
            string url = "https://simple-books-api.glitch.me/books";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://simple-books-api.glitch.me");

            var result = await client.GetAsync("/books");
            var content = await result.Content.ReadAsStringAsync();
            output.WriteLine(content);

            result.Should().NotBeNull();
            
        }

        [Fact]
        public async Task GetSteamStatsUsers()
        {
            string url = "https://www.valvesoftware.com/about/stats";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.valvesoftware.com");

            var response = await client.GetAsync("/about/stats");
            var content = await response.Content.ReadAsStringAsync();
            output.WriteLine(content);

            var stats = System.Text.Json.JsonSerializer.Deserialize<SteamStatsResponse>(content);  //deserialize<class>

            using (new AssertionScope())
            {
                int.Parse(stats.users_online, System.Globalization.NumberStyles.AllowThousands)
                   .Should().BeGreaterThanOrEqualTo(0);
                response.IsSuccessStatusCode.Should().BeTrue();
            }
        }                                                     //see why I can't use logger instead use output. Why do I have to do
                                                              //dotnet test, pass the command line option --logger "console;verbosity=detailed":
                                                              //can I see the window like in the class video Standard Output ...1/25 class at 1:30 
                                                              //test output window was said in class

        
        [Fact]
        public async Task ReqresAPI_EndpointRequestBody()        //POST W/BODY
        {

            HttpClient client = new()
            {
                BaseAddress = new("https://reqres.in/")
            };
            RegisterPostModel postBody = new RegisterPostModel()
            {
                email = "eve.holt@reqres.in",
                password = "pistol"
            };
            var serialized = System.Text.Json.JsonSerializer.Serialize(postBody);
            var response = await client.PostAsync("/api/register", new StringContent(serialized,
                encoding: System.Text.Encoding.UTF8, "application/json"));      //all sending to API //"media type"//look at curl for hint


            var responseContent = await response.Content.ReadAsStringAsync();
            var responseAsModel = System.Text.Json.JsonSerializer.Deserialize<RegisterResponseModel>(responseContent);

            using(new AssertionScope())
            {
                response.IsSuccessStatusCode.Should().BeTrue();
                responseAsModel.id.Should().Be(4);
                responseAsModel.token.Should().NotBeNullOrEmpty();
                
            }
        }

        [Fact]
        public async Task ReqresAPI_PatchUpdate()        
        {

            HttpClient client = new()
            {
                BaseAddress = new("https://reqres.in/")
            };
            UpdatePatchModel patchBody = new UpdatePatchModel()
            {
                name = "morpheus",
                job = "zion resident"
            };
            var serialized = System.Text.Json.JsonSerializer.Serialize(patchBody);
            var response = await client.PatchAsync("/api/users/{id}", new StringContent(serialized,
                encoding: System.Text.Encoding.UTF8, "application/json"));      //all sending to API //"media type"//look at curl for hint


            var responseContent = await response.Content.ReadAsStringAsync();
            var responseAsModel = System.Text.Json.JsonSerializer.Deserialize<UpdateResponseModel>(responseContent);

            using (new AssertionScope())
            {
                response.IsSuccessStatusCode.Should().BeTrue();
                responseAsModel.name.Should().Be("morpheus");
                responseAsModel.job.Should().Be("zion resident");

            }
        }

        [Fact]
        public async Task SimpleBooksParms()
        {
            HttpClient client = new()
            {
                BaseAddress = new("https://simple-books-api.glitch.me/")
            };

            var response = await client.GetAsync("/books?type=non-fiction&limit=1");
            var content = await response.Content.ReadAsStringAsync();
            output.WriteLine(content);

            var data = System.Text.Json.JsonSerializer.Deserialize<NonnFictionResponse>(content);

            using (new AssertionScope())
            {
                response.IsSuccessStatusCode.Should().BeTrue();
                data.type.Should().Be("non-fiction");

            }
                

            
        }

        [Fact]
        public async Task ReqresAPI_Delete()
        {
            HttpClient client = new()
            {
                BaseAddress = new("https://reqres.in/")
            };

            var response = await client.DeleteAsync("/api/users/{id}");
            var content = await response.Content.ReadAsStringAsync();

            using (new AssertionScope())
            {
                content.Should().Be("");
                response.IsSuccessStatusCode.Should().BeTrue();
            }

        }

        [Fact]
         public async Task NegativeTestSimpleBooks()
         {
            HttpClient client = new()
            {
                BaseAddress = new("https://simple-books-api.glitch.me/")
            };

            var response = await client.GetAsync("/books?limit=21");
            var content = await response.Content.ReadAsStringAsync();
            output.WriteLine(content);

            var responseError = System.Text.Json.JsonSerializer.Deserialize<NegResponse>(content);

            responseError.error.Should().Be("Invalid value for query parameter 'limit'. Cannot be greater than 20.");
         }

        
            
        


    }
}




/*[Fact]
        public async Task GetNonfictionBookParam()
        {
            string url = "https://simple-books-api.glitch.me/books?type=non-fiction";     DELETED CLASS
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://simple-books-api.glitch.me");

            var result = await client.GetAsync("books?type=non-fiction");
            var content = await result.Content.ReadAsStringAsync();
            output.WriteLine(content);

            var genre = System.Text.Json.JsonSerializer.Deserialize<SimpleBooksResponse>(content);

            genre.type.Should().Be("non-fiction");*/
