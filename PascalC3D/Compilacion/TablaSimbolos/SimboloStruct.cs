using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class SimboloStruct
    {
        public string identifier;
        public int size;
        public LinkedList<Param> attributes;

        public SimboloStruct(string identifier, int size, LinkedList<Param> attributes)
        {
            this.identifier = identifier;
            this.size = size;
            this.attributes = attributes;
        }

        public Jackson getAttribute(string id)
        {
            for(int i = 0; i < this.attributes.Count; i++)
            {
                Param value = this.attributes.ElementAt(i);
                if (value.id.Equals(id))
                {
                    return new Jackson(i, value);
                }
            }
            return new Jackson(-1, null);
        }


    }
}
