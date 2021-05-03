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
            VOID = 6,
            ARRAY = 7
        }
        public Tipos tipo;
        public string tipoId;
        public SimboloStruct symStruct;
        public int dimension;


        public Tipo(Tipos tipo, string tipoId = "",SimboloStruct symStruct = null,int dimension = 0)
        {
            this.tipo = tipo;
            this.tipoId = tipoId;
            this.symStruct = symStruct;
            this.dimension = dimension;
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
                case Tipos.ARRAY: return "ARRAY";
                default: return "ERROR";
            }
        }
    }
}
