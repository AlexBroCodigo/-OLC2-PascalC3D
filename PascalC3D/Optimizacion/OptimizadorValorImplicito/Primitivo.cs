using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorValorImplicito
{
    class Primitivo : Expresion
    {
        private object valor;

        public Primitivo(object valor)
        {
            this.valor = valor;
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
            return ""+valor;
        }

    }
}
