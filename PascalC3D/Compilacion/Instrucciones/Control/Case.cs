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
                for(int i = 0; i < opciones.Count; i++)
                {
                    Opcion opcion = opciones.ElementAt(i);
                    opcion.variable = variable;
                    if (i == opciones.Count - 1) opcion.sentenciasElse = sentenciasElse;
                    opcion.compilar(ent, errores);
                }
                generator.addComment("Finaliza Case");
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
