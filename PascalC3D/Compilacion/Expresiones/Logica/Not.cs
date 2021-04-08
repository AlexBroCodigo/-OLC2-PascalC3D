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
    class Not : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion value;

        public Not(Expresion value, int linea, int columna)
        {
            this.value = value;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
            this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
            
            this.value.trueLabel = this.falseLabel;
            this.value.falseLabel = this.trueLabel;

            Retorno value = this.value.compilar(ent);
            if(value.type.tipo == Tipos.BOOLEAN)
            {
                Retorno retorno = new Retorno("",false,value.type);
                retorno.trueLabel = this.trueLabel;
                retorno.falseLabel = this.falseLabel;
                return retorno;
            } else throw new Error("Semántico", "No se puede evaluar un NOT con un " + value.type.tipoToString(), ent.obtenerAmbito(), linea, columna);

        }
    }
}
