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
    class While : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion condicion;
        private LinkedList<Instruccion> sentencias;

        public While(Expresion condicion, LinkedList<Instruccion> sentencias, int linea, int columna)
        {
            this.condicion = condicion;
            this.sentencias = sentencias;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Generator generator = Generator.getInstance();
                string lblWhile = generator.newLabel();
                generator.addComment("Inicia while");
                generator.addLabel(lblWhile);
                Retorno condicion = this.condicion.compilar(ent);
                if (condicion.type.tipo == Tipos.BOOLEAN)
                {
                    ent.fors.AddLast(new IteFor(false, null));
                    ent.ybreak.AddLast(condicion.falseLabel);
                    ent.ycontinue.AddLast(lblWhile);
                    generator.addLabel(condicion.trueLabel);
                    foreach (Instruccion sentencia in sentencias) sentencia.compilar(ent, errores);
                    generator.addGoto(lblWhile);
                    generator.addLabel(condicion.falseLabel);
                    ent.fors.RemoveLast();
                    ent.ybreak.RemoveLast();
                    ent.ycontinue.RemoveLast();
                    generator.addComment("Finaliza while");
                }
                else throw new Error("Semántico", "La condicion a evaluar en el while no es de tipo Boolean", ent.obtenerAmbito(), linea, columna);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
