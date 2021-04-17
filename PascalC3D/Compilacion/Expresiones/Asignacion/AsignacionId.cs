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
            generator.addComment("Inicia AsignacionId");
            if (this.anterior == null)
            {
                Simbolo symbol = ent.getVar(id,linea,columna);
                if (symbol.isRef)
                {

                    string tempAux = generator.newTemporal();
                    string temp = generator.newTemporal();
                    generator.freeTemp(tempAux);
                    generator.addExpression(tempAux, "SP", "" + symbol.position, "+");
                    generator.addExpression(temp, tempAux, "1", "+");
                    generator.addGetStack(temp, temp);

                    this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                    this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                    generator.addIf(temp, "1", "==", this.trueLabel);
                    generator.addGoto(this.falseLabel);

                    string templabel = generator.newLabel();
                    generator.addLabel(this.trueLabel);
                    generator.addGetStack(temp, tempAux);
                    generator.addGoto(templabel);
                    generator.addLabel(this.falseLabel);
                    generator.addGetHeap(temp, tempAux);
                    generator.addLabel(templabel);
                    generator.addComment("Finaliza AsignacionId");
                    return new Retorno(temp, true, symbol.type, symbol); //OBTENGO LA COORDENADA DE LA VARIABLE POR REFERENCIA
                }
                else if (symbol.isGlobal)
                {
                    generator.addComment("Finaliza AsignacionId");
                    return new Retorno("" + symbol.position, false, symbol.type, symbol);
                }
                else //isHeap
                {
                    string temp = generator.newTemporal();
                    generator.addExpression(temp, "SP", "" + symbol.position, "+");
                    generator.addComment("Finaliza AsignacionId");
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
                generator.addComment("Finaliza AsignacionId");
                return new Retorno(temp, true, attribute.value.type, new Simbolo(attribute.value.type, this.id, attribute.index, false, false, true,linea,columna));
            }
        }
    }
}
