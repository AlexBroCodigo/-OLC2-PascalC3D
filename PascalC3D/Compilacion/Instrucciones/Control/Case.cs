using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalC3D.Compilacion.Instrucciones.Control
{
    class Case : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion variable;
        private LinkedList<Opcion> opciones;
        private LinkedList<Instruccion> sentenciasElse;

        public Case(Expresion variable, LinkedList<Opcion> opciones, LinkedList<Instruccion> sentenciasElse, int linea, int columna)
        {
            this.variable = variable;
            this.opciones = opciones;
            this.sentenciasElse = sentenciasElse;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Generator generator = Generator.getInstance();
                generator.addComment("Inicia Case");
                //AJUSTANDO UTILIDADES
                LinkedList<If> listaifs = new LinkedList<If>();
                foreach (Opcion opcion in opciones)
                {
                    opcion.variable = variable;
                    listaifs.AddLast((If)opcion.compilar(ent, errores));
                }   
                for(int i = 0; i < listaifs.Count; i++)
                {
                    If actual;
                    if (i == listaifs.Count - 1)
                    {
                        actual = listaifs.ElementAt(i);
                        if (this.sentenciasElse != null)
                        {
                            actual.sentenciasElse = this.sentenciasElse;
                        }
                    } else
                    {
                        LinkedList<Instruccion> lista = new LinkedList<Instruccion>();
                        actual = listaifs.ElementAt(i);
                        If siguiente = listaifs.ElementAt(i + 1);
                        lista.AddLast((Instruccion)siguiente);
                        actual.sentenciasElse = lista;
                    }
                }
                If ifmaster = listaifs.First.Value;

                //AREA DE COMPILACION
                ifmaster.compilar(ent, errores);
                generator.addComment("Finaliza Case");
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
