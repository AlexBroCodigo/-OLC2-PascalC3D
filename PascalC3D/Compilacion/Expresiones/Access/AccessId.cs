using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Access
{
    class AccessId : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private Expresion anterior;

        public bool vieneDeRelacional;

        public AccessId(string id, Expresion anterior, int linea, int columna)
        {
            this.id = id;
            this.anterior = anterior;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
            vieneDeRelacional = false;
        }

        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            generator.addComment("Inicia AccessId");
            if (this.anterior == null)
            {
                Simbolo symbol = ent.getVar(this.id, linea, columna);
                string temp = generator.newTemporal();
                if (symbol.isGlobal)
                {
                    generator.addGetStack(temp,""+symbol.position);
                    if (symbol.type.tipo != Tipos.BOOLEAN && symbol.type.tipo != Tipos.STRUCT) return new Retorno(temp, true, symbol.type, symbol);
                    //MI MODIFICACION
                    if(symbol.type.tipo == Tipos.BOOLEAN)
                    {
                        if (vieneDeRelacional) return new Retorno(temp, true, symbol.type, symbol);
                        Retorno retorno = new Retorno("", false, symbol.type, symbol);
                        this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                        this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                        generator.addIf(temp, "1", "==", this.trueLabel);
                        generator.addGoto(this.falseLabel);
                        retorno.trueLabel = this.trueLabel;
                        retorno.falseLabel = this.falseLabel;
                        return retorno;
                    } else //STRUCT
                    {
                        Retorno retorno = new Retorno(temp, true, symbol.type, symbol);
                        this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                        this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                        retorno.trueLabel = this.trueLabel;
                        retorno.falseLabel = this.falseLabel;
                        return retorno;
                    }
                } else
                {
                    string tempAux = generator.newTemporal(); 
                    generator.freeTemp(tempAux);
                    generator.addExpression(tempAux,"SP",""+symbol.position,"+");
                    generator.addGetStack(temp, tempAux);
                    if (symbol.type.tipo != Tipos.BOOLEAN) return new Retorno(temp, true, symbol.type, symbol);

                    Retorno retorno = new Retorno("", false, symbol.type);
                    this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                    this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                    generator.addIf(temp,"1","==", this.trueLabel);
                    generator.addGoto(this.falseLabel);
                    retorno.trueLabel = this.trueLabel;
                    retorno.falseLabel = this.falseLabel;
                    return retorno;
                }
            } else
            {
                Retorno anterior = this.anterior.compilar(ent);
                SimboloStruct symStruct = anterior.type.symStruct;
                if (anterior.type.tipo != Tipos.STRUCT || symStruct == null) throw new Error("Semántico", "Acceso no valido para el tipo: " + anterior.type.tipo,ent.obtenerAmbito(),linea,columna);
                Jackson attribute = symStruct.getAttribute(this.id);
                if(attribute.value == null) throw new Error("Semántico", "El object no tiene el atributo: "+this.id, ent.obtenerAmbito(), linea, columna);

                string tempAux = generator.newTemporal();
                generator.freeTemp(tempAux);
                string temp = generator.newTemporal();

                generator.addExpression(tempAux, anterior.getValue(),""+attribute.index,"+"); //Busca la posicion del atributo
                generator.addGetHeap(temp, tempAux); //Trae el valor del heap
                
                Retorno retorno = new Retorno(temp, true, attribute.value.type);
                retorno.trueLabel = anterior.trueLabel;
                retorno.falseLabel = anterior.falseLabel;
                return retorno;
            }
        }
    }
}
