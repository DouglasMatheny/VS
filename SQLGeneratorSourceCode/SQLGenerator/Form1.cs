using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Text.RegularExpressions;
using SQLGenerator.BOs;
using System.IO;
using SQLUserControl;
using System.Configuration;

namespace SQLGenerator
{
    public partial class Form1 : Form
    {
        private TableObjects tableObjects;
        public Form1()
        {
            InitializeComponent();
        }

        private void openSelectScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.panelFields.Controls.Clear();
            this.panelFields.Visible = false;
            this.panelDisp.Visible = false;

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = ConfigurationManager.AppSettings["InitDir"];
            fileDialog.Filter = @"SQL files (*.sql)|*.sql|All files (*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = ReadFile(fileDialog.FileName);
                TableObjects tableObjects = ParseSelectStatement(file);
                int i = 0;
                foreach (Field f in tableObjects.Fields)
                {
                    fieldControl fc = new fieldControl();
                    
                    fc.Location = new Point(0, i);
                    fc.lblName.Text = f.Name;
                    this.panelFields.Controls.Add(fc);
                    i = i + 30;
                }
                this.panelFields.Refresh();
                this.btnGenerateSQL.Visible = true;
                this.panelFields.Visible = true;
                this.panelDisp.Visible = true;
                this.label5.Text = tableObjects.TableName;
            }
        }

        private TableObjects ParseSelectStatement(string SelectString)
        {
            tableObjects = new TableObjects();
            if (SelectString.ToUpper().IndexOf(" FROM ") > 0)
            {
                tableObjects.TableName = SelectString.ToUpper().Substring(SelectString.ToUpper().IndexOf(" FROM ") + 6, SelectString.Length - (SelectString.ToUpper().IndexOf(" FROM ") + 6));
            }
            else
            {
                tableObjects.TableName = SelectString.ToUpper().Substring(SelectString.ToUpper().IndexOf("FROM ") + 5, SelectString.Length - (SelectString.ToUpper().IndexOf("FROM ") + 5));
            }
            //tableObjects.TableName = SelectString.ToUpper().Substring(SelectString.ToUpper().IndexOf(" FROM ") + 6, SelectString.Length - (SelectString.ToUpper().IndexOf(" FROM ") + 6));
            tableObjects.TableName = tableObjects.TableName.ToUpper().Replace(";", "").Replace("LIMIT 100", "").Replace(" ", "").Replace("ADMIN.", "").Replace(@" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", ""); ;
            if (SelectString.ToUpper().IndexOf(" FROM ") > 0)
            {
                SelectString = SelectString.ToUpper().Remove(SelectString.ToUpper().IndexOf(" FROM "), SelectString.Length - (SelectString.ToUpper().IndexOf(" FROM ")));
            }
            else
            {
                SelectString = SelectString.ToUpper().Remove(SelectString.ToUpper().IndexOf("FROM "), SelectString.Length - (SelectString.ToUpper().IndexOf("FROM ")));
            }
            SelectString = SelectString.ToUpper().Replace(@"SELECT", "").Replace(@";", "").Replace(@" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            if (tableObjects.TableName.ToUpper().IndexOf("..") > 0)
                tableObjects.TableName = tableObjects.TableName.ToUpper().Remove(0, (tableObjects.TableName.ToUpper().IndexOf("..")+2));
            SelectString = SelectString.ToUpper().Replace(@".", "");
            //tableObjects.TableName = SelectString.ToUpper().Substring(SelectString.ToUpper().IndexOf("FROM") + 4, SelectString.Length - (SelectString.ToUpper().IndexOf("FROM") + 4));
            ///SelectString = SelectString.ToUpper().Remove(SelectString.ToUpper().IndexOf("FROM" + tableObjects.TableName));
            tableObjects.Fields = new List<Field>();
            string[] sarray = SelectString.Split(',');
            foreach(string s in sarray)
            {
                Field f = new Field();
                f.Name = s;
                tableObjects.Fields.Add(f);
            }
            return tableObjects;

        }

        public string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string data = sr.ReadToEnd();
            sr.Close();
            return data;
        }

        private void btnGenerateSQL_Click(object sender, EventArgs e)
        {

            string Source1Prefix = ConfigurationManager.AppSettings["Source1Prefix"];
            string Source2Prefix = ConfigurationManager.AppSettings["Source2Prefix"];
            string OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
            string TempTablePrefix = ConfigurationManager.AppSettings["TempTablePrefix"]; 
            string Source1TableName = "";
            string Source2TableName = "";
            string seed  = "";
            string Source1Counts = "";
            string Source2Counts = "";

            Dictionary<int,string> pk = new Dictionary<int,string>();
            StringBuilder sbPKJoins = new StringBuilder();
            StringBuilder sbPKGroupBy = new StringBuilder();
            StringBuilder sbDupJoins = new StringBuilder();
            StringBuilder sbDupGroupBy = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbColumnDetailQuery = new StringBuilder();
            int k = 0;

            tableObjects.Fields.Clear();

            int i = 0;
            foreach (Control c in this.panelFields.Controls)
            {
                Field f = new Field();
                fieldControl fc = (fieldControl)c;
                f.Name = fc.lblName.Text;
                f.PrimaryKey = fc.chkPK.Checked;
                f.Exclude = fc.chkExclude.Checked;
                f.Seed = fc.chkSeed.Checked;

                if (f.Seed)
                    seed = f.Name;
                tableObjects.Fields.Add(f);

                if (f.PrimaryKey)
                {
                    pk.Add(i, f.Name);
                    i++;
                }
               
            }

            for(int j=0; j < pk.Count; j++)
            {
                if(j == 0)
                {
                    sbPKJoins.Append("WHERE Source1." + pk[j].ToString() + " = Source2." + pk[j].ToString()+"\n");
                    sbPKGroupBy.Append(" "+pk[j].ToString());
                }
                else
                {
                    sbPKJoins.Append("AND Source1." + pk[j].ToString() + " = Source2." + pk[j].ToString()+"\n");
                    sbPKGroupBy.Append(", "+pk[j].ToString());
                }
            }


            foreach(Field f in tableObjects.Fields)
            {
                if (f.Exclude == false)
                {
                        if (k == 0)
                        {
                            sbDupJoins.Append("WHERE RTRIM(ISNULL(CAST(Source1." + f.Name + " AS VARCHAR(50)),'')) = " + "RTRIM(ISNULL(CAST(Source2." + f.Name + " AS VARCHAR(50)),''))\n");
                            sbDupGroupBy.Append(f.Name);
                        }
                        else
                        {
                            sbDupJoins.Append("AND RTRIM(ISNULL(CAST(Source1." + f.Name + " AS VARCHAR(50)),'')) = " + "RTRIM(ISNULL(CAST(Source2." + f.Name + " AS VARCHAR(50)),''))\n");
                            sbDupGroupBy.Append(", "+f.Name);
                        }
                        k++;
                }
            }
            k = 0;

            if (this.chkTemp.Checked)
            {
                //CREATE TEMP TABLES AND CHECK SAMPLE COUNTS
                Source1TableName = Source1Prefix + tableObjects.TableName;
                Source2TableName = Source2Prefix + tableObjects.TableName;
                sb.Append("--Populate temp table using sample data from Source1\n");
                sb.Append("SELECT *\n");
                sb.Append("INTO " + Source1TableName + "\n");
                sb.Append("FROM " + Source1Prefix + tableObjects.TableName + "\n");
                sb.Append("WHERE " + seed + " LIKE '%" + txtTempSeed.Text+"';\n");
                sb.Append("\n");
                sb.Append("--Populate temp table using sample data from Source2\n");
                sb.Append("SELECT *\n");
                sb.Append("INTO " + Source2TableName + "\n");
                sb.Append("FROM " + Source2Prefix + tableObjects.TableName + "\n");
                sb.Append("WHERE " + seed + " LIKE '%" + txtTempSeed.Text + "';\n");
                sb.Append("\n");
            }
            else
            {
                Source1TableName = Source1Prefix + tableObjects.TableName;
                Source2TableName = Source2Prefix + tableObjects.TableName;
            }

            //CREATE TABLES WITH Source1 and Source2 duplicates, so we can filter these out.
            sb.Append("\n");
            sb.Append("--Place all Source1 duplicates in a seperate table.\n");
            sb.Append("SELECT "+ sbPKGroupBy + ", COUNT(*) AS COUNTS \n");
            sb.Append("INTO " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS\n");
            sb.Append("FROM " + Source1TableName + "\n");
            sb.Append("GROUP BY "+ sbPKGroupBy+ " HAVING COUNT(*) > 1;\n");
            sb.Append("\n");
            sb.Append("--Place all Source2 duplicates in a seperate table.\n");
            sb.Append("SELECT " + sbPKGroupBy + ", COUNT(*)  AS COUNTS \n");
            sb.Append("INTO " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS\n");
            sb.Append("FROM " + Source2TableName + "\n");
            sb.Append("GROUP BY " + sbPKGroupBy + " HAVING COUNT(*) > 1;\n\n");


            //TABLE COUNTS
            sb.Append("--Put the table name in for the spreadsheet.\n");
            sb.Append("SELECT '(00)TABLENAME' AS KEY1,'" + tableObjects.TableName + "' AS VALUE\n");
            sb.Append("\nUNION\n");
            sb.Append("--Record Counts for Source1 table:" + tableObjects.TableName + "\n");
            sb.Append("SELECT '(01) Total Source1 Count' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + Source1Prefix + tableObjects.TableName + "\n");
            sb.Append("\nUNION\n");
            sb.Append("--Record Counts for Source2 table:" + tableObjects.TableName + "\n");
            sb.Append("SELECT '(02) Total Source2 Count' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + Source2Prefix + tableObjects.TableName + "\n");
            sb.Append("\nUNION\n");

            if (this.chkTemp.Checked)
            {
                sb.Append("--Record Counts for Source1 temp table table:" + Source1TableName + "\n");
                sb.Append("SELECT '(03) Source1 Sample Count' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                sb.Append("FROM " + Source1TableName + "\n");
                sb.Append("\nUNION\n");
                sb.Append("--Record Counts for Source2 temp table table:" + Source2TableName + "\n");
                sb.Append("SELECT '(04) Source2 Sample Count' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                sb.Append("FROM " + Source2TableName + "\n");
                sb.Append("UNION\n");

            }
            else
            {
                sb.Append("--Placeholder for temp table record counts(its there for the spreadsheet).\n");
                sb.Append("SELECT '(03) Source1 Sample Count' AS KEY1, CAST(0 AS VARCHAR(50)) AS VALUE\n");
                sb.Append("\nUNION\n");
                sb.Append("--Placeholder for temp table record counts(its there for the spreadsheet).\n");
                sb.Append("SELECT '(04) Source2 Sample Count' AS KEY1, CAST(0 AS VARCHAR(50)) AS VALUE\n");
                sb.Append("UNION\n");
            }
            //Source1 Duplicates
            sb.Append("--Source1 Duplicate Records\n");
            sb.Append("SELECT '(05) Source1 Duplicates' AS KEY1, CAST(SUM(COUNTS) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS\n");
            sb.Append("\nUNION\n");

            //Source2 Duplicates
            sb.Append("--Source2 Duplicate Records\n");
            sb.Append("SELECT '(06) Source2 Duplicates' AS KEY1, CAST(SUM(COUNTS) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS\n");
            sb.Append("\nUNION\n\n");
            //MATCHES
            sb.Append("--Matching Records\n");
            sb.Append("SELECT '(07) Primary Key Matches' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + Source1TableName + " Source1\n");
            sb.Append("WHERE EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source2TableName + " Source2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("\nUNION\n");



            //MISSING
            sb.Append("--Missing Records in Source2, not in Source1\n");
            sb.Append("SELECT '(08) Missing Records' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + Source2TableName + " Source2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source1TableName + " Source1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("\nUNION\n");

  
            //ADDITIONAL
            sb.Append("--Additional Records in Source1, not in Source2\n");
            sb.Append("SELECT '(10) Additional Records' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + Source1TableName + " Source1\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source2TableName + " Source2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            //DISPLAY PK COLUMNS
            foreach (Field f in tableObjects.Fields)
            {
                if (f.PrimaryKey == true)
                {
                    sb.Append("\nUNION\n");
                    sb.Append("--Column is part of PK:" + f.Name + "\n");
                    sb.Append("SELECT '" + f.Name + "' AS KEY1, 'PK' AS VALUE\n");
                    sb.Append("\n");
                }
            }
            //COLUMN VALIDATIONS
            foreach(Field f in tableObjects.Fields)
            {
                if (f.Exclude == false)
                {
                    if (f.PrimaryKey == false)
                    {
                        sb.Append("\nUNION\n");
                        sb.Append("--Validate the column:" + f.Name + "\n");
                        sb.Append("SELECT '" + f.Name + "' AS KEY1, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                        sb.Append("FROM " + Source1TableName + " Source1\n");
                        sb.Append("WHERE EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + Source2TableName + " Source2\n");
                        sb.Append(sbPKJoins.ToString());
                        sb.Append("--This is the column we are checking\n");
                        sb.Append("AND RTRIM(ISNULL(CAST(Source1." + f.Name + " AS VARCHAR(50)),'')) != " + "RTRIM(ISNULL(CAST(Source2." + f.Name + " AS VARCHAR(50)),''))\n");
                        sb.Append(")\n");
                        sb.Append("--Filter out any exact duplicates that exists on Source1\n");
                        sb.Append("AND NOT EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS Source2\n");
                        sb.Append(sbPKJoins.ToString());
                        sb.Append(")\n");
                        sb.Append("--Filter out any exact duplicates that exists on Source2\n");
                        sb.Append("AND NOT EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS Source2\n");
                        sb.Append(sbPKJoins.ToString());
                        sb.Append(")\n");

                        if (k == 0)
                        {
                            StringBuilder sbPKGroupDetBy = new StringBuilder();
                            StringBuilder sbPKJoins1 = new StringBuilder();
                            sbPKJoins1.Append(sbPKJoins);
                            sbPKJoins1.Replace("WHERE", "");
                            sbPKGroupDetBy.Append(sbPKGroupBy.ToString());
                            sbPKGroupDetBy.Replace(" "," Source1.");
                            sbColumnDetailQuery.Append("--Sample Query column difference details:" + f.Name + "\n");
                            sbColumnDetailQuery.Append("SELECT "+sbPKGroupDetBy+ ", Source1." + f.Name + " AS Source1_"+ f.Name + ", Source2." + f.Name + " AS Source2_"+ f.Name+"\n");
                            sbColumnDetailQuery.Append("FROM " + Source1TableName + " Source1\n");
                            sbColumnDetailQuery.Append("INNER JOIN " + Source2TableName + " Source2\n");
                            sbColumnDetailQuery.Append("ON(\n");
                            sbColumnDetailQuery.Append(sbPKJoins1.ToString());
                            sbColumnDetailQuery.Append(")\n");
                            sbColumnDetailQuery.Append("--This is the column we are checking\n");
                            sbColumnDetailQuery.Append("WHERE RTRIM(ISNULL(CAST(Source1." + f.Name + " AS VARCHAR(50)),'')) != " + "RTRIM(ISNULL(CAST(Source2." + f.Name + " AS VARCHAR(50)),''))\n");

                            sbColumnDetailQuery.Append("--Filter out any exact duplicates that exists on Source1\n");
                            sbColumnDetailQuery.Append("AND NOT EXISTS\n");
                            sbColumnDetailQuery.Append("(\n");
                            sbColumnDetailQuery.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS Source2\n");
                            sbColumnDetailQuery.Append(sbPKJoins.ToString());
                            sbColumnDetailQuery.Append(")\n");
                            sbColumnDetailQuery.Append("--Filter out any exact duplicates that exists on Source2\n");
                            sbColumnDetailQuery.Append("AND NOT EXISTS\n");
                            sbColumnDetailQuery.Append("(\n");
                            sbColumnDetailQuery.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS Source2\n");
                            sbColumnDetailQuery.Append(sbPKJoins.ToString());
                            sbColumnDetailQuery.Append(")\n");
                            sbColumnDetailQuery.Append("ORDER BY " + sbPKGroupDetBy + " LIMIT 1000;\n");
                            k++;
                        }
                    }
                }
 
            }
            sb.Append(";\n");
            //DROP THE TEMP TABLES
            sb.Append("--Cleanup temp tables\n");
            if (this.chkTemp.Checked)
            {
                sb.Append("DROP TABLE "+Source1TableName+";\n");
                sb.Append("\n");
                sb.Append("DROP TABLE " + Source2TableName + ";\n");
                sb.Append("\n");
            }
            sb.Append("DROP TABLE " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS;\n");
            sb.Append("\n");
            sb.Append("DROP TABLE " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS;\n");
            sb.Append("\n");
            //Detailed Queries

            //MISSING ORG_PART_KEYS

            sb.Append("/*\n");
            sb.Append("--This is the old way I was doing it which is not entirely accurate\n");
            sb.Append("--Missing Records Counts BY ORG_PART_KEY\n");
            sb.Append("SELECT ORG_PART_KEY, COUNT(*)\n");
            sb.Append("FROM " + Source2TableName + " Source2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source1TableName + " Source1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("GROUP BY ORG_PART_KEY ORDER BY ORG_PART_KEY ;\n\n");

            sb.Append("--This is the new way.  Use these queries to find Missing and Additional Records\n");
            Source1Counts = Source1TableName + "_COUNTS";
            Source2Counts = Source2TableName + "_COUNTS";

            sb.Append("DROP TABLE " + Source1Counts + "; \n");
            sb.Append("DROP TABLE " + Source2Counts + "; \n");





            //Missing Detail, filtering out duplicates and rollups
            sb.Append("--Query for missing details.  PKs in Source2, not in Source1.\n");
            sb.Append("SELECT "+sbPKGroupBy+",*\n");
            sb.Append("FROM " + Source2TableName + " Source2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source1TableName + " Source1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("--Filter out any exact duplicates that exists on Source1\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS Source1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("--Filter out any exact duplicates that exists on Source2\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS Source1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("ORDER BY " + sbPKGroupBy + " LIMIT 1000;\n\n");

            //Additional Detail, filtering out duplicates and rollups
            sb.Append("--Query for additional details\n");
            sb.Append("--Additional Records in Source1, not in Source2\n");
            sb.Append("SELECT " + sbPKGroupBy + ",*\n");
            sb.Append("FROM " + Source1TableName + " Source1\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + Source2TableName + " Source2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");

            sb.Append("--Filter out any exact duplicates that exists on Source1\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source1" + "_DUPS Source2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("--Filter out any exact duplicates that exists on Source2\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + TempTablePrefix + tableObjects.TableName + "_Source2" + "_DUPS Source2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("ORDER BY " + sbPKGroupBy + " LIMIT 1000;\n\n");

            sb.Append("--Query for column difference details\n");
            sb.Append(sbColumnDetailQuery.ToString());
            sb.Append("*/\n");

            //Write out string
            TextWriter tw = new StreamWriter(OutputDirectory + @"\" + tableObjects.TableName + "_" + DateTime.Now.Year + DateTime.Now.Month +
                DateTime.Now.Day+DateTime.Now.Hour+DateTime.Now.Minute+DateTime.Now.Second+".sql");
            tw.NewLine = "\n";
            tw.WriteLine(sb.ToString());
            tw.Flush();
            tw.Close();

        }

        private void chkTemp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTemp.Checked == true)
            {
                label6.Visible = true;
                txtTempSeed.Visible = true;
            }
            else
            {
                label6.Visible = false;
                txtTempSeed.Visible = false;
            }

        }

    }
}
