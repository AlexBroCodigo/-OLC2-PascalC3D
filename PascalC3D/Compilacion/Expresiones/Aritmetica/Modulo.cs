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
    class Modulo : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;

        public Modulo(Expresion left, Expresion right, int linea, int columna)
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
            Tipos tipoResultado = TablaTipos.obtenerTipo("mod", left.type, right.type);
            if (tipoResultado == Tipos.ERROR) throw new Error("Semántico", "No se puede evaluar un modulo entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(), linea, columna);
            Tipo tipo = new Tipo(tipoResultado);
            Generator generator = Generator.getInstance();
            string temp = generator.newTemporal();
            //INTEGER
            if (right.valorToString().Equals("0")) throw new Error("Semántico", "Resultado indefinido, no se puede realizar un modulo entre 0", ent.obtenerAmbito(), linea, columna);
            generator.addExpression(temp, left.getValue(), right.getValue(), "%");
            return new Retorno(temp, true, tipo);
        }
    }
}
