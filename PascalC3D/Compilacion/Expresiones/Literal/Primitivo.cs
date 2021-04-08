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
    class Primitivo : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Tipos type;
        public object value;

        public Primitivo(Tipos type, object value, int linea, int columna)
        {
            this.linea = linea;
            this.columna = columna;
            this.type = type;
            this.value = value;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            switch (type)
            {
                case Tipos.INTEGER:
                case Tipos.REAL:
                    return new Retorno(value.ToString(),false, new Tipo(type));
                case Tipos.BOOLEAN:
                    string valor;
                    if ((bool)value) valor = "1";
                    else valor = "0";
                    Generator generator = Generator.getInstance();
                    this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                    this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                    
                    Retorno retorno = new Retorno(valor, false, new Tipo(type));
                    retorno.trueLabel = this.trueLabel;
                    retorno.falseLabel = this.falseLabel;
                    return retorno;
                case Tipos.VOID:
                    return new Retorno("-1", false, new Tipo(type));
                default:
                    throw new Error("Semántico", "Tipo de dato no reconocido",ent.obtenerAmbito(), linea, columna);
            }
        }
    }
}
