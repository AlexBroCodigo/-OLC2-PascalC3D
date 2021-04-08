using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Literal
{
    class PrimitivoString : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Tipos type;
        private string value;

        public PrimitivoString(Tipos type,string value,int linea,int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.type = type;
            this.value = value;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            generator.addComment("Inicia PrimitivoString");
            string temp = generator.newTemporal();
            generator.addExpression(temp, "HP");
            for(int i = 0;i < value.Length; i++)
            {
                generator.addSetHeap("HP",""+(int)value[i]);
                generator.nextHeap();
            }
            generator.addSetHeap("HP", "-1");
            generator.nextHeap();
            generator.addComment("Finaliza PrimitivoString");
            return new Retorno(temp, true, new Tipo(type, "String"));
        }
    }
}
