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
    class RestaUni : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get;set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion unario;

        public RestaUni(Expresion unario, int linea, int columna)
        {
            this.unario = unario;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Retorno unario = this.unario.compilar(ent);
            if (!(unario.type.tipo == Tipos.INTEGER || unario.type.tipo == Tipos.REAL)) throw new Error("Semántico","No se puede evaluar una resta unaria con un "+ unario.type.tipoToString(),ent.obtenerAmbito(), linea, columna);
            Generator generator = Generator.getInstance();
            string temp = generator.newTemporal();
            //INTEGER, REAL
            generator.addExpression(temp,"0",unario.getValue(),"-");
            return new Retorno(temp,true,unario.type);

        }
    }
}
