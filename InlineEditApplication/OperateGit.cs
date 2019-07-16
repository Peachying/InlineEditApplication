using InlineEditApplication.JsonEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InlineEditApplication
{
    public class OperateGit
    {
        private const string url_fork = @"/forks"; //@"https://api.github.com/repos/Peachying/testinlineedit/forks";
        private const string url_Head = @"https://api.github.com/repos/GraceXu96/"; 
        private static string url_getRefTail = @"/git/refs/heads/master"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/refs/heads/master";
        private static string url_createBlobTail = @"/git/blobs"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/blobs";
        private static string url_createTreeTail = @"/git/trees"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/trees";
        private static string url_getCommitTail = @"/git/commits/"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/commits/";
        private static string url_createCommitTail = @"/git/commits"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/commits";
        private static string url_updateRefTail = @"/git/refs/heads/master"; //@"https://api.github.com/repos/GraceXu96/testinlineedit/git/refs/heads/master";
        private static string url_pullRequestTail = @"/pulls"; //@"https://api.github.com/repos/Peachying/testinlineedit/pulls";

 

        public static void ForkRepo(string repoUrl)
        {            
            string fork_res = JObject.Parse(Post(repoUrl + url_fork, "")).ToString();
            Console.WriteLine("Fork the original reposity.");
        }

        public static void PullRequest(string repoUrl)
        {
            string url_pullRequest = repoUrl + url_pullRequestTail;
            CreatePullRequest pullRequestBody = new CreatePullRequest
            {
                Title = "test PR with Github API",
                Head = "GraceXu96:master",
                Base = "master",
                Body = "Please pull this in!"
            };
            string reqBody = JsonConvert.SerializeObject(pullRequestBody);
            string pr_res = JObject.Parse(Post(url_pullRequest, reqBody)).ToString();
            Console.WriteLine(pr_res);
        }

        public static void Commit(string originmdPath, string modifiedPath, string repoName)
        {
            Console.WriteLine("******************Six steps for Commit***************************");
            //get reference & tree to commit 
            string url_getRef = url_Head + repoName + url_getRefTail;
            string url_getCommit = url_Head + repoName + url_getCommitTail;
            string parent_sha = JObject.Parse(Get(url_getRef, new Dictionary<string, string>()))["object"]["sha"].ToString();
            string baseTree_sha = JObject.Parse(Get(url_getCommit + parent_sha, new Dictionary<string, string>()))["tree"]["sha"].ToString();

            //create a  blob
            string url_createBlob = url_Head + repoName + url_createBlobTail;
            StreamReader sr = new StreamReader(originmdPath, Encoding.GetEncoding("utf-8"));
            CreateBlobRequest createBlobRequest = new CreateBlobRequest
            {
                Content = sr.ReadToEnd(),
                Encoding = "utf-8"
            };
            string createBlobBody = JsonConvert.SerializeObject(createBlobRequest);
            string blob_sha = JObject.Parse(Post(url_createBlob, createBlobBody))["sha"].ToString();

            //create a new tree for commit
            string url_createTree = url_Head + repoName + url_createTreeTail;
            CreateTreeRequest createTreeRequest = new CreateTreeRequest
            {
                BaseTree = baseTree_sha,
                Tree = new TreeNode[] {
                    new TreeNode{
                        Path = modifiedPath,
                        Mode = "100644",
                        Type = "blob",
                        Sha = blob_sha
                    }
                }
            };
            string createTreeBody = JsonConvert.SerializeObject(createTreeRequest);
            string treeSubmit_sha = JObject.Parse(Post(url_createTree, createTreeBody))["sha"].ToString();

            //create a  new commit
            string url_createCommit = url_Head + repoName + url_createCommitTail;
            CreateCommitRequest createCommitRequest = new CreateCommitRequest
            {
                Message = "Commit automatically!",
                Parents = new string[] { parent_sha },
                Tree = treeSubmit_sha
            };
            string createCommitBody = JsonConvert.SerializeObject(createCommitRequest);
            string createSubmit_sha = JObject.Parse(Post(url_createCommit, createCommitBody))["sha"].ToString();

            //update reference
            string url_updateRef = url_Head + repoName + url_updateRefTail;
            UpdateReferenceRequest updateReferenceRequest = new UpdateReferenceRequest
            {
                Sha = createSubmit_sha,
                Force = true
            };
            string updateReferenceBody = JsonConvert.SerializeObject(updateReferenceRequest);
            string updateRef_res = Post(url_updateRef, updateReferenceBody).ToString();
        }

        public static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/vnd.github.v3+json";
            req.Headers.Add("Authorization", "token ");
            req.UserAgent = "Code Sample Web Client";
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(content);
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }



        public static string Get(string url, Dictionary<string, string> dic)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);

            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            req.ContentType = "application/vnd.github.v3+json";
            req.Headers.Add("Authorization", "token ");
            req.UserAgent = "Code Sample Web Client";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
    }
}
