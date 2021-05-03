using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    interface Expresion
    {
        public OptimizacionResultado optimizarCodigo();
        public String generarAugus();


    }
}
