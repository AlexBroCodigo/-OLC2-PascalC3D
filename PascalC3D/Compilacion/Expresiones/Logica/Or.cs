using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Logica
{
    class Or : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;

        public Or(Expresion left, Expresion right, int linea, int columna)
        {
            this.left = left;
            this.right = right;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
            this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;

            this.left.trueLabel = this.right.trueLabel = this.trueLabel;
            this.left.falseLabel = generator.newLabel();
            this.right.falseLabel = this.falseLabel;

            generator.addComment("Inicia Or");
            Retorno left = this.left.compilar(ent);
            generator.addLabel(this.left.falseLabel);
            Retorno right = this.right.compilar(ent);
            generator.addComment("Finaliza Or");

            Tipos tipoResultado = TablaTipos.obtenerTipo("or", left.type, right.type);
            if (tipoResultado == Tipos.ERROR) throw new Error("Semántico", "No se puede evaluar un OR entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(), linea, columna);
            Tipo tipo = new Tipo(tipoResultado); //INTEGER o BOOLEAN
            Retorno retorno = new Retorno("", false, tipo);
            retorno.trueLabel = this.trueLabel;
            retorno.falseLabel = this.right.falseLabel;
            return retorno;
        }
    }
}
