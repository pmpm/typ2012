using System;
using System.Linq;
using FluentNHibernate.Automapping;

namespace CQRS2012.Gui.Models.Database
{
    public class DbMapConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DbMapAttribute), false);

            return attributes.Any(attr => attr.GetType() == typeof(DbMapAttribute));
        }
    }
}