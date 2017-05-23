using System;
using System.Collections.Generic;
using System.Text;

namespace SQLGenerator.BOs
{
    public class TableObjects
    {
        public TableObjects() { }
        public string TableName { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public Boolean PrimaryKey { get; set; }
        public Boolean Seed { get; set; }
        public Boolean Exclude { get; set; } 
    }
}
