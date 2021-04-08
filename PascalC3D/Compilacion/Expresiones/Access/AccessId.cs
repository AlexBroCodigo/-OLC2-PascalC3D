﻿using PascalC3D.Compilacion.Generador;
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
                    if (symbol.type.tipo != Tipos.BOOLEAN) return new Retorno(temp, true, symbol.type, symbol);
                    //Mi modificacion
                    if (vieneDeRelacional) return new Retorno(temp, true, symbol.type, symbol);
                    
                    Retorno retorno = new Retorno("", false, symbol.type, symbol);
                    this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                    this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                    generator.addIf(temp,"1","==", this.trueLabel);
                    generator.addGoto(this.falseLabel);
                    retorno.trueLabel = this.trueLabel;
                    retorno.falseLabel = this.falseLabel;
                    return retorno;
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
                //pediente
                return null;
            }
        }
    }
}