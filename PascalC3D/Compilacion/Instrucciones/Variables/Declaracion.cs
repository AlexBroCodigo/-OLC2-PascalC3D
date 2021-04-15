using PascalC3D.Compilacion.Expresiones.Literal;
using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Variables
{
    class Declaracion : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        private Tipo type;
        private LinkedList<string> idList;
        private Expresion value;

        public Declaracion(Tipo type, LinkedList<string> idList, Expresion value, int linea, int columna)
        {
            this.type = type;
            this.idList = idList;
            this.value = value;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                Generator generator = Generator.getInstance();
                Retorno value;
                if (this.value != null)
                {
                    value = this.value.compilar(ent);
                    if (!sameType(this.type, value.type)) throw new Error("Semántico", "Valor no compatible con el tipo de dato", ent.obtenerAmbito(), linea, columna);
                }
                else //LITERAL POR DEFECTO
                {
                    switch (type.tipo)
                    {
                        case Tipos.INTEGER:
                            Primitivo defecto = new Primitivo(Tipos.INTEGER, "0", linea, columna);
                            value = defecto.compilar(ent);
                            break;
                        case Tipos.REAL:
                            Primitivo defecto1 = new Primitivo(Tipos.REAL, "0.0", linea, columna);
                            value = defecto1.compilar(ent);
                            break;
                        case Tipos.BOOLEAN:
                            Primitivo defecto2 = new Primitivo(Tipos.BOOLEAN, false, linea, columna);
                            value = defecto2.compilar(ent);
                            break;
                        default://case Tipos.STRING:
                            PrimitivoString defecto3 = new PrimitivoString(Tipos.STRING, "", linea, columna);
                            value = defecto3.compilar(ent);
                            break;
                    }
                }
                this.validateType(ent);
                foreach (string id in idList)
                {
                    Simbolo newVar = ent.addVar(id, type, false, false, linea, columna);
                    if (newVar.isGlobal)
                    {
                        if (this.type.tipo == Tipos.BOOLEAN)
                        {
                            //Mi modificacion
                            //para cuando viene mas de una variable en la declaracion
                            if(!idList.ElementAt(0).Equals(id))
                            {
                                value.trueLabel = generator.newLabel();
                                value.falseLabel = generator.newLabel();
                            }
                            if (value.getValue().Equals("1")) generator.addGoto(value.trueLabel);
                            else generator.addGoto(value.falseLabel);

                            string templabel = generator.newLabel();
                            generator.addLabel(value.trueLabel);
                            generator.addSetStack("" + newVar.position, "1");
                            generator.addGoto(templabel);
                            generator.addLabel(value.falseLabel);
                            generator.addSetStack("" + newVar.position, "0");
                            generator.addLabel(templabel);
                        }
                        else
                        {
                            generator.addSetStack("" + newVar.position, value.getValue());
                        }
                    }
                    else
                    {
                        string temp = generator.newTemporal();
                        generator.freeTemp(temp);
                        generator.addExpression(temp, "SP", "" + newVar.position, "+");
                        if (this.type.tipo == Tipos.BOOLEAN)
                        {
                            //Mi modificacion
                            //para cuando viene mas de una variable en la declaracion
                            if (!idList.ElementAt(0).Equals(id))
                            {
                                value.trueLabel = generator.newLabel();
                                value.falseLabel = generator.newLabel();
                            }
                            if (value.getValue().Equals("1")) generator.addGoto(value.trueLabel);
                            else generator.addGoto(value.falseLabel);

                            string templabel = generator.newLabel();
                            generator.addLabel(value.trueLabel);
                            generator.addSetStack(temp, "1");
                            generator.addGoto(templabel);
                            generator.addLabel(value.falseLabel);
                            generator.addSetStack(temp, "0");
                            generator.addLabel(templabel);
                        }
                        else
                        {
                            generator.addSetStack(temp, value.getValue());
                        }
                    }
                }
            }
            catch (Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }

        public void validateType(Entorno ent)
        {
            if(this.type.tipo == Tipos.STRUCT)
            {
                SimboloStruct @struct = ent.searchStruct(this.type.tipoId);
                if (@struct == null) throw new Error("Semántico","No existe el object: " + this.type.tipoId,ent.obtenerAmbito(),linea,columna);
                this.type.symStruct = @struct;
            }
        }

        private bool sameType(Tipo type1, Tipo type2)
        {
            if (type1.tipo == type2.tipo)
            {
                if (type1.tipo == Tipos.STRUCT) return type1.tipoId.ToLower().Equals(type2.tipoId.ToLower());
                return true;
            }
            else if (type1.tipo == Tipos.STRUCT || type2.tipo == Tipos.STRUCT)
            {
                if (type1.tipo == Tipos.VOID || type2.tipo == Tipos.VOID) return true;
            }
            return false;
        }
    }
}
