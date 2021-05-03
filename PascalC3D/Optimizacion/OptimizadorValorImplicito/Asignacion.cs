using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorValorImplicito
{
    class Asignacion : Instruccion
    {
        private int linea;
        private int columna;
        private string id;
        private Operacion valor;
        public Asignacion instruccionPrevia;

        public Asignacion(string id, Operacion valor,int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.id = id;
            this.valor = valor;
            this.instruccionPrevia = null;
        }

        public OptimizacionResultado optimizarCodigo(ReporteOptimizacion reporte)
        {
            string antes = generarAugus(reporte);
            OptimizacionResultado resultado = new OptimizacionResultado();
            resultado.codigo = antes;
            return resultado;
        }

        public string generarAugus(ReporteOptimizacion reporte)
        {
            string codigoAugus = this.id + " = " + this.valor.generarAugus() + ";\n";
            OPtimizacion optimizacion = new OPtimizacion();
            optimizacion.linea = "" + (linea + 1);
            optimizacion.antes = codigoAugus;
            optimizacion.tipo = "Mirilla - Simplificación algebraica y por fuerza";

            if(this.valor.tipo == Operacion.TIPO_OPERACION.SUMA)
            {
                if (this.valor.validarRegla8(this.id))
                {
                    optimizacion.regla = "Regla 8";
                    optimizacion.despues = "";
                    reporte.agregarOpt(optimizacion);
                    return "";
                } else if (!this.valor.validarRegla12().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla12() + ";\n";
                    optimizacion.regla = "Regla 12";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
            }
            else if(this.valor.tipo == Operacion.TIPO_OPERACION.RESTA)
            {
                if (this.valor.validarRegla9(this.id))
                {
                    optimizacion.regla = "Regla 9";
                    optimizacion.despues = "";
                    reporte.agregarOpt(optimizacion);
                    return "";
                }
                else if (!this.valor.validarRegla13().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla13() + ";\n";
                    optimizacion.regla = "Regla 13";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
            }
            else if(this.valor.tipo == Operacion.TIPO_OPERACION.MULTIPLICACION)
            {
                if (this.valor.validarRegla10(this.id))
                {
                    optimizacion.regla = "Regla 10";
                    optimizacion.despues = "";
                    reporte.agregarOpt(optimizacion);
                    return "";
                } else if (!this.valor.validarRegla14().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla14() + ";\n";
                    optimizacion.regla = "Regla 14";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
                else if (!this.valor.validarRegla16().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla16() + ";\n";
                    optimizacion.regla = "Regla 16";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
                else if (!this.valor.validarRegla17().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla17() + ";\n";
                    optimizacion.regla = "Regla 17";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
            } 
            else if(this.valor.tipo == Operacion.TIPO_OPERACION.DIVISION)
            {
                if (this.valor.validarRegla11(this.id))
                {
                    optimizacion.regla = "Regla 11";
                    optimizacion.despues = "";
                    reporte.agregarOpt(optimizacion);
                    return "";
                }
                else if (!this.valor.validarRegla15().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla15() + ";\n";
                    optimizacion.regla = "Regla 15";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                } else if (!this.valor.validarRegla18().Equals(""))
                {
                    codigoAugus = this.id + " = " + this.valor.validarRegla18() + ";\n";
                    optimizacion.regla = "Regla 18";
                    optimizacion.despues = codigoAugus;
                    reporte.agregarOpt(optimizacion);
                }
            }
            else if (valor.tipo == Operacion.TIPO_OPERACION.ID)
            {
                codigoAugus = this.id + " = " + this.valor.generarAugus() + ";\n";
                if (this.instruccionPrevia != null)
                {
                    if(this.instruccionPrevia.valor.tipo == Operacion.TIPO_OPERACION.ID)
                    {
                        //MI REGLA 5
                        if (this.valor.validarRegla1(this.id, this.valor.valor, this.instruccionPrevia.id, this.instruccionPrevia.valor.valor))
                        {
                            optimizacion.tipo = "Mirilla - Eliminación de Instrucciones Redundantes y de Almacenamiento";
                            optimizacion.regla = "Regla 1";
                            optimizacion.despues = "";
                            reporte.agregarOpt(optimizacion);
                            return "";
                        }
                    }
                }
            }
            else codigoAugus = this.id + " = " + this.valor.generarAugus() + ";\n";

            return codigoAugus;
            
        }

        
    }
}
