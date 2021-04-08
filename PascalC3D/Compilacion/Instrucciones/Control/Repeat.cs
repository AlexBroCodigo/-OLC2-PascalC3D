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
                Entorno newEnv = new Entorno(ent, true);
                generator.addComment("Inicia Repeat");
                newEnv.ycontinue = this.condicion.falseLabel = generator.newLabel();
                newEnv.ybreak = this.condicion.trueLabel = generator.newLabel();
                generator.addLabel(this.condicion.falseLabel);
                foreach (Instruccion sentencia in sentencias) sentencia.compilar(newEnv, errores);
                Retorno condition = this.condicion.compilar(ent);
                if (condition.type.tipo == Tipos.BOOLEAN)
                {
                    generator.addLabel(condition.trueLabel);
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
