using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Utils
{
    class Error : Exception
    {
        public String tipo;
        public String descripcion;
        public String ambito;
        public int linea;
        public int columna;

        public Error(String tipo, String descripcion, String ambito,int linea,int columna)
        {
            this.tipo = tipo;
            this.descripcion = descripcion;
            this.ambito = ambito;
            this.linea = linea;
            this.columna = columna;
        }
    }
}
