using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormScaffolding
{
    class Patterns
    {
        public string ConceptualModels { get { return @"<edmx:ConceptualModels>(.*)</edmx:ConceptualModels>"; } }

        public string EntityType { get { return @"<EntityType Name=\W?\w*\W?>"; } }

        public string GetValue { get { return @""".*"""; } }

        public string Association { get { return @"<Association Name=\W?\w*\W?>"; } }

        public string EntitiyName { get { return @"<\s*EntityContainer\s*Name\s*=\s*"".*""\s*annotation:LazyLoadingEnabled="; } }

        public string EntityContainer { get { return @"<\s*EntityContainer Name.*>.*<\s*/EntityContainer.*"; } }

        public string End { get { return @"\s*<End\s*Role="".*"" EntitySet="".*""\s*/>"; } }

        public string Role { get { return @"Role=""\w*"""; } }

        public string EntitySet { get { return @"EntitySet=""\w*"""; } }

        public string ReferentialConstraint { get { return @"<ReferentialConstraint>.*"; } }

        public string Principal { get { return @"<Principal Role=\W?\w*\W?>"; } }

        public string PropertyRef { get { return @".*<PropertyRef Name=\W?\w*\W?.*>"; } }

        public string Dependent { get { return @"<Dependent Role=\W?\w*\W?.*>.*</Dependent>"; } }

        public string Property { get { return @"<Property Name=""\w*"".*>"; } }

        public string Name { get { return @"Name=""\w*"""; } }

        public string Type { get { return @"Type="".*"""; } }

        public System.Text.RegularExpressions.MatchCollection GetMatches(string data, string pattern, System.Text.RegularExpressions.RegexOptions? option=null)
        {
            System.Text.RegularExpressions.Regex regex;
            if (!option.HasValue)
                regex = new System.Text.RegularExpressions.Regex(pattern);
            else
                regex = new System.Text.RegularExpressions.Regex(pattern, option.Value);

            return regex.Matches(data);
        }

        public System.Text.RegularExpressions.Match GetMatch(string data, string pattern, System.Text.RegularExpressions.RegexOptions? option=null)
        {
            System.Text.RegularExpressions.Regex regex;
            if (!option.HasValue)
                regex = new System.Text.RegularExpressions.Regex(pattern);
            else
                regex = new System.Text.RegularExpressions.Regex(pattern, option.Value);

            return regex.Match(data);
        }
    }
}
