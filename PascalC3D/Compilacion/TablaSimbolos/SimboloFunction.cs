using PascalC3D.Compilacion.Instrucciones.Functions;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class SimboloFunction
    {
        public Tipo type;
        public string id;
        public string uniqueId;
        public int size;
        public LinkedList<Param> parametros;
        public int linea;
        public int columna;

        public SimboloFunction(FunctionSt func,string uniqueId)
        {
            this.type = func.type;
            this.id = func.id;
            this.size = func.parametros.Count;
            this.uniqueId = uniqueId;
            this.parametros = func.parametros;
            this.linea = func.linea;
            this.columna = func.columna;
        }
    }
}
