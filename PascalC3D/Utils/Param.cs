using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Utils
{
    class Param
    {
        public string id;
        public Tipo type;

        public Param(string id, Tipo type)
        {
            this.id = id.ToLower();
            this.type = type;
        }

        public string getUnicType()
        {
            if(this.type.tipo == Tipos.STRUCT)
            {
                return this.type.tipoId;
            }
            return this.type.tipo + "";
        }

        public string toString()
        {
            return "{id: " + id + ", type: " + type + "}";
        }


    }
}
