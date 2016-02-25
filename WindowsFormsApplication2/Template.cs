using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinFormScaffolding
{
    class FormGenerator
    {
        int x = 0;
        int y = 0;
        static int formx = 1326;

        enum ContoleType { Label, Text, Combo, Grid }

        string addCtr = "this.Controls.Add(this.";
        string defLblCtr = "private System.Windows.Forms.Label ";
        string defTxtCtr = "private System.Windows.Forms.TextBox ";
        string defCmbCtr = "private System.Windows.Forms.ComboBox ";
        string defGridCtr = "private System.Windows.Forms.DataGridView ";
        string defBtnCtr = "private System.Windows.Forms.Button ";

        string addNewLblCtr = "this.#val# = new System.Windows.Forms.Label();";
        string addNewTxtCtr = "this.#val# = new System.Windows.Forms.TextBox();";
        string addNewCmbCtr = "this.#val# = new System.Windows.Forms.ComboBox();";
        string addNewGridCtr = "this.#val# = new System.Windows.Forms.DataGridView();";
        string addNewBtnCtr = "this.#val# = new System.Windows.Forms.Button();";


        string addLblOptions = @"     
            this.#nameVal#.AutoSize = true;
            this.#nameVal#.Location = new System.Drawing.Point(#x#, #y#);
            this.#nameVal#.Name = ""#nameVal#"";
            this.#nameVal#.Size = new System.Drawing.Size(#widthVal#, 23);
            this.#nameVal#.TabIndex = 0;
            this.#nameVal#.Text = ""#textVal#""";

        string addTxtOptions = @"
            this.#nameVal#.Location = new System.Drawing.Point(#x#, #y#);
            this.#nameVal#.Name = ""#nameVal#"";
            this.#nameVal#.Size = new System.Drawing.Size(100, 22);
            this.#nameVal#.TabIndex = 1";


        string addCmbOptions = @"
            this.#nameVal#.FormattingEnabled = true;
            this.#nameVal#.Location = new System.Drawing.Point(#x#, #y#);
            this.#nameVal#.Name = ""#nameVal#"";
            this.#nameVal#.Size = new System.Drawing.Size(121, 24);
            this.#nameVal#.TabIndex = 0";

        string addGridOption = @"
            this.#nameVal#.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.#nameVal#.Location = new System.Drawing.Point(#x#, #y#);
            this.#nameVal#.Name = ""#nameVal#"";
            this.#nameVal#.Size = new System.Drawing.Size(798, 150);
            this.#nameVal#.TabIndex = 0";

        string addBtnOption = @"
            this.#nameVal#.Location = new System.Drawing.Point(432, 81);
            this.#nameVal#.Name = ""#nameVal#"";
            this.#nameVal#.Size = new System.Drawing.Size(75, 36);
            this.#nameVal#.TabIndex = 2;
            this.#nameVal#.Text = ""#textVal#"";
            this.#nameVal#.Click += new System.EventHandler(this.#nameVal#_Click);
            this.#nameVal#.UseVisualStyleBackColor = true
        ";



        string temp0 = @"
namespace ##namespace##
{
    partial class ##FormName##
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

       private void InitializeComponent()
       {

        //##Components0##

        //##Components##

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(##Formx##,##Formy##);
            this.Name = ""##FormName##/"";
            this.Text = ""##FormName##"";
            this.Load += new System.EventHandler(this.##FormName##_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
    }

    

        //##DefComponents##
        //private System.Windows.Forms.Label lblLabel1;
        //private System.Windows.Forms.TextBox txtTextBox1;
}
}";

        public Scaffolding.struc[] GenerateFormDesign(Scaffolding.struc[] forms)
        {

            string[] res = new string[forms.Length];
            for (int i = 0; i < forms.Length; i++)
            {

                var ass = forms[i].Ass.Select(p => p.Dependent).ToList();
                var prop = forms[i].Props.Select(p=>p.Name).Except(ass).ToList();

                string temp = temp0;

                //set nameSpace
                temp = temp.Replace("##FormName##", forms[i].Fname).Replace("##namespace##", forms[i].NameSpace == null ? "Generated" : forms[i].NameSpace);
                //GenerateControl
                temp = GenerateControl(temp, ass, ContoleType.Combo);
                temp = GenerateControl(temp, prop, ContoleType.Text);
                //Create GridView For Each Form
                temp = CreateGridView(temp, forms[i]);
                //set FormSize
                temp = temp.Replace("##Formx##", formx.ToString()).Replace("##Formy##", ((y + 1000).ToString()));

                res[i] = temp;
                forms[i].Design = temp;
            }

            return forms;
        }

        private string GenerateControl(string templateText, List<string> fields, ContoleType ctrType)
        {
            string temp = templateText;

            x = 0;
            y = 0;

            foreach (string t in fields)
            {
                string text = t.ToString().Trim();

                //.....................................................................................
                //.....................................................................................

                //Define Label
                //.....................................................................................
                temp = temp.Replace("//##Components0##", addNewLblCtr.Replace("#val#", "lbl" + text) + Environment.NewLine + "//##Components0##");

                temp = temp.Replace("//##Components0##",
                addLblOptions.Replace("#nameVal#", "lbl" + text)
               .Replace("#textVal#", text)
               .Replace("#x#", x.ToString())
               .Replace("#y#", y.ToString())
               .Replace("#widthVal#", (text.Length + 5).ToString()) + ";" + Environment.NewLine + "//##Components0##");

                UpdateSize(text.Length, ref x, ref y);

                temp = temp.Replace("//##DefComponents##", defLblCtr + "lbl" + text + ";" + Environment.NewLine + "//##DefComponents##");

                temp = temp.Replace("//##Components##", addCtr + "lbl" + text + ");" + Environment.NewLine + "//##Components##");

                //Define ComboBox
                //.....................................................................................
                if (ctrType == ContoleType.Combo)
                {
                    temp = temp.Replace("//##Components0##", addNewCmbCtr.Replace("#val#", "cmb" + text) + Environment.NewLine + "//##Components0##");

                    temp = temp.Replace("//##Components0##", addCmbOptions.Replace("#nameVal#", "cmb" + text)
                   .Replace("#x#", x.ToString())
                   .Replace("#y#", y.ToString())
                   + ";" + Environment.NewLine + "//##Components0##");

                    UpdateSize(text.Length + 100, ref x, ref y);

                    temp = temp.Replace("//##DefComponents##", defCmbCtr + "cmb" + text + ";" + Environment.NewLine + "//##DefComponents##");

                    temp = temp.Replace("//##Components##", addCtr + "cmb" + text + ");" + Environment.NewLine + "//##Components##");
                }
                //.....................................................................................

                //Define TextBox
                //.....................................................................................
                if (ctrType == ContoleType.Text)
                {
                    temp = temp.Replace("//##Components0##", addNewTxtCtr.Replace("#val#", "txt" + text) + Environment.NewLine + "//##Components0##");

                    temp = temp.Replace("//##Components0##", addTxtOptions.Replace("#nameVal#", "txt" + text)
                   .Replace("#x#", x.ToString())
                   .Replace("#y#", y.ToString())
                    + ";" + Environment.NewLine + "//##Components0##");

                    UpdateSize(text.Length + 100, ref x, ref y);

                    temp = temp.Replace("//##DefComponents##", defTxtCtr + "txt" + text + ";" + Environment.NewLine + "//##DefComponents##");

                    temp = temp.Replace("//##Components##", addCtr + "txt" + text + ");" + Environment.NewLine + "//##Components##");
                }
                //.....................................................................................

            }

            return temp;
        }

        private string CreateGridView(string temp, Scaffolding.struc forms)
        {

            //Define GridView
            //.....................................................................................

            temp = temp.Replace("//##Components0##", addNewGridCtr.Replace("#val#", "grid" + forms.Fname) + Environment.NewLine + "//##Components0##");

            temp = temp.Replace("//##Components0##", addGridOption.Replace("#nameVal#", "grid" + forms.Fname)
           .Replace("#x#", x.ToString())
           .Replace("#y#", y.ToString())
            + ";" + Environment.NewLine + "//##Components0##");

            UpdateSize(150, ref x, ref y);

            temp = temp.Replace("//##DefComponents##", defGridCtr + "grid" + forms.Fname + ";" + Environment.NewLine + "//##DefComponents##");

            temp = temp.Replace("//##Components##", addCtr + "grid" + forms.Fname + ");" + Environment.NewLine + "//##Components##");

            //.....................................................................................

            //Define Btn
            //.....................................................................................

            temp = temp.Replace("//##Components0##", addNewBtnCtr.Replace("#val#", "btnInsert") + Environment.NewLine + "//##Components0##");

            temp = temp.Replace("//##Components0##", addBtnOption.Replace("#nameVal#", "btnInsert")
           .Replace("#x#", x.ToString())
           .Replace("#y#", y.ToString())
           .Replace("#textVal#", "Insert")
            + ";" + Environment.NewLine + "//##Components0##");

            UpdateSize(36, ref x, ref y);

            temp = temp.Replace("//##DefComponents##", defBtnCtr + "btnInsert" + ";" + Environment.NewLine + "//##DefComponents##");

            temp = temp.Replace("//##Components##", addCtr + "btnInsert" + ");" + Environment.NewLine + "//##Components##");

            //.....................................................................................
            //.....................................................................................

            temp = temp.Replace("//##Components0##", addNewBtnCtr.Replace("#val#", "btnUpdate") + Environment.NewLine + "//##Components0##");

            temp = temp.Replace("//##Components0##", addBtnOption.Replace("#nameVal#", "btnUpdate")
           .Replace("#x#", (x + 50).ToString())
           .Replace("#y#", y.ToString())
           .Replace("#textVal#", "Update")
            + ";" + Environment.NewLine + "//##Components0##");

            UpdateSize(36, ref x, ref y);

            temp = temp.Replace("//##DefComponents##", defBtnCtr + "btnUpdate" + ";" + Environment.NewLine + "//##DefComponents##");

            temp = temp.Replace("//##Components##", addCtr + "btnUpdate" + ");" + Environment.NewLine + "//##Components##");

            //.....................................................................................
            //.....................................................................................

            temp = temp.Replace("//##Components0##", addNewBtnCtr.Replace("#val#", "btnDelete") + Environment.NewLine + "//##Components0##");

            temp = temp.Replace("//##Components0##", addBtnOption.Replace("#nameVal#", "btnDelete")
           .Replace("#x#", (x + 100).ToString())
           .Replace("#y#", y.ToString())
           .Replace("#textVal#", "Delete")
            + ";" + Environment.NewLine + "//##Components0##");

            UpdateSize(36, ref x, ref y);

            temp = temp.Replace("//##DefComponents##", defBtnCtr + "btnDelete" + ";" + Environment.NewLine + "//##DefComponents##");

            temp = temp.Replace("//##Components##", addCtr + "btnDelete" + ");" + Environment.NewLine + "//##Components##");

            //.....................................................................................
            //.....................................................................................


            return temp;
        }

        private void UpdateSize(int len, ref int x, ref int y)
        {
            y += (25);
        }
    }

    class CodeGenerator
    {

        string template = @"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ##NameSpace##
{
    public partial class ##FormName## : Form
    {
        public ##FormName##()
        {
            InitializeComponent();
        }

        //##PublicCode##

        private void ##FormName##_Load(object sender, EventArgs e)
        {
            //##Code##
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            //##BtnInsertCode##
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //##BtnUpdateCode##
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //##BtnDeleteCode##
        }
    }
}

";

        string database = "##EntitiyName## db = new ##EntitiyName##();";
        string databaseObject = "##EntitiyName##.##ObjectName##";


        public Scaffolding.struc[] GenerateClass(Scaffolding.struc[] forms, string entitiyName)
        {
            string[] res = new string[forms.Length];
            for (int i = 0; i < forms.Length; i++)
            {

                var ass = forms[i].Ass.ToList();

                string tCode = "";
                string temp = template;
                string temp2 = "";

                //set FormName
                temp = temp.Replace("##FormName##", forms[i].Fname).Replace("##NameSpace##", forms[i].NameSpace == null ? "Generated" : forms[i].NameSpace);
                temp = temp.Replace("//##PublicCode##", database.Replace("##EntitiyName##", entitiyName) + Environment.NewLine + "//##PublicCode##");

                foreach (var t in ass)
                {
                    tCode = "cmb" + t.Dependent + ".DataSource = "
                       + databaseObject.Replace("##EntitiyName##", "db")
                       .Replace("##ObjectName##", t.PrincipalTableName)
                       + ".ToList();";

                    temp = temp.Replace("//##Code##", tCode + Environment.NewLine + "//##Code##");

                    tCode = "cmb" + t.Dependent + ".ValueMember = " + '"' + t.Principal + '"' + ';';
                    temp = temp.Replace("//##Code##", tCode + Environment.NewLine + "//##Code##");

                    var tt = forms.Where(p => p.TableName == t.PrincipalTableName).FirstOrDefault().Props.Select(p=>p.Name).ElementAt(1);
                    tCode = "cmb" + t.Dependent + ".DisplayMember = " + '"' + tt + '"' + ';';
                    temp = temp.Replace("//##Code##", tCode + Environment.NewLine + "//##Code##");

                    temp2 += ".Include(p=>p."
                    + t.PrincipalWithoutRoleTableName
                    + ")";

                }

                //.......................GridView.........................................
                //.......................................................................

                tCode = "grid" + forms[i].Fname + ".DataSource = "
                + databaseObject.Replace("##EntitiyName##", "db")
                .Replace("##ObjectName##", forms[i].ModelName)
                + temp2
                + ".ToList();";
                temp = temp.Replace("//##Code##", tCode + Environment.NewLine + "//##Code##");

                //.......................................................................
                //.......................BtnInsert.........................................
                //.......................................................................

                //var ass = forms[i].Ass.Select(p => p.Dependent).ToList();
                //var prop = forms[i].Props.Select(p=>p.Name).Except(ass).ToList();

                string convert;

                tCode = "  db.#ModelName#.Add(new #TableName# { #modelObject# })"; 
                 tCode = tCode.Replace("#TableName#", forms[i].TableName);
                tCode = tCode.Replace("#ModelName#", forms[i].ModelName);
                for (int k = 0; k < forms[i].Props.Count; k++)
                {
                    convert = Convertor(forms[i].Props.Select(p=>p.Type).ElementAt(k));

                    var tKey = forms[i].Props.Select(p=>p.Name).ElementAt(k);
                    var tAss = forms[i].Ass.Where(p => p.Dependent == tKey).FirstOrDefault();

                    if (string.IsNullOrEmpty(convert) && forms[i].Props.Select(p => p.StoreGeneratedPattern).ElementAt(k)!= "Identity")
                    {
                        tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = " + (tAss == null ? "txt" : "cmb") + forms[i].Props.Select(p=>p.Name).ElementAt(k) + (tAss == null ? ".Text" : ".SelectedValue") + ",#modelObject#");
                    }

                    if (!string.IsNullOrEmpty(convert) && forms[i].Props.Select(p => p.StoreGeneratedPattern).ElementAt(k) != "Identity")
                        tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = " + convert + (tAss == null ? "txt" : "cmb") + forms[i].Props.Select(p=>p.Name).ElementAt(k) + (tAss == null ? ".Text" : ".SelectedValue") + ")" + ",#modelObject#");
                    //tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = " + convert + "txt" + forms[i].Props.Select(p=>p.Name).ElementAt(k) + ".Text" + ")" + ",#modelObject#");
                }
                tCode = tCode.Replace(",#modelObject#", "");

                temp = temp.Replace("//##BtnInsertCode##", tCode + ";" + Environment.NewLine);

                //.......................................................................
                //.......................................................................
                //.......................BtnUpdate.........................................
                //.......................................................................

                tCode = @"  var obj = new #TableName#{ #modelObject# };
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges()";

                tCode = tCode.Replace("#TableName#", forms[i].TableName);

                for (int k = 0; k < forms[i].Props.Count; k++)
                {
                    //tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = txt" + forms[i].Props.Select(p=>p.Name).ElementAt(k) + ".Text" + ",#modelObject#");

                    convert = Convertor(forms[i].Props.Select(p=>p.Type).ElementAt(k));

                    var tKey = forms[i].Props.Select(p=>p.Name).ElementAt(k);
                    var tAss = forms[i].Ass.Where(p => p.Dependent == tKey).FirstOrDefault();

                    if (string.IsNullOrEmpty(convert))
                        tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = " + (tAss == null ? "txt" : "cmb") + forms[i].Props.Select(p=>p.Name).ElementAt(k) + (tAss == null ? ".Text" : ".SelectedValue") + ",#modelObject#");

                    if (!string.IsNullOrEmpty(convert))
                        tCode = tCode.Replace("#modelObject#", forms[i].Props.Select(p=>p.Name).ElementAt(k) + " = " + convert + (tAss == null ? "txt" : "cmb")  + forms[i].Props.Select(p=>p.Name).ElementAt(k) + (tAss == null ? ".Text" : ".SelectedValue") + ")" + ",#modelObject#");

                }

                tCode = tCode.Replace(",#modelObject#", "");

                temp = temp.Replace("//##BtnUpdateCode##", tCode + ";" + Environment.NewLine);

                //.......................................................................

                forms[i].Code = temp;
                res[i] = temp;
            }

            return forms;
        }

        private string Convertor(string vType)
        {
            string res = null;

            if (vType.Contains("nvarchar"))
                res = "";

            if (vType.Contains("varchar"))
                res = "";

            if (vType.Contains("int"))
                res = "Convert.ToInt32(";

            if (vType.Contains("float"))
                res = "Convert.ToInt32(";

            if (vType.Contains("date"))
                res = "DateTime.Parse(";

            if (vType.Contains("bit"))
                res = "bool.Parse(";

            return res;
        }
    }
}
