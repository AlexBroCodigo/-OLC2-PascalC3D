using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    interface Instruccion
    {
        public OptimizacionResultado optimizarCodigo(ReporteOptimizacion reporte);
        public String generarAugus(ReporteOptimizacion reporte);

    }
}
