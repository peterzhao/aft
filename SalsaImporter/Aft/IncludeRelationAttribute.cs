using System;
using System.Collections.Generic;
using System.Linq;

namespace SalsaImporter.Aft
{
    public class IncludeRelationAttribute : Attribute
    {
        private readonly string[] _relations;

        public IncludeRelationAttribute(params string[] relations)
        {
            _relations = relations;
        }

        public List<string> Relations
        {
            get { return _relations.ToList(); }
        }
    }
}