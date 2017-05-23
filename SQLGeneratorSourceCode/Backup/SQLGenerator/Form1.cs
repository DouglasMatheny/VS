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
            SelectString = SelectString.ToUpper().Replace(@"SELECT", "").Replace(@";", "").Replace(@" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("ADMIN", "");

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
            string StartDate = ConfigurationManager.AppSettings["StartDate"];
            string EndDate = ConfigurationManager.AppSettings["EndDate"];

            string tempdb = ConfigurationManager.AppSettings["TempDB"];
            string T1db = ConfigurationManager.AppSettings["T1_DB"];
            string T2db = ConfigurationManager.AppSettings["T2_DB"];
            string OutDir = ConfigurationManager.AppSettings["OutputDirectory"]; 
            string TablePrefix = ConfigurationManager.AppSettings["TablePrefix"]; 
            string T1TableName = "";
            string T2TableName = "";
            string seed  = "";
            string T1Rollups = "";
            string T2Rollups = "";
            string T1PRollups = "";
            string T2PRollups = "";
            string T1Counts = "";
            string T2Counts = "";

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
                    sbPKJoins.Append("WHERE T1." + pk[j].ToString() + " = T2." + pk[j].ToString()+"\n");
                    sbPKGroupBy.Append(" "+pk[j].ToString());
                }
                else
                {
                    sbPKJoins.Append("AND T1." + pk[j].ToString() + " = T2." + pk[j].ToString()+"\n");
                    sbPKGroupBy.Append(", "+pk[j].ToString());
                }
            }


            foreach(Field f in tableObjects.Fields)
            {
                if (f.Exclude == false)
                {
                    if (f.Name != StartDate && f.Name != EndDate)
                    {
                        if (k == 0)
                        {
                            sbDupJoins.Append("WHERE TRIM(NVL(CAST(T1." + f.Name + " AS VARCHAR(50)),'')) = " + "TRIM(NVL(CAST(T2." + f.Name + " AS VARCHAR(50)),''))\n");
                            sbDupGroupBy.Append(f.Name);
                        }
                        else
                        {
                            sbDupJoins.Append("AND TRIM(NVL(CAST(T1." + f.Name + " AS VARCHAR(50)),'')) = " + "TRIM(NVL(CAST(T2." + f.Name + " AS VARCHAR(50)),''))\n");
                            sbDupGroupBy.Append(", "+f.Name);
                        }
                        k++;
                    }
                }
            }
            k = 0;

            if (this.chkTemp.Checked)
            {
                //CREATE TEMP TABLES AND CHECK SAMPLE COUNTS
                T1TableName = tempdb+".."+TablePrefix+tableObjects.TableName+"_T1";
                T2TableName = tempdb+".."+TablePrefix + tableObjects.TableName + "_T2";
                sb.Append("--Populate temp table using sample data from T1\n");
                sb.Append("SELECT *\n");
                sb.Append("INTO " + T1TableName + "\n");
                sb.Append("FROM " + T1db + "..T1_" + tableObjects.TableName + "\n");
                sb.Append("WHERE " + seed + " LIKE '%" + txtTempSeed.Text+"';\n");
                sb.Append("\n");
                sb.Append("--Populate temp table using sample data from T2\n");
                sb.Append("SELECT *\n");
                sb.Append("INTO " + T2TableName + "\n");
                sb.Append("FROM " + T2db + "..T2_" + tableObjects.TableName + "\n");
                sb.Append("WHERE " + seed + " LIKE '%" + txtTempSeed.Text + "';\n");
                sb.Append("\n");
            }
            else
            {
                T1TableName = T1db + "..T1_" + tableObjects.TableName;
                T2TableName = T2db + "..T2_" + tableObjects.TableName;
            }

            //CREATE TABLES WITH T-1 and T-2 duplicates, so we can filter these out.
            sb.Append("\n");
            sb.Append("--Place all T-1 duplicates in a seperate table.\n");
            sb.Append("SELECT "+ sbPKGroupBy + ", COUNT(*)\n");
            sb.Append("INTO " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS\n");
            sb.Append("FROM " + T1TableName + "\n");
            sb.Append("GROUP BY "+ sbPKGroupBy+ " HAVING COUNT(*) > 1;\n");
            sb.Append("\n");
            sb.Append("--Place all T-2 duplicates in a seperate table.\n");
            sb.Append("SELECT " + sbPKGroupBy + ", COUNT(*)\n");
            sb.Append("INTO " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS\n");
            sb.Append("FROM " + T2TableName + "\n");
            sb.Append("GROUP BY " + sbPKGroupBy + " HAVING COUNT(*) > 1;\n\n");

            //CHECK TO SEE WHAT WAS ROLLED UP
            if (this.chkDateLogic.Checked)
            {
                T1PRollups = tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_PossibleRollups";
                T2PRollups = tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_PossibleRollups";
                T1Rollups = tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_Rollups";
                T2Rollups = tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_Rollups";
                sb.Append("\n");
                sb.Append("--Create a temp table with all possible rollups using T-2 data\n");
                sb.Append("SELECT " + sbDupGroupBy + ", COUNT(*) AS COUNTS\n");
                sb.Append("INTO " + T2PRollups + "\n");
                sb.Append("FROM " + T2TableName + "\n");
                sb.Append("GROUP BY " + sbDupGroupBy + " HAVING COUNT(*) > 1;\n");
                sb.Append("\n");
                sb.Append("--Create a temp table with all possible rollups using T-1 data\n");
                sb.Append("SELECT " + sbDupGroupBy + ", COUNT(*) AS COUNTS\n");
                sb.Append("INTO " + T1PRollups + "\n");
                sb.Append("FROM " + T1TableName + " T1\n");
                sb.Append("WHERE EXISTS\n");
                sb.Append("(\n");
                sb.Append("SELECT 1 FROM " + T2PRollups + " T2\n");
                sb.Append(sbDupJoins.ToString());
                sb.Append(")\n");
                sb.Append("GROUP BY " + sbDupGroupBy + ";\n");
                sb.Append("\n");
                sb.Append("--Create a temp table with all T-1 records rolled up\n");
                sb.Append("SELECT DISTINCT *\n");
                sb.Append("INTO " + T1Rollups + "\n");
                sb.Append("FROM " + T1PRollups + "\n");
                sb.Append("WHERE COUNTS = 1;\n");
                sb.Append("\n");
                sb.Append("--Create a temp table with all T-2 records rolled up\n");
                sb.Append("SELECT DISTINCT *\n");
                sb.Append("INTO " + T2Rollups + "\n");
                sb.Append("FROM " + T2PRollups + " T2\n");
                sb.Append("WHERE EXISTS\n");
                sb.Append("(\n");
                sb.Append("SELECT 1 FROM " + T1Rollups + " T1\n");
                sb.Append(sbDupJoins.ToString());
                sb.Append(");\n");
            }

            //TABLE COUNTS
            sb.Append("--Put the table name in for the spreadsheet.\n");
            sb.Append("SELECT '(00)TABLENAME' AS KEY,'" + tableObjects.TableName + "' AS VALUE\n");
            sb.Append("\nUNION\n");
            sb.Append("--Record Counts for T1 table:" + tableObjects.TableName + "\n");
            sb.Append("SELECT '(01) Total T1 Count' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + T1db + "..T1_" + tableObjects.TableName + "\n");
            sb.Append("\nUNION\n");
            sb.Append("--Record Counts for T2 table:" + tableObjects.TableName + "\n");
            sb.Append("SELECT '(02) Total T2 Count' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + T2db + "..T2_" + tableObjects.TableName + "\n");
            sb.Append("\nUNION\n");

            if (this.chkTemp.Checked)
            {
                sb.Append("--Record Counts for T1 temp table table:" + T1TableName + "\n");
                sb.Append("SELECT '(03) T1 Sample Count' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                sb.Append("FROM " + T1TableName + "\n");
                sb.Append("\nUNION\n");
                sb.Append("--Record Counts for T2 temp table table:" + T2TableName + "\n");
                sb.Append("SELECT '(04) T2 Sample Count' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                sb.Append("FROM " + T2TableName + "\n");
                sb.Append("UNION\n");

            }
            else
            {
                sb.Append("--Placeholder for temp table record counts(its there for the spreadsheet).\n");
                sb.Append("SELECT '(03) T1 Sample Count' AS KEY, CAST(0 AS VARCHAR(50)) AS VALUE\n");
                sb.Append("\nUNION\n");
                sb.Append("--Placeholder for temp table record counts(its there for the spreadsheet).\n");
                sb.Append("SELECT '(04) T2 Sample Count' AS KEY, CAST(0 AS VARCHAR(50)) AS VALUE\n");
                sb.Append("UNION\n");
            }
            //T-1 Duplicates
            sb.Append("--T-1 Duplicate Records\n");
            sb.Append("SELECT '(05) T-1 Duplicates' AS KEY, CAST(SUM(COUNT) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS\n");
            sb.Append("\nUNION\n");

            //T-2 Duplicates
            sb.Append("--T-2 Duplicate Records\n");
            sb.Append("SELECT '(06) T-2 Duplicates' AS KEY, CAST(SUM(COUNT) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM "  + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS\n");
            sb.Append("\nUNION\n\n");
            //MATCHES
            sb.Append("--Matching Records\n");
            sb.Append("SELECT '(07) Primary Key Matches' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + T1TableName + " T1\n");
            sb.Append("WHERE EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T2TableName + " T2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("\nUNION\n");



            //MISSING
            sb.Append("--Missing Records in T-2, not in T-1\n");
            sb.Append("SELECT '(08) Missing Records' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + T2TableName + " T2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T1TableName + " T1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("\nUNION\n");

            if (chkDateLogic.Checked == true)
            {
                //ROLLED UP RECORDS
                sb.Append("--Duplicate Records in T-2, rolled into 1 on T-1\n");
                sb.Append("SELECT '(09) Rolled Up Records' AS KEY, CAST(SUM(COUNTS) AS VARCHAR(50)) AS VALUE\n");
                sb.Append("FROM " + T2Rollups + "\n");
            }
            else
            {
                sb.Append("SELECT '(09) Rolled Up Records' AS KEY, 'DATE LOGIC NOT APPLIED' AS VALUE\n");
            }
            sb.Append("\nUNION\n");
            //ADDITIONAL
            sb.Append("--Additional Records in T-1, not in T-2\n");
            sb.Append("SELECT '(10) Additional Records' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
            sb.Append("FROM " + T1TableName + " T1\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T2TableName + " T2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            //DISPLAY PK COLUMNS
            foreach (Field f in tableObjects.Fields)
            {
                if (f.PrimaryKey == true)
                {
                    sb.Append("\nUNION\n");
                    sb.Append("--Column is part of PK:" + f.Name + "\n");
                    sb.Append("SELECT '" + f.Name + "' AS KEY, 'PK' AS VALUE\n");
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
                        sb.Append("SELECT '" + f.Name + "' AS KEY, CAST(COUNT(*) AS VARCHAR(50)) AS VALUE\n");
                        sb.Append("FROM " + T1TableName + " T1\n");
                        sb.Append("WHERE EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + T2TableName + " T2\n");
                        sb.Append(sbPKJoins.ToString());
                        if (chkDateLogic.Checked == true)
                        {
                            sb.Append("AND T2." + StartDate + " < T2." + EndDate + " \n");
                        }
                        sb.Append("--This is the column we are checking\n");
                        sb.Append("AND TRIM(NVL(CAST(T1." + f.Name + " AS VARCHAR(50)),'')) != " + "TRIM(NVL(CAST(T2." + f.Name + " AS VARCHAR(50)),''))\n");
                        sb.Append(")\n");
                        sb.Append("--Filter out any exact duplicates that exists on T1\n");
                        sb.Append("AND NOT EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS T2\n");
                        sb.Append(sbPKJoins.ToString());
                        sb.Append(")\n");
                        sb.Append("--Filter out any exact duplicates that exists on T2\n");
                        sb.Append("AND NOT EXISTS\n");
                        sb.Append("(\n");
                        sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS T2\n");
                        sb.Append(sbPKJoins.ToString());
                        sb.Append(")\n");
                        if (this.chkDateLogic.Checked)
                        {
                            sb.Append("--Filter out any duplicate records rolled up into 1.  Excluding version dates, T1 and T-2 are identical for these, so no need to check them.\n");
                            sb.Append("AND NOT EXISTS\n");
                            sb.Append("(\n");
                            sb.Append("SELECT 1 FROM " + T1Rollups + " T2\n");
                            sb.Append(sbDupJoins.ToString());
                            sb.Append(")\n");
                        }

                        if (k == 0)
                        {
                            StringBuilder sbPKGroupDetBy = new StringBuilder();
                            StringBuilder sbPKJoins1 = new StringBuilder();
                            sbPKJoins1.Append(sbPKJoins);
                            sbPKJoins1.Replace("WHERE", "");
                            sbPKGroupDetBy.Append(sbPKGroupBy.ToString());
                            sbPKGroupDetBy.Replace(" "," T1.");
                            sbColumnDetailQuery.Append("--Sample Query column difference details:" + f.Name + "\n");
                            sbColumnDetailQuery.Append("SELECT "+sbPKGroupDetBy+ ", T1." + f.Name + " AS T1_"+ f.Name + ", T2." + f.Name + " AS T2_"+ f.Name+"\n");
                            sbColumnDetailQuery.Append("FROM " + T1TableName + " T1\n");
                            sbColumnDetailQuery.Append("INNER JOIN " + T2TableName + " T2\n");
                            sbColumnDetailQuery.Append("ON(\n");
                            sbColumnDetailQuery.Append(sbPKJoins1.ToString());
                            sbColumnDetailQuery.Append(")\n");
                            sbColumnDetailQuery.Append("--This is the column we are checking\n");
                            sbColumnDetailQuery.Append("WHERE TRIM(NVL(CAST(T1." + f.Name + " AS VARCHAR(50)),'')) != " + "TRIM(NVL(CAST(T2." + f.Name + " AS VARCHAR(50)),''))\n");

                            if (chkDateLogic.Checked == true)
                            {
                                sbColumnDetailQuery.Append("AND T2." + StartDate + " < T2." + EndDate + " \n");
                            }
                            sbColumnDetailQuery.Append("--Filter out any exact duplicates that exists on T1\n");
                            sbColumnDetailQuery.Append("AND NOT EXISTS\n");
                            sbColumnDetailQuery.Append("(\n");
                            sbColumnDetailQuery.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS T2\n");
                            sbColumnDetailQuery.Append(sbPKJoins.ToString());
                            sbColumnDetailQuery.Append(")\n");
                            sbColumnDetailQuery.Append("--Filter out any exact duplicates that exists on T2\n");
                            sbColumnDetailQuery.Append("AND NOT EXISTS\n");
                            sbColumnDetailQuery.Append("(\n");
                            sbColumnDetailQuery.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS T2\n");
                            sbColumnDetailQuery.Append(sbPKJoins.ToString());
                            sbColumnDetailQuery.Append(")\n");

                            if (this.chkDateLogic.Checked)
                            {
                                sbColumnDetailQuery.Append("--Filter out any duplicate records rolled up into 1.  Excluding version dates, T1 and T-2 are identical for these, so no need to check them.\n");
                                sbColumnDetailQuery.Append("AND NOT EXISTS\n");
                                sbColumnDetailQuery.Append("(\n");
                                sbColumnDetailQuery.Append("SELECT 1 FROM " + T1Rollups + " T2\n");
                                sbColumnDetailQuery.Append(sbDupJoins.ToString());
                                sbColumnDetailQuery.Append(")\n");
                            }
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
                sb.Append("DROP TABLE "+T1TableName+";\n");
                sb.Append("\n");
                sb.Append("DROP TABLE " + T2TableName + ";\n");
                sb.Append("\n");
            }
            if (this.chkDateLogic.Checked)
            {
                sb.Append("DROP TABLE " + T1PRollups + ";\n");
                sb.Append("\n");
                sb.Append("DROP TABLE " + T2PRollups + ";\n");
                sb.Append("\n");
                sb.Append("DROP TABLE " + T1Rollups + ";\n");
                sb.Append("\n");
                sb.Append("DROP TABLE " + T2Rollups + ";\n");
                sb.Append("\n");
            }
            sb.Append("DROP TABLE " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS;\n");
            sb.Append("\n");
            sb.Append("DROP TABLE " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS;\n");
            sb.Append("\n");
            //Detailed Queries

            //MISSING ORG_PART_KEYS

            sb.Append("/*\n");
            sb.Append("--This is the old way I was doing it which is not entirely accurate\n");
            sb.Append("--Missing Records Counts BY ORG_PART_KEY\n");
            sb.Append("SELECT ORG_PART_KEY, COUNT(*)\n");
            sb.Append("FROM " + T2TableName + " T2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T1TableName + " T1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("GROUP BY ORG_PART_KEY ORDER BY ORG_PART_KEY ;\n\n");

            sb.Append("--This is the new way.  Use these queries to find Missing and Additional Records\n");
            T1Counts = T1TableName + "_COUNTS";
            T2Counts = T2TableName + "_COUNTS";

            sb.Append("DROP TABLE " + T1Counts + "; \n");
            sb.Append("DROP TABLE " + T2Counts + "; \n");

            sb.Append("--Get Counts by XXX_KEY \n");
            sb.Append("SELECT ORG_PART_KEY, XXX_KEY, COUNT(*) AS COUNTS \n");
            sb.Append("INTO " + T1Counts+" \n");
            sb.Append("FROM " + T1TableName + " \n");
            sb.Append("GROUP BY ORG_PART_KEY, XXX_KEY; \n");
            sb.Append(" \n");
            sb.Append("SELECT ORG_PART_KEY, XXX_KEY, COUNT(*) AS COUNTS \n");
            sb.Append("INTO " + T2Counts + " \n");
            sb.Append("FROM " + T2TableName + " \n");
            sb.Append("GROUP BY ORG_PART_KEY, XXX_KEY; \n");
            sb.Append(" \n");
            sb.Append("--Find records which are missing a XXX_KEY altogether \n");
            sb.Append("SELECT ORG_PART_KEY, SUM(COUNTS) \n");
            sb.Append("FROM " +T2Counts+" T2 \n");
            sb.Append("WHERE NOT EXISTS \n");
            sb.Append("( \n");
            sb.Append("SELECT 1 FROM " + T1Counts + " T1 \n");
            sb.Append("WHERE T1.XXX_KEY = T2.XXX_KEY \n");
            sb.Append(") \n");
            sb.Append("GROUP BY ORG_PART_KEY \n");
            sb.Append("ORDER BY ORG_PART_KEY; \n");
            sb.Append(" \n");
            sb.Append("--Find records which have XXX_KEY in both places but more in T2 than in T1\n");
            sb.Append("SELECT T1.ORG_PART_KEY, SUM(T2.COUNTS-T1.COUNTS) \n");
            sb.Append("FROM " + T2Counts  + " T2 \n");
            sb.Append("INNER JOIN " + T1Counts + " T1 \n");
            sb.Append("ON(T1.XXX_KEY = T2.XXX_KEY) \n");
            sb.Append("WHERE T2.COUNTS > T1.COUNTS \n");
            sb.Append("GROUP BY T1.ORG_PART_KEY \n");
            sb.Append("ORDER BY T1.ORG_PART_KEY; \n");
            sb.Append(" \n");

            sb.Append("--Find extra records by XXX_KEY \n");
            sb.Append("SELECT ORG_PART_KEY, SUM(COUNTS) \n");
            sb.Append("FROM " + T1Counts + " T2 \n");
            sb.Append("WHERE NOT EXISTS \n");
            sb.Append("( \n");
            sb.Append("SELECT 1 FROM " + T2Counts + " T1 \n");
            sb.Append("WHERE T1.XXX_KEY = T2.XXX_KEY \n");
            sb.Append(") \n");
            sb.Append("GROUP BY ORG_PART_KEY \n");
            sb.Append("ORDER BY ORG_PART_KEY; \n");
            sb.Append(" \n");
            sb.Append("--Find records which have XXX_KEY in both places but more in T1 than in T2\n");
            sb.Append("SELECT T1.ORG_PART_KEY, SUM(T2.COUNTS-T1.COUNTS) \n");
            sb.Append("FROM " + T1Counts + " T2 \n");
            sb.Append("INNER JOIN " + T2Counts + " T1 \n");
            sb.Append("ON(T1.XXX_KEY = T2.XXX_KEY) \n");
            sb.Append("WHERE T2.COUNTS > T1.COUNTS \n");
            sb.Append("GROUP BY T1.ORG_PART_KEY \n");
            sb.Append("ORDER BY T1.ORG_PART_KEY; \n");
            sb.Append(" \n");

            if (chkDateLogic.Checked == true)
            {
                sb.Append("--Records removed were start and end dates are equal\n");
                sb.Append("SELECT COUNT(*)\n");
                sb.Append("FROM " + T2TableName + " T2\n");
                sb.Append("WHERE " + StartDate + " = " + EndDate + "\n");
                sb.Append("AND NOT EXISTS\n");
                sb.Append("(\n");
                sb.Append("SELECT 1 FROM " + T1TableName + " T1\n");
                sb.Append(sbPKJoins.ToString());
                sb.Append(")\n");
                sb.Append(";\n\n");
            }


            //Missing Detail, filtering out duplicates and rollups
            sb.Append("--Query for missing details.  PKs in T-2, not in T-1.\n");
            sb.Append("SELECT "+sbPKGroupBy+",*\n");
            sb.Append("FROM " + T2TableName + " T2\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T1TableName + " T1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            if (chkDateLogic.Checked == true)
            {
                sb.Append("AND " + StartDate + " != " + EndDate + "\n");
            }
            sb.Append("--Filter out any exact duplicates that exists on T1\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS T1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("--Filter out any exact duplicates that exists on T2\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS T1\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            if (this.chkDateLogic.Checked)
            {
                sb.Append("--Filter out any duplicate records rolled up into 1.  Excluding version dates, T1 and T-2 are identical for these, so no need to check them.\n");
                sb.Append("AND NOT EXISTS\n");
                sb.Append("(\n");
                sb.Append("SELECT 1 FROM " + T1Rollups + " T1 \n");
                sb.Append(sbDupJoins.ToString());
                sb.Append(")\n");
            }
            sb.Append("ORDER BY " + sbPKGroupBy + " LIMIT 1000;\n\n");

            //Additional Detail, filtering out duplicates and rollups
            sb.Append("--Query for additional details\n");
            sb.Append("--Additional Records in T-1, not in T-2\n");
            sb.Append("SELECT " + sbPKGroupBy + ",*\n");
            sb.Append("FROM " + T1TableName + " T1\n");
            sb.Append("WHERE NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + T2TableName + " T2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");

            sb.Append("--Filter out any exact duplicates that exists on T1\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T1" + "_DUPS T2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            sb.Append("--Filter out any exact duplicates that exists on T2\n");
            sb.Append("AND NOT EXISTS\n");
            sb.Append("(\n");
            sb.Append("SELECT 1 FROM " + tempdb + ".." + TablePrefix + tableObjects.TableName + "_T2" + "_DUPS T2\n");
            sb.Append(sbPKJoins.ToString());
            sb.Append(")\n");
            if (this.chkDateLogic.Checked)
            {
                sb.Append("--Filter out any duplicate records rolled up into 1.  Excluding version dates, T1 and T-2 are identical for these, so no need to check them.\n");
                sb.Append("AND NOT EXISTS\n");
                sb.Append("(\n");
                sb.Append("SELECT 1 FROM " + T1Rollups + " T2\n");
                sb.Append(sbDupJoins.ToString());
                sb.Append(")\n");
            }

            sb.Append("ORDER BY " + sbPKGroupBy + " LIMIT 1000;\n\n");

            sb.Append("--Query for column difference details\n");
            sb.Append(sbColumnDetailQuery.ToString());
            sb.Append("*/\n");

            //Write out string
            TextWriter tw = new StreamWriter(OutDir + @"\"+tableObjects.TableName+"_"+DateTime.Now.Year+DateTime.Now.Month+
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


        private void chkDateLogic_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDateLogic.Checked)
            {
                this.label8.Visible = true;
                this.label9.Visible = true;
                this.label10.Visible = true;
                this.label11.Visible = true;
                this.label10.Text = ConfigurationManager.AppSettings["StartDate"];
                this.label11.Text = ConfigurationManager.AppSettings["EndDate"];
            }
            else
            {
                this.label8.Visible = false;
                this.label9.Visible = false;
                this.label10.Visible = false;
                this.label11.Visible = false;
            }
        }

    }
}
