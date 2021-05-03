using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    class Funcion
    {
        public string nombre;
        public LinkedList<Etiqueta> instrucciones;

        public Funcion(string nombre,LinkedList<Etiqueta> instrucciones)
        {
            this.nombre = nombre;
            this.instrucciones = instrucciones;
        }
    }
}
