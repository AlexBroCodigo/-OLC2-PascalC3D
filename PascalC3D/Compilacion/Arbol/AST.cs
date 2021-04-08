using PascalC3D.Compilacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Arbol
{
    class AST
    {
        public LinkedList<Instruccion> instrucciones;

        public AST(LinkedList<Instruccion> instruccions)
        {
            instrucciones = instruccions;
        }
    }
}
