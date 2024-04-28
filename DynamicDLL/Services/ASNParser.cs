using DynamicDLL.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DynamicDLL.Services
{
    public class ASNParser
    {
        private string data;

        public ASNParser(string data)
        {
            this.data = data;
        }

        public List<string> GetBlocks()
        {
            Regex regexMainBlock = new Regex(@"(^|\s+)BEGIN((.*\n)+)(^|\s+)END", RegexOptions.Multiline);
            Regex regexComment = new Regex(@"(^|\s+)(\/|\*).*", RegexOptions.Multiline);
            Match mainBlock = regexMainBlock.Match(data);
            string[] lines = mainBlock.Value.Split('\n');
            List<string> blocks = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool read = false;

            foreach (string line in lines)
            {
                if (line.Contains("::="))
                {
                    if (sb.Length > 0)
                    {
                        blocks.Add(sb.ToString());
                    }
                    sb = new StringBuilder();
                    sb.Append(line);
                    read = true;
                }
                else if (regexComment.IsMatch(line) || line.Contains("END"))
                {
                    read = false;
                    continue;
                }
                else if (read)
                {
                    sb.Append(line);
                }
            }

            if (sb.Length > 0)
            {
                blocks.Add(sb.ToString());
            }

            sb.Clear();
            return blocks;
        }

        public ASNType ParseBlock(string data)
        {
            string blockName = "";
            Type blockType;
            List<ASNTypeProperty> properties = new List<ASNTypeProperty>();
            string[] blockLines = data.Split('\r');

            foreach (string line in blockLines)
            {
                if (line.Contains("::="))
                {
                    string[] parts = line.Split("::=");
                    blockName = parts[0].Trim();
                    blockType = Constants.asnTypeMapping[parts[1].Trim().ToUpper()];
                    continue;
                }

                string[] lineParts = line.Split(' ');

                foreach(string part in lineParts)
                {
                    
                }

            }

            //TODO: Parsing logic schould be here

            ASNType aSNType = new ASNType(blockName);
            aSNType.AddProperty("range", typeof(int));
            aSNType.AddProperty("name", typeof(string));
            aSNType.AddProperty("message", typeof(string));
            aSNType.AddProperty("fuel", typeof (string), new List<object> { "solid", "liquid", "gas"});
            aSNType.AddProperty("speed", typeof(string), new List<object> { "mph", "kmph" });
            aSNType.AddProperty("payload", typeof(List<string>));

            return aSNType;
        }
    }
}
