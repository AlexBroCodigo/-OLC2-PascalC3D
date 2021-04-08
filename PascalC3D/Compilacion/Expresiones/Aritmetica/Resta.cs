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
    class Resta : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;

        public Resta(Expresion left, Expresion right,int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.left = left;
            this.right = right;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Retorno left = this.left.compilar(ent);
            Retorno right = this.right.compilar(ent);
            Tipos tipoResultado = TablaTipos.obtenerTipo("-", left.type, right.type);
            if (tipoResultado == Tipos.ERROR) throw new Error("Semántico", "No se puede evaluar una resta entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(), linea, columna);
            Tipo tipo = new Tipo(tipoResultado);
            Generator generator = Generator.getInstance();
            string temp = generator.newTemporal();
            //INTEGER, REAL
            generator.addExpression(temp, left.getValue(), right.getValue(), "-");
            return new Retorno(temp, true, tipo);
        }
    }
}
