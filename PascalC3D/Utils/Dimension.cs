using PascalC3D.Compilacion.Expresiones.Literal;
using PascalC3D.Compilacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Utils
{
    class Dimension
    {
        public Primitivo inferior;
        public Primitivo superior;
        public int numero; //La dimension que es (primera, segunda)

        public Dimension(Primitivo inferior, Primitivo superior)
        {
            this.inferior = inferior;
            this.superior = superior;
        }

    }
}
