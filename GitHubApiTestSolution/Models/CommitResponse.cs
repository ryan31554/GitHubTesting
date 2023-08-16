using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHubApiTestSolution.Models;
using Newtonsoft.Json;

namespace GitHubApiTestSolution.Models
{
    public class CommitResponse
    {
        public CommitModel Commit { get; set; }
    }
}
