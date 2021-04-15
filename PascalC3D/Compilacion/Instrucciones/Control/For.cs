using PascalC3D.Compilacion.Expresiones.Access;
using PascalC3D.Compilacion.Expresiones.Aritmetica;
using PascalC3D.Compilacion.Expresiones.Asignacion;
using PascalC3D.Compilacion.Expresiones.Literal;
using PascalC3D.Compilacion.Expresiones.Relacional;
using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Instrucciones.Variables;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Control
{
    class For : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private Expresion primero;
        private string fad;
        private Expresion segundo;
        private LinkedList<Instruccion> sentencias;

        public For(string id, Expresion primero, string fad, Expresion segundo, LinkedList<Instruccion> sentencias, int linea, int columna)
        {
            this.id = id;
            this.primero = primero;
            this.fad = fad;
            this.segundo = segundo;
            this.sentencias = sentencias;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            //CREO MIS UTILIDADES
            //ASIGNACION INICIAL
            AsignacionId target = new AsignacionId(id, null, linea, columna);
            Asignacion asignacionInicial = new Asignacion(target, primero, linea, columna);
            //ACTUALIZACION DE LA VARIABLE Y CONDICION A EVALUAR
            Asignacion actualizarVariable;
            Primitivo valorFAD = new Primitivo(Tipos.INTEGER, "1", linea, columna);
            Expresion condicion;
            AccessId left = new AccessId(id,null,linea,columna);
            if (fad.Equals("to"))
            {
                Less menorIgual = new Less(true, left, segundo, linea, columna);
                condicion = (Expresion)menorIgual;
                Suma suma = new Suma(left, valorFAD, linea, columna);
                actualizarVariable = new Asignacion(target, (Expresion)suma, linea, columna);

            } else //downto
            {
                Greater mayorIgual = new Greater(true,left,segundo,linea,columna);
                condicion = (Expresion)mayorIgual;
                Resta resta = new Resta(left, valorFAD, linea, columna);
                actualizarVariable = new Asignacion(target, (Expresion)resta, linea, columna);
            }
            //INICIO LA COMPILACION
            try
            {
                Generator generator = Generator.getInstance();
                generator.addComment("Inicia FOR");
                asignacionInicial.compilar(ent, errores);
                string lblFor = generator.newLabel();
                generator.addLabel(lblFor);
                Retorno retcondicion = condicion.compilar(ent);
                if (retcondicion.type.tipo == Tipos.BOOLEAN)
                {
                    ent.fors.AddLast(new IteFor(true, actualizarVariable));
                    ent.ybreak.AddLast(retcondicion.falseLabel);
                    ent.ycontinue.AddLast(lblFor);
                    generator.addLabel(retcondicion.trueLabel);
                    foreach (Instruccion sentencia in sentencias) sentencia.compilar(ent, errores);
                    actualizarVariable.compilar(ent, errores);
                    generator.addGoto(lblFor);
                    generator.addLabel(retcondicion.falseLabel);
                    ent.fors.RemoveLast();
                    ent.ybreak.RemoveLast();
                    ent.ycontinue.RemoveLast();
                    generator.addComment("Finaliza FOR");
                }
                else throw new Error("Semántico", "La condicion a evaluar en el for no es de tipo Boolean", ent.obtenerAmbito(), linea, columna);

            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
