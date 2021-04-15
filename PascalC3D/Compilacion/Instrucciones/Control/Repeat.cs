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
    class Repeat : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion condicion;
        private LinkedList<Instruccion> sentencias;

        public Repeat(Expresion condicion, LinkedList<Instruccion> sentencias, int linea, int columna)
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
                generator.addComment("Inicia Repeat");
                this.condicion.falseLabel = generator.newLabel();
                this.condicion.trueLabel = generator.newLabel();
                ent.fors.AddLast(new IteFor(false, null));
                ent.ycontinue.AddLast(this.condicion.falseLabel);
                ent.ybreak.AddLast(this.condicion.trueLabel);
                generator.addLabel(this.condicion.falseLabel);
                foreach (Instruccion sentencia in sentencias) sentencia.compilar(ent, errores);
                Retorno condition = this.condicion.compilar(ent);
                if (condition.type.tipo == Tipos.BOOLEAN)
                {
                    generator.addLabel(condition.trueLabel);
                    ent.fors.RemoveLast();
                    ent.ycontinue.RemoveLast();
                    ent.ybreak.RemoveLast();
                    generator.addComment("Finaliza Repeat");
                }
                else throw new Error("Semántico", "La condicion a evaluar en el repeat no es de tipo Boolean", ent.obtenerAmbito(), linea, columna);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
