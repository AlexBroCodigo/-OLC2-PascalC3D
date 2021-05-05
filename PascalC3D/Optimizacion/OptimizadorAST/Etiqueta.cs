using PascalC3D.Optimizacion.OptimizadorCondicionales;
using PascalC3D.Optimizacion.OptimizadorValorImplicito;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Optimizacion.OptimizadorAST
{
    class Etiqueta : Instruccion
    {

        public string id;
        public LinkedList<Instruccion> instrucciones;
        public int linea;
        public int columna;
        public string codigoOptimizado;
        public bool imprimirEtiqueta;

        public Etiqueta(string id, LinkedList<Instruccion> instrucciones, int linea, int columna)
        {
            this.id = id;
            this.instrucciones = instrucciones;
            this.linea = linea;
            this.columna = columna;
            codigoOptimizado = "";
            imprimirEtiqueta = true;
        }

        private string traducirCodigo(ReporteOptimizacion reporte,AST ast,LinkedList<Instruccion> instrucciones,bool aplicaBloque)
        {
            int contador = 0;
            string codigoOptimizado = "";
            Instruccion instruccionAnterior = null;
            Asignacion asignacionPrevia = null;
            string codigoAnterior = "";

            foreach(Instruccion ins in instrucciones)
            {
                if(ins is Asignacion)
                {
                    Asignacion asig = (Asignacion)ins;
                    asig.instruccionPrevia = asignacionPrevia;
                    asignacionPrevia = (Asignacion)ins; 
                }
                else if(ins is GOTO)
                {
                    GOTO insgoto = (GOTO)ins;
                    insgoto.ast = ast;
                }
                else if(ins is If)
                {
                    If insif = (If)ins;
                    for(int i = contador+1; i < this.instrucciones.Count; i++)
                    {
                        insif.instrucciones.AddLast(this.instrucciones.ElementAt(i));
                    }
                }

                string optimizado = "";
                if(ins is If)
                {
                    If insif = (If)ins;
                    insif.ast = ast; //necesario antes de optimizar cada if
                    optimizado = insif.optimizarCodigo(reporte).codigo;
                }
                else
                {
                    if(instruccionAnterior is If && ins is GOTO)
                    {
                        If antif = (If)instruccionAnterior;
                        if(!antif.seAplicoRegla3) optimizado = ins.optimizarCodigo(reporte).codigo;
                    } else optimizado = ins.optimizarCodigo(reporte).codigo; 
                }

                //Regla 2 Mirilla
                if(ins is GOTO)
                {
                    if (codigoAnterior.StartsWith("goto"))
                    {
                        if(instruccionAnterior is If)
                        {
                            codigoAnterior = "";
                            continue;
                        }
                    }
                    GOTO insgoto = (GOTO)ins;
                    if (ast.existeEtiqueta(insgoto.id))
                    {
                        if (!optimizado.Equals(""))
                        {
                            codigoOptimizado += "   " + optimizado;
                            codigoAnterior = optimizado;
                        }
                        if ((contador + 1) == this.instrucciones.Count) continue; //si no existen mas instrucciones no hay optimizacion
                        OPtimizacion optimizacion = new OPtimizacion(); //si hay optimizacion
                        optimizacion.linea = "" + (insgoto.linea + 1);
                        string codigoOptimizar = "";
                        for (int i = contador + 1; i < this.instrucciones.Count; i++)
                        {
                            Instruccion instruccion = this.instrucciones.ElementAt(i);
                            if (instruccion is GOTO)
                            {
                                GOTO mygoto = (GOTO)instruccion;
                                mygoto.ast = ast;
                            }
                            else if (instruccion is If) continue;
                            codigoOptimizar += instruccion.optimizarCodigo(reporte).codigo;
                        }
                        optimizacion.antes = codigoOptimizar;
                        optimizacion.despues = insgoto.id + ":\n";
                        optimizacion.regla = "Regla 1";
                        optimizacion.tipo = "Mirilla - Eliminación de Código Inalcanzable";
                        reporte.agregarOpt(optimizacion);
                        codigoAnterior = "";
                        break;
                    }
                    else
                    {
                        if (!optimizado.Equals(""))
                        {
                            codigoOptimizado += "   " + optimizado;
                            codigoAnterior = optimizado;
                        }
                    }
                }
                else
                {
                    if (!optimizado.Equals(""))
                    {
                        codigoOptimizado += "   " + optimizado;
                        codigoAnterior = optimizado;
                    }
                }
                instruccionAnterior = ins;
                contador++;
            }
            return codigoOptimizado;
        }



        public string optimizarCodigo(ReporteOptimizacion reporte, AST ast, bool aplicaBloque = false)
        {
            this.codigoOptimizado = "";
            if (this.imprimirEtiqueta) this.codigoOptimizado += this.id + ":\n";
            string strResultado = this.traducirCodigo(reporte,ast,this.instrucciones,aplicaBloque);
            this.codigoOptimizado += strResultado;
            return this.codigoOptimizado;
        }

        public OptimizacionResultado optimizarCodigo(ReporteOptimizacion reporte)
        {
            return null;
        }

        public string generarAugus(ReporteOptimizacion reporte)
        {
            return "";
        }

        
    }
}
