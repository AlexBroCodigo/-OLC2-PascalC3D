using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Asignacion
{
    class AsignacionId : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }
        
        public string id;
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
                Retorno anterior = this.anterior.compilar(ent);
                if (anterior.type.tipo != Tipos.STRUCT) throw new Error("Semántico", "Acceso no valido para el tipo: " + anterior.type.tipo, ent.obtenerAmbito(), linea, columna);
                
                SimboloStruct symStruct = anterior.type.symStruct;
                Jackson attribute = symStruct.getAttribute(this.id);
                if (attribute.value == null) throw new Error("Semántico", "El object " + symStruct.identifier + "no tiene el atributo" + this.id,ent.obtenerAmbito(),linea,columna);

                string tempAux = generator.newTemporal();
                generator.freeTemp(tempAux);
                string temp = generator.newTemporal();
                if(anterior.symbol != null && !anterior.symbol.isHeap)
                {
                    //TODO variables por referencia
                    generator.addGetStack(tempAux, anterior.getValue());
                } else
                {
                    generator.addGetHeap(tempAux, anterior.getValue());
                }
                generator.addExpression(temp, tempAux, "" + attribute.index, "+");
                return new Retorno(temp, true, attribute.value.type, new Simbolo(attribute.value.type, this.id, attribute.index, false, false, true,linea,columna));
            }
        }
    }
}
