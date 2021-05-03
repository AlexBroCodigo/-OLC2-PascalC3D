using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    class Simbolo : Expresion
    {
        private string id;
        private int linea;
        private int columna;

        public Simbolo(string id, int linea, int columna)
        {
            this.id = id;
            this.linea = linea;
            this.columna = columna;
        }

        public OptimizacionResultado optimizarCodigo()
        {
            string antes = this.generarAugus();
            OptimizacionResultado resultado = new OptimizacionResultado();
            resultado.codigo = antes;
            return resultado;
        }

        public string generarAugus()
        {
            return this.id;
        }
    }
}
