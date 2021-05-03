using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.TablaSimbolos
{
    class SimboloArray
    {
        public string id;
        public LinkedList<Dimension> dimensiones;
        public Tipo tipoArreglo;

        public SimboloArray(string id, LinkedList<Dimension> dimensiones, Tipo tipoArreglo)
        {
            this.id = id;
            this.dimensiones = dimensiones;
            this.tipoArreglo = tipoArreglo;
        }
    }
}
