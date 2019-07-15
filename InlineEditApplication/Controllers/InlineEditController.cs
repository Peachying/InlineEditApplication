﻿using System;
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
            string origintmpFile = OperateContent.modifyMdfile(infoArray);
            string modifiedPath = infoArray[0].Origin_url.Split(new String[] { @"master/" }, StringSplitOptions.None)[1];
            OperateGit.ForkRepo();
            OperateGit.Commit(origintmpFile, modifiedPath);
            OperateGit.PullRequest();
            
            return origintmpFile;
        }
    }
}