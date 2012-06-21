using System;

namespace CQRS2012.Gui.Models.Database
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbMapAttribute : Attribute
    {
        public DbMapAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}