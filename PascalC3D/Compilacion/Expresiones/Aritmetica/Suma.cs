using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Aritmetica
{
    class Suma : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;

        public Suma(Expresion left,Expresion right, int linea, int columna)
        {
            this.left = left;
            this.right = right;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Retorno left = this.left.compilar(ent);
            Retorno right = this.right.compilar(ent);
            Tipos tipoResultado = TablaTipos.obtenerTipo("+", left.type, right.type);
            if (tipoResultado == Tipos.ERROR) throw new Error("Semántico", "No se puede evaluar una suma entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(),linea,columna);
            Tipo tipo = new Tipo(tipoResultado);
            Generator generator = Generator.getInstance();
            string temp = generator.newTemporal();
            switch (tipoResultado)
            {
                case Tipos.INTEGER:
                case Tipos.REAL:
                    generator.addExpression(temp, left.getValue(), right.getValue(), "+");
                    return new Retorno(temp, true, tipo);
                default: //STRING
                    generator.addComment("Inicia Plus String");
                    string tempAux = generator.newTemporal();
                    generator.freeTemp(tempAux);
                    generator.addExpression(tempAux, "SP",""+(ent.getSize() + 1), "+");
                    generator.addSetStack(tempAux, left.getValue());
                    generator.addExpression(tempAux, tempAux, "1", "+");
                    generator.addSetStack(tempAux, right.getValue());
                    generator.addNextEnv(ent.getSize());
                    generator.addCall("native_concat_str");
                    generator.addGetStack(temp, "SP");
                    generator.addAntEnv(ent.getSize());
                    generator.addComment("Finaliza Plus String");
                    return new Retorno(temp, true, tipo);
            }
        }
    }
}
