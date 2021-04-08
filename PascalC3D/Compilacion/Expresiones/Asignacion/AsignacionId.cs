using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Expresiones.Asignacion
{
    class AsignacionId : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private Expresion anterior;

        public AsignacionId(string id, Expresion anterior, int linea, int columna)
        {
            this.id = id;
            this.anterior = anterior;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            if(this.anterior == null)
            {
                Simbolo symbol = ent.getVar(id,linea,columna);
                if (symbol.isGlobal) return new Retorno("" + symbol.position, false, symbol.type, symbol);
                else
                {
                    string temp = generator.newTemporal();
                    generator.addExpression(temp, "SP", "" + symbol.position, "+");
                    return new Retorno(temp, true, symbol.type, symbol);
                }
            } else
            {
                return null;
            }
        }
    }
}
