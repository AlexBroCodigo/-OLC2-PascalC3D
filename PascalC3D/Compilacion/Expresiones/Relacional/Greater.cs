using PascalC3D.Compilacion.Expresiones.Access;
using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Relacional
{
    class Greater : Expresion
    {
        public string trueLabel { get;set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;
        private bool isGrtEqual;

        public Greater(bool isGrtEqual, Expresion left, Expresion right, int linea, int columna)
        {
            this.isGrtEqual = isGrtEqual;
            this.left = left;
            this.right = right;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            //Mi modificacion
            if (this.left is AccessId)
            {
                AccessId access = (AccessId)this.left;
                access.vieneDeRelacional = true;
            }
            Retorno left = this.left.compilar(ent);
            Retorno right;
            Generator generator = Generator.getInstance();
            switch (left.type.tipo)
            {
                case Tipos.INTEGER:
                case Tipos.REAL:
                    right = this.right.compilar(ent);
                    switch (right.type.tipo)
                    {
                        case Tipos.INTEGER:
                        case Tipos.REAL:
                            this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                            this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                            if (this.isGrtEqual) generator.addIf(left.getValue(), right.getValue(), ">=", this.trueLabel);
                            else generator.addIf(left.getValue(), right.getValue(), ">", this.trueLabel);
                            generator.addGoto(this.falseLabel);
                            Retorno retorno = new Retorno("", false, new Tipo(Tipos.BOOLEAN));
                            retorno.trueLabel = this.trueLabel;
                            retorno.falseLabel = this.falseLabel;
                            return retorno;
                        default: break;
                    }
                    break;
                case Tipos.BOOLEAN:
                    //Mi modificacion
                    if (this.right is AccessId)
                    {
                        AccessId access = (AccessId)this.right;
                        access.vieneDeRelacional = true;
                    }
                    right = this.right.compilar(ent);
                    this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                    this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                    if (isGrtEqual) generator.addIf(left.getValue(), right.getValue(), ">=", trueLabel);
                    else generator.addIf(left.getValue(), right.getValue(), ">", trueLabel);
                    generator.addGoto(falseLabel);
                    if (right.type.tipo == Tipos.BOOLEAN)
                    {
                        Retorno retorno = new Retorno("", false, new Tipo(Tipos.BOOLEAN));
                        retorno.trueLabel = this.trueLabel;
                        retorno.falseLabel = this.falseLabel;
                        return retorno;
                    }
                    break;
                case Tipos.STRING:
                    right = this.right.compilar(ent);
                    if (right.type.tipo == Tipos.STRING)
                    {
                        string temp = generator.newTemporal();
                        string tempAux = generator.newTemporal();
                        generator.freeTemp(tempAux);
                        generator.addExpression(tempAux, "SP", "" + (ent.getSize() + 1), "+");
                        generator.addSetStack(tempAux, left.getValue());
                        generator.addExpression(tempAux, tempAux, "1", "+");
                        generator.addSetStack(tempAux, right.getValue());
                        generator.addNextEnv(ent.getSize());
                        if(this.isGrtEqual) generator.addCall("native_greaterEq_str");
                        else generator.addCall("native_greater_str");
                        generator.addGetStack(temp, "SP");
                        generator.addAntEnv(ent.getSize());

                        this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                        this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                        generator.addIf(temp, "1", "==", this.trueLabel);
                        generator.addGoto(this.falseLabel);
                        Retorno retorno = new Retorno("", false, new Tipo(Tipos.BOOLEAN));
                        retorno.trueLabel = this.trueLabel;
                        retorno.falseLabel = this.falseLabel;
                        return retorno;
                    }
                    break;
                default: right = new Retorno("", false, new Tipo(Tipos.ERROR)); break;//ERROR
            }
            throw new Error("Semántico", "No se puede evaluar un > o >= entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(), linea, columna);
        }
    }
}
