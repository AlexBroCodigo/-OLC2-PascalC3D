using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Control
{
    class Bloque : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public LinkedList<Instruccion> instrucciones;

        public Bloque(LinkedList<Instruccion> instrucciones)
        {
            this.instrucciones = instrucciones;
            linea = 0;
            columna = 0;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            return null;
        }
    }
}
