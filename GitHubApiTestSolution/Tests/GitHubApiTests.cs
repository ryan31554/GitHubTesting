using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using System.Threading.Tasks;
using GitHubApiTestSolution.Models;
using GitHubApiTestSolution.Solution_Artifacts.Extensions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GitHubApiTestSolution.Tests
{
    [TestFixture]
    public class GitHubApiTests : PlaywrightTest
    {
        private static readonly string jBogardGitHubRepo = "repos/jbogard/MediatR";
        private static readonly string apiUrl = "https://api.github.com/repos/jbogard/MediatR";
        private static readonly string tokenFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Tests", "StaticTestData", "Token.txt");
        private static readonly string api_Token = File.ReadAllText(tokenFilePath);

        private IAPIRequestContext Request = null;
        private HttpClient httpClient;

        [SetUp]
        public async Task SetUp()
        {
            await CreateAPIRequestContext();
        }

        private async Task CreateAPIRequestContext()
        {
            var headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/vnd.github.v3+json",
                ["Authorization"] = $"token {api_Token}"
            };

            Request = await this.Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = "https://api.github.com",
                ExtraHTTPHeaders = headers,
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            await Request.DisposeAsync();
        }

        /// <summary>
        /// Sends a GET request to the API to fetch data for a specific endpoint and deserializes it into the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        /// <param name="relativeEndPoint"></param>
        /// <returns></returns>
        private async Task<T> GetDataAsync<T>(string repo, string relativeEndPoint)
        {
            var response = await Request.GetAsync($"/{repo}/{relativeEndPoint}");
            var json = await response.TextAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        [Test]
        public async Task MostRecentCommitShouldBeJimmyBogard()
        {
            var responseContent = await GetDataAsync<List<CommitResponse>>(jBogardGitHubRepo, "commits");

            var mostRecentCommit = responseContent
                .OrderByDescending(commit => commit.Commit.Author.Date)
                .Select(commit => commit.Commit.Author.Name)
                .First();

            mostRecentCommit.RemoveExtraSpaces().Should().Be("Jimmy Bogard");
        }

        [Test]
        public async Task MostRecentCommitIsDated10July()
        {
            var responseContent = await GetDataAsync<List<CommitResponse>>(jBogardGitHubRepo, "commits");

            var commitsOrdered = responseContent
                .OrderByDescending(commit => commit.Commit.Author.Date)
                .ToList();

            foreach (var c in commitsOrdered)
            {
                Console.WriteLine($"Author is {c.Commit.Author.Name}, date is {c.Commit.Author.Date}");
            }

            var mostRecentCommit = responseContent
                .OrderByDescending(commit => commit.Commit.Author.Date)
                .Select(commit => commit.Commit.Author.Date)
                .First();

            mostRecentCommit.FormatDateWithBackgroundNumbers().Should().Be("10th July 2023");
        }

        [Test]
        public async Task RequestWithoutAuthTokenShouldReturn403WithBadCredentialsMessage()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            var url = $"{apiUrl}/commits";

            // Remove authentication headers to simulate absence of auth token
            httpClient.DefaultRequestHeaders.Authorization = null;

            // Clear other headers that might interfere
            httpClient.DefaultRequestHeaders.Remove("Authorization");

            var response = await httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assertions
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            responseBody.Should().Contain("Bad credentials");
        }
    }
}
