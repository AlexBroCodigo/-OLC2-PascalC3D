using PascalC3D.Compilacion.TablaSimbolos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Utils
{
    class Tipo
    {
        public enum Tipos
        {
            INTEGER = 0,
            REAL = 1,
            STRING = 2,
            BOOLEAN = 3,
            ERROR = 4,
            STRUCT = 5,
            VOID = 6
        }
        public Tipos tipo;
        public string tipoId;
        public SimboloStruct symStruct;


        public Tipo(Tipos tipo, string tipoId = "",SimboloStruct symStruct = null)
        {
            this.tipo = tipo;
            this.tipoId = tipoId;
            this.symStruct = symStruct;
        }

        public String tipoToString()
        {
            switch (tipo)
            {
                case Tipos.INTEGER: return "INTEGER";
                case Tipos.REAL: return "REAL";
                case Tipos.STRING: return "STRING";
                case Tipos.BOOLEAN: return "BOOLEAN";
                case Tipos.STRUCT: return "STRUCT";
                case Tipos.VOID: return "VOID";
                default: return "ERROR";
            }
        }
    }
}
