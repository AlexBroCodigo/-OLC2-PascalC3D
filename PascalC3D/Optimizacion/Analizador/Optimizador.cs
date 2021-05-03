using Irony.Parsing;
using PascalC3D.Optimizacion.OptimizadorAST;
using PascalC3D.Optimizacion.Reporte;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.Analizador
{
    class Optimizador
    {
        public string codigoOptimizado;
        public string codigoAnterior;
        public LinkedList<Etiqueta> instrucciones;
        public ReporteOptimizacion reporte;

        public Optimizador()
        {
            codigoOptimizado = "";
            codigoAnterior = "";
            instrucciones = null;
            reporte = null;
        }

        public void inicializar()
        {
            reporte = new ReporteOptimizacion();
            codigoOptimizado = "";
            codigoAnterior = "";
            instrucciones = new LinkedList<Etiqueta>();
        }

        public string optimizar(string texto, ParseTree arbol,bool aplicaBloques = false)
        {
            string codFuncion = ""; //mi variable
            string codInstrucciones = ""; //mi variable
            this.codigoAnterior = texto;
            this.codigoOptimizado = "";
            GeneradorOptiAST migenerador = new GeneradorOptiAST(arbol);
            LinkedList<Funcion> funciones = migenerador.funciones;
            codigoOptimizado += migenerador.head;
            foreach(Funcion funcion in funciones)
            {
                codInstrucciones = "";
                LinkedList<Etiqueta> instrucciones = funcion.instrucciones;
                this.instrucciones = instrucciones;
                AST ast = new AST(this.instrucciones);
                //PRIMERA PASADA: PARA GUARDAR TODAS LAS ETIQUETAS
                if(instrucciones != null)
                {
                    foreach(Etiqueta ins in instrucciones)
                    {
                        ast.agregarEtiqueta(ins);
                    }
                }

                //SEGUNDA PASADA: OPTIMIZAMOS
                if(instrucciones != null)
                {
                    foreach(Etiqueta func in instrucciones)
                    {
                        if (ast.etiquetasBetadas.Contains(func.id)) continue;
                        codInstrucciones += func.optimizarCodigo(reporte, ast, aplicaBloques);
                    }
                }
                codFuncion = "void " + funcion.nombre + "(){\n" + codInstrucciones + "}\n\n";
                codigoOptimizado += codFuncion;
            }
            return codigoOptimizado;
        }

        public void reportar()
        {
            reporte.generarReporteOptimizacion();
        }

    }
}
