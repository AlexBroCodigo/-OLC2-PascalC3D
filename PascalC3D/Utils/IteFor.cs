using PascalC3D.Compilacion.Instrucciones.Variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Utils
{
    class IteFor
    {
        public bool isFor;
        public Asignacion actualizarVariable;

        public IteFor(bool isFor, Asignacion actualizarVariable)
        {
            this.isFor = isFor;
            this.actualizarVariable = actualizarVariable;
        }
    }
}
