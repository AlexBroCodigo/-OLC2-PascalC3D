using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    class GOTO : Instruccion
    {
        public string id;
        public int linea;
        public int columna;
        public AST ast;

        public GOTO(string id, int linea, int columna)
        {
            this.id = id;
            this.linea = linea;
            this.columna = columna;
            ast = null;
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
            string codigoAugus = "goto " + this.id + ";\n";
            return codigoAugus;
        }

        
    }
}
