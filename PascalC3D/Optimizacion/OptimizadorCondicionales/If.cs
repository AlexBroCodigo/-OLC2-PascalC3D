using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.OptimizadorValorImplicito;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorCondicionales
{
    class If : Instruccion
    {
        private Operacion condicion;
        private string etiqueta;
        private int linea;
        private int columna;
        public LinkedList<Instruccion> instrucciones;
        public AST ast;

        public If(Operacion condicion, string etiqueta, int linea, int columna)
        {
            this.condicion = condicion;
            this.etiqueta = etiqueta;
            this.linea = linea;
            this.columna = columna;
            instrucciones = new LinkedList<Instruccion>();
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
            string codigoAugus = "if(" + this.condicion.generarAugus() + ") goto " + this.etiqueta + ";\n";
            OPtimizacion optimizacion = new OPtimizacion();
            optimizacion.linea = ""+(this.linea+1);
            optimizacion.antes = codigoAugus;
            optimizacion.tipo = "Mirilla - Eliminación de Codigo Inalcanzable";

            if(this.condicion.tipo == Operacion.TIPO_OPERACION.IGUAL_IGUAL)
            {
                if (this.condicion.validarRegla4())
                {
                    optimizacion.regla = "Regla 4";
                    optimizacion.despues = "goto " + this.etiqueta + ";";
                    reporte.agregarOpt(optimizacion);
                    codigoAugus = "goto " + this.etiqueta + ";\n";
                }
                else if (this.condicion.validarRegla5())
                {
                    optimizacion.regla = "Regla 5";
                    optimizacion.despues = "";
                    reporte.agregarOpt(optimizacion);
                }
            }

            try
            {
                if (codigoAugus.StartsWith("if"))
                {
                    if (this.instrucciones.Count > 0)
                    {
                        if(this.instrucciones.ElementAt(0) is GOTO) //validamos que la siguiente instruccion sea un goto
                        {
                            string condicionNueva = this.condicion.invertirCondicion();

                            if(!condicionNueva.Equals(this.condicion.generarAugus())) //si la condicion si cambio se hace la optimizacion
                            {
                                GOTO etiquetaFalse = (GOTO)this.instrucciones.ElementAt(0);
                                Etiqueta etiquetaTrue = this.ast.obtenerEtiqueta(this.etiqueta);

                                string codigoOptimizar = codigoAugus;
                                codigoOptimizar += "goto " + etiquetaFalse.id + ";\n";
                                codigoOptimizar += etiquetaTrue.id + ":\n";
                                codigoOptimizar += "[instrucciones_" + etiquetaTrue.id + "]\n";
                                codigoOptimizar += etiquetaFalse.id + ":\n";
                                codigoOptimizar += "[instrucciones_" + etiquetaFalse.id + "]\n";

                                codigoAugus = "if(" + condicionNueva + ") goto " + etiquetaFalse.id + ";\n";
                                string codigoOptimizado = codigoAugus;
                                codigoOptimizado += "[instrucciones_" + etiquetaTrue.id + "]\n";
                                codigoOptimizado += etiquetaFalse.id + ":\n";
                                codigoOptimizado += "[instrucciones_" + etiquetaFalse.id + "]\n";

                                optimizacion.antes = codigoOptimizar;
                                optimizacion.despues = codigoOptimizado;
                                optimizacion.regla = "Regla 3";
                                optimizacion.tipo = "Mirilla - Eliminación de Código Inalcanzable";
                                reporte.agregarOpt(optimizacion);
                                etiquetaTrue.imprimirEtiqueta = false;
                                //etiquetaTrue.ast = ast;
                                codigoAugus += etiquetaTrue.optimizarCodigo(reporte,ast);

                                ast.etiquetasBetadas.AddLast(etiquetaTrue.id);
                            }
                        }
                    }
                }
            } catch(Exception)
            {
                return null;
            }
            return codigoAugus;
        }

        
    }
}
