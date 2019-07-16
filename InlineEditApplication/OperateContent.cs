using InlineEditApplication.JsonEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InlineEditApplication
{
        
    public class OperateContent
    {
        private static string cmdPath = @"C:\Users\t-yucxu\Desktop\testInlineEdit\middlefile"; //@"D:\home\"; 
        private static string processName = cmdPath + @"\pandoc.exe";
        private const string args = @"-t markdown -o ""{0}"" -f html  ""{1}""";
           

        public static string modifyMdfile(FragInfo[] infoArray)
        {
            //var infoArray = JsonConvert.DeserializeObject<FragInfo[]>(infoStr);
            string origintmpFile = Path.GetTempFileName();//tmp origin md file path
            if (infoArray.Length >= 0)
            {
                //save origin markdown from Github to tmp file
                var originUrl = infoArray[0].Origin_url;
                var realUrl = originUrl.Replace("github", "raw.githubusercontent");
                realUrl = realUrl.Replace("blob", "");
                string originContent = OperateGit.Get(realUrl, new Dictionary<string, string>());
                FileStream fs = new FileStream(origintmpFile,FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(originContent);
                sw.Close();
                fs.Close();
            }

            foreach (FragInfo info in infoArray)
            {
                info.Content = removeHtmlTags(info.Content);//remove <del></del> and <ins></ins>
                string mdContent = Convert(info.Content);             
                EditFile(int.Parse(info.StartLine), int.Parse(info.EndLine), origintmpFile, mdContent);                
            }
            Console.WriteLine("Replace modified parts in the original md file");
            return origintmpFile;
        }

        

            private static string Convert(string content)
        {
            //use opandoc to convert html=>md
            var process = new Process();

            var htmltmpFile = Path.GetTempFileName();
            var mdtmpFile = Path.GetTempFileName();

            File.WriteAllText(htmltmpFile, content, Encoding.UTF8);

            //FileStream fs_html = new FileStream(htmltmpFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //StreamWriter sw_html = new StreamWriter(fs_html);
            //sw_html.WriteLine(content);
            //sw_html.Flush();
            //sw_html.Close();
            //fs_html.Close();
            ProcessStartInfo psi = new ProcessStartInfo(processName, string.Format(args, mdtmpFile, htmltmpFile))
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
            FileStream fs_md = new FileStream(mdtmpFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sr_md = new StreamReader(fs_md);
            string md_content = sr_md.ReadToEnd();
            sr_md.Close();
            fs_md.Close();
            //File.Delete(htmltmpFile);
            //File.Delete(mdtmpFile);  //clear tmp
            return md_content;
        }

        private static string removeHtmlTags(string content)
        {
            //modify the fragiel html file
            string regex_del = @"<del[^>]*?>.*?</del>";
            string regex_ins = @"<\/?ins.*?>";
            content = Regex.Replace(content, regex_del, string.Empty, RegexOptions.IgnoreCase);
            content = Regex.Replace(content, regex_ins, string.Empty, RegexOptions.IgnoreCase);
            return content;
        }

        

        private static void EditFile(int startLine, int endLine, string origintmpFile, string content)
        {
            //edit the corresponding lines in the original md
            string newLines = content.Replace("\r\n", " ");
            FileStream fs = new FileStream(origintmpFile, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));
            string line = sr.ReadLine();
            string text = "";
            //1. read and replace target lines 2. save into text 3. write text back
            for (int i = 1; line != null; i++)
            {
                if (i != startLine)
                    text += line + "\r\n";
                else
                    text += newLines + "\r\n";
                line = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            FileStream fs1 = new FileStream(origintmpFile, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1, Encoding.GetEncoding("utf-8"));
            sw.Write(text);
            sw.Close();            
            fs1.Close();
        }
    }

    
}
