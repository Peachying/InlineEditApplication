using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineEditApplication.JsonEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InlineEditApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InlineEditController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "inlineedit" };
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] InlineEditRequest req)
        {
            FragInfo[] infoArray = req.Fraginfo;
            string repoUrl = infoArray[0].Origin_url.Split(new String[] { @"/blob" }, StringSplitOptions.None)[0];
            int lastone = repoUrl.LastIndexOf(@"/");
            string repoName = repoUrl.Substring(repoUrl.LastIndexOf(@"/") + 1, repoUrl.Length - repoUrl.LastIndexOf(@"/")-1);
            repoUrl = repoUrl.Replace(@"github.com", @"api.github.com/repos");
            string origintmpFile = OperateContent.modifyMdfile(infoArray);
            string modifiedPath = infoArray[0].Origin_url.Split(new String[] { @"master/" }, StringSplitOptions.None)[1];//the full path of the modified fiel in the repo
            OperateGit.ForkRepo(repoUrl);
            OperateGit.Commit(origintmpFile, modifiedPath, repoName);
            OperateGit.PullRequest(repoUrl);
            
            return origintmpFile;
        }
    }
}