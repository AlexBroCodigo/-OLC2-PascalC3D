using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Interfaces
{
    interface Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        Retorno compilar(Entorno ent);
        
    }
}
