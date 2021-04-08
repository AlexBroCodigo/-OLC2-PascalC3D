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
            OBJECT = 5,
            VOID = 6
        }
        public Tipos tipo;
        public string tipoId;
        public string tipoObjeto;

        //tipoAuxiliar: Me servira para los objetos
        public Tipo(Tipos tipo, string tipoId = "", string tipoObjeto = "")
        {
            this.tipo = tipo;
            this.tipoId = tipoId;
            this.tipoObjeto = tipoObjeto;
        }

        public String tipoToString()
        {
            switch (tipo)
            {
                case Tipos.INTEGER: return "INTEGER";
                case Tipos.REAL: return "REAL";
                case Tipos.STRING: return "STRING";
                case Tipos.BOOLEAN: return "BOOLEAN";
                case Tipos.ERROR: return "ERROR";
                case Tipos.OBJECT: return "OBJECT";
                case Tipos.VOID: return "VOID";
                default: return "ERROR";
            }
        }
    }
}
