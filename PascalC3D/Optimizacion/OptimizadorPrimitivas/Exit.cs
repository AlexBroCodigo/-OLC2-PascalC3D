using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorPrimitivas
{
    class Exit : Instruccion
    {
        public Exit()
        {

        }

        public OptimizacionResultado optimizarCodigo(ReporteOptimizacion reporte)
        {
            string antes = this.generarAugus(reporte);
            OptimizacionResultado resultado = new OptimizacionResultado();
            resultado.codigo = antes;
            return resultado;
        }

        public string generarAugus(ReporteOptimizacion reporte)
        {
            string codigoAugus = "return;\n";
            return codigoAugus;
        }

    }
}
