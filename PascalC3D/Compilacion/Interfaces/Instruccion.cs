using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Interfaces
{
    interface Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public object compilar(Entorno ent,Errores errores);

    }
}
