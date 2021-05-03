using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.OptimizadorValorImplicito;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorPrimitivas
{
    class Imprimir : Instruccion
    {
        public Operacion cad;
        public string cadena;
        public int linea;
        public int columna;

        public Imprimir(Operacion cad,string cadena, int linea, int columna)
        {
            this.cad = cad;
            this.cadena = cadena;
            this.linea = linea;
            this.columna = columna;
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
            string codigoAugus = "printf(" +this.cadena + "," + this.cad.generarAugus() + ");\n";
            return codigoAugus;
        }

        
    }
}
