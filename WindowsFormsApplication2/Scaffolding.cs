using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinFormScaffolding
{
    public class Scaffolding
    {
        public class struc
        {
            public string NameSpace { get; set; }
            public string Fname { get; set; }
            public string TableName { get; set; }
            public Dictionary<string, string> Props { get; set; }
            public AssociationStruct[] Ass { get; set; }
            public string Code { get; set; }
            public string Design { get; set; }
            public string ModelName { get; set; }
        }

        public class AssociationStruct
        {
            public string Dependent { get; set; }
            public string Principal { get; set; }
            public string PrincipalTableName { get; set; }
            public string PrincipalWithoutRoleTableName { get; set; }
            public string DependentTableName { get; set; }
            public string DependentWithoutRolTableName { get; set; }
        }

        public string OutPutAddress { get; set; }
        public string NameSpace { get; set; }

        public struc[] ParsedData { get; set; }

        public Scaffolding()
        {
            OutPutAddress = Environment.CurrentDirectory + "\\Forms";

            if (!Directory.Exists(OutPutAddress))
                Directory.CreateDirectory(OutPutAddress);

        }

        Patterns regexPatterns = new Patterns();

        public bool Pars(string modelPath)
        {
            StreamReader sr = new StreamReader(modelPath);
            string data = sr.ReadToEnd();
            sr.Close();

            string ConceptualSection = regexPatterns.GetMatch(data, regexPatterns.ConceptualModels, RegexOptions.Singleline).Value;
            var dic = GetsRoles(ConceptualSection);

            var matchesEntityType = regexPatterns.GetMatches(data, regexPatterns.EntityType);

            string[] entityTypeList = Regex.Split(data, "</EntityType>");
            string[] associationList = Regex.Split(data, "</Association>");

            Regex association = new Regex(regexPatterns.Association);


            Queue<struc> result = new Queue<struc>();
            Queue<AssociationStruct> associationResult = new Queue<AssociationStruct>();

            for (int k = 0; k < associationList.Length; k++)
            {
                matchesEntityType = regexPatterns.GetMatches(associationList[k], regexPatterns.Association);
                associationResult.Enqueue(GetAssociation(associationList[k]));
            }


            for (int k = 0; k < entityTypeList.Length; k++)
            {
                matchesEntityType = regexPatterns.GetMatches(entityTypeList[k], regexPatterns.EntityType);
                for (int i = 0; i < matchesEntityType.Count; i++)
                {
                    struc te = new struc();
                    te.Fname = Filter(matchesEntityType[i].Value);

                    var p2 = regexPatterns.GetMatches(matchesEntityType[i].Value, regexPatterns.GetValue);

                    for (int j = 0; j < p2.Count; j++)
                    {
                        te.Fname = Filter(p2[i].Value);
                    }

                    te.Props = GetProperty(entityTypeList[k]);

                    if (result.Where(p => p.Fname == te.Fname).FirstOrDefault() != null)
                        continue;

                    if (!string.IsNullOrEmpty(NameSpace))
                        te.NameSpace = NameSpace;

                    te.ModelName = te.Fname;
                    te.TableName = te.Fname;
                    te.Fname = "frm" + te.Fname;
       
                    result.Enqueue(te);
                }
            }

            var filter = Distinct(result.ToArray());
            ParsedData = filter;
            ProcessAssociation(filter, associationResult.ToArray());

            Translate(filter, dic);

            FormGenerator generate = new FormGenerator();
            var designFile = generate.GenerateFormDesign(filter);

            foreach (struc t in designFile)
            {
                if (File.Exists(OutPutAddress + "\\" + t.Fname + ".Designer.cs"))
                    File.Delete(OutPutAddress + "\\" + t.Fname + ".Designer.cs");

                StreamWriter sw = new StreamWriter(OutPutAddress + "\\" + t.Fname + ".Designer.cs", true);
                sw.Write(t.Design);
                sw.Close();

            }

            var efName = regexPatterns.GetMatches(regexPatterns.GetMatch(data, regexPatterns.EntitiyName).Value, regexPatterns.GetValue)[0].Value.Replace('"', ' ').Trim();

            CodeGenerator code = new CodeGenerator();
            var codeFile = code.GenerateClass(filter, efName);

            foreach (struc t in codeFile)
            {
                if (File.Exists(OutPutAddress + "\\" + t.Fname + ".cs"))
                    File.Delete(OutPutAddress + "\\" + t.Fname + ".cs");

                StreamWriter sw = new StreamWriter(OutPutAddress + "\\" + t.Fname + ".cs", true);
                sw.Write(t.Code);
                sw.Close();

            }

            for (int i = 0; i < result.Count; i++)
            {
                if (File.Exists(OutPutAddress + "\\" + result.ElementAt(i).Fname + ".resx"))
                    File.Delete(OutPutAddress + "\\" + result.ElementAt(i).Fname + ".resx");

                File.Copy(Environment.CurrentDirectory + "\\RFile.resx", OutPutAddress + "\\" + result.ElementAt(i).Fname + ".resx");
            }

            return true;
        }

        public Dictionary<string, string> GetsRoles(string data)
        {
            string temp = regexPatterns.GetMatch(data, regexPatterns.EntityContainer, RegexOptions.Singleline).Value;
            var temp2 = regexPatterns.GetMatches(temp, regexPatterns.End);

            Dictionary<string, string> roles = new Dictionary<string, string>();
            for (int i = 0; i < temp2.Count; i++)
            {
                string tRole = regexPatterns.GetMatch(regexPatterns.GetMatch(temp2[i].Value, regexPatterns.Role).Value, regexPatterns.GetValue).Value.Replace('"', ' ').Trim();
                string tEntitySet = regexPatterns.GetMatch(regexPatterns.GetMatch(temp2[i].Value, regexPatterns.EntitySet).Value, regexPatterns.GetValue).Value.Replace('"', ' ').Trim();

                if (roles.Keys.Where(p => p == tRole).FirstOrDefault() == null)
                    roles.Add(tRole, tEntitySet);
            }

            return roles;
        }

        private void Translate(struc[] parsedData, Dictionary<string, string> dic)
        {
            for (int i = 0; i < parsedData.Length; i++)
            {
                for (int j = 0; j < parsedData[i].Ass.Length; j++)
                {
                    var temp = dic.Where(p => p.Key == parsedData[i].Ass[j].DependentTableName).FirstOrDefault();
                    if (temp.Value != null)
                    {
                        parsedData[i].Ass[j].DependentTableName = temp.Value;
                    }

                    temp = dic.Where(p => p.Key == parsedData[i].Ass[j].PrincipalTableName).FirstOrDefault();
                    if (temp.Value != null)
                    {
                        parsedData[i].Ass[j].PrincipalTableName = temp.Value;
                    }
                }

                var temp2 = dic.Where(p => p.Key == parsedData[i].TableName).FirstOrDefault();
                if (temp2.Value != null)
                {
                    parsedData[i].TableName = temp2.Value;
                }
            }
        }

        private AssociationStruct GetAssociation(string data)
        {
            AssociationStruct st = new AssociationStruct();
            Regex filterName = new Regex(regexPatterns.GetValue);

            //.......................Dependent...........................
            //...........................................................

            var matchValue = regexPatterns.GetMatches(data, regexPatterns.ReferentialConstraint, RegexOptions.Singleline);
            if (matchValue.Count > 0)
                data = matchValue[0].Value;


            matchValue = regexPatterns.GetMatches(data, regexPatterns.Principal);
            if (matchValue.Count > 0)
            {
                st.PrincipalTableName = Filter(regexPatterns.GetMatch(matchValue[0].Value, regexPatterns.GetValue).Value);
                st.PrincipalWithoutRoleTableName = Filter(regexPatterns.GetMatch(matchValue[0].Value, regexPatterns.GetValue).Value);
            }

            Regex regPrincipal = new Regex(@".*</Principal>", RegexOptions.Singleline);

            matchValue = regPrincipal.Matches(data);

            if (matchValue.Count > 0)
            {
                var tMatchValue = regexPatterns.GetMatches(matchValue[0].Value, regexPatterns.PropertyRef);
                if (tMatchValue.Count > 0)
                    st.Principal = Filter(regexPatterns.GetMatch(tMatchValue[0].Value, regexPatterns.GetValue).Value);
            }

            //.......................Dependent...........................
            //...........................................................

            Regex regTbName = new Regex(@"<Dependent Role=\W?.*\W?>");
            matchValue = regTbName.Matches(data);
            if (matchValue.Count > 0)
            {
                st.DependentTableName = Filter(filterName.Match(matchValue[0].Value).Value);
                st.DependentWithoutRolTableName = Filter(filterName.Match(matchValue[0].Value).Value);
            }

            matchValue = regexPatterns.GetMatches(data, regexPatterns.Dependent, RegexOptions.Singleline);
            if (matchValue.Count > 0)
            {
                var tMatchValue = regexPatterns.GetMatches(matchValue[0].Value, regexPatterns.PropertyRef);
                st.Dependent = Filter(regexPatterns.GetMatch(tMatchValue[0].Value, regexPatterns.GetValue).Value);
            }

            return st;
        }

        private void ProcessAssociation(struc[] data, AssociationStruct[] associatioData)
        {
            for (int i = 0; i < data.Length; i++)
            {
                var tbName = data[i].TableName;
                var association = associatioData.Where(p => p.DependentTableName == tbName).Distinct().ToArray();
                data[i].Ass = Distinct(association);
            }
        }

        private AssociationStruct[] Distinct(AssociationStruct[] input)
        {
            Queue<AssociationStruct> res = new Queue<AssociationStruct>();
            for (int i = 0; i < input.Length; i++)
            {
                if (res.Where(p => p.Dependent == input[i].Dependent
                    && p.DependentTableName == input[i].DependentTableName
                    && p.Principal == input[i].Principal
                    && p.PrincipalTableName == input[i].PrincipalTableName).FirstOrDefault() == null)
                    res.Enqueue(input[i]);
            }
            return res.ToArray();
        }

        private struc[] Distinct(struc[] input)
        {
            Queue<struc> res = new Queue<struc>();
            for (int i = 0; i < input.Length; i++)
            {
                if (res.Where(p => p.Fname == input[i].Fname).FirstOrDefault() == null)
                    res.Enqueue(input[i]);
            }
            return res.ToArray();
        }

        private Dictionary<string, string> GetProperty(string data)
        {
            var matches = regexPatterns.GetMatches(data, regexPatterns.Property);

            Dictionary<string, string> property = new Dictionary<string, string>();

            for (int i = 0; i < matches.Count; i++)
            {
                //listBox1.Items.Add(matches[i].Value);

                property.Add(SubTree(matches[i].Value, regexPatterns.Name), SubTree(matches[i].Value, regexPatterns.Type));
            }

            return property;
        }

        private string SubTree(string data, string pattern)
        {
            var temp = regexPatterns.GetMatches(data, pattern, RegexOptions.Singleline);
            var res = regexPatterns.GetMatches(temp[0].Value, regexPatterns.GetValue);

            return Filter(res[0].Value);
        }

        private string Filter(string input)
        {
            return input.Replace('"', ' ').Trim();
        }
    }
}
