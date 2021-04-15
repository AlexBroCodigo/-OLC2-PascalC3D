using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Control
{
    class If : Instruccion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion condicion;
        private LinkedList<Instruccion> sentencias;
        public LinkedList<Instruccion> sentenciasElse;

        public If(Expresion condicion, LinkedList<Instruccion> sentencias, LinkedList<Instruccion> sentenciasElse, int linea, int columna)
        {
            this.condicion = condicion;
            this.sentencias = sentencias;
            this.sentenciasElse = sentenciasElse;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Generator generator = Generator.getInstance();
                generator.addComment("Inicia If");
                Retorno condicion = this.condicion.compilar(ent);
                if (condicion.type.tipo == Tipos.BOOLEAN)
                {
                    generator.addLabel(condicion.trueLabel);
                    foreach (Instruccion instruccion in sentencias) instruccion.compilar(ent, errores);
                    if (sentenciasElse != null)
                    {
                        string tempLbl = generator.newLabel();
                        generator.addGoto(tempLbl);
                        generator.addLabel(condicion.falseLabel);
                        foreach (Instruccion inselse in sentenciasElse) inselse.compilar(ent, errores);
                        generator.addLabel(tempLbl);
                    }
                    else generator.addLabel(condicion.falseLabel);
                    generator.addComment("Finaliza If");
                }
                else throw new Error("Semántico", "La condicion a evaluar en el if no es de tipo Boolean", ent.obtenerAmbito(), linea, columna);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
