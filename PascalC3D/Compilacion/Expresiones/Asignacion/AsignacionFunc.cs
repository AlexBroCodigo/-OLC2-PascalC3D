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
    class AsignacionFunc : Expresion, Instruccion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;
        private Expresion anterior;
        private LinkedList<Expresion> parametros;

        public AsignacionFunc(string id, LinkedList<Expresion> parametros, Expresion anterior, int linea, int columna)
        {
            this.id = id;
            this.parametros = parametros;
            this.anterior = anterior;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            if(this.anterior == null)
            {
                SimboloFunction symFunc = ent.searchFunc(this.id);
                if (symFunc == null) throw new Error("Semántico","No se encontro la función: "+this.id, ent.obtenerAmbito(), linea, columna);
                LinkedList<Retorno> paramsValues = new LinkedList<Retorno>();
                Generator generator = Generator.getInstance();
                int size = generator.saveTemps(ent); //Guardo temporales

                foreach(Expresion param in this.parametros)
                {
                    paramsValues.AddLast(param.compilar(ent));
                }
                //TODO comprobar parametros correctos
                string temp = generator.newTemporal();
                generator.freeTemp(temp);
                //Paso de parametros en cambio simulado
                if(paramsValues.Count != 0)
                {
                    generator.addExpression(temp, "SP",""+(ent.size + 1), "+"); //+1 porque la posicion 0 es para el retorno;
                    int index = -1;
                    foreach(Retorno value in paramsValues)
                    {
                        //TODO paso de parametros booleanos
                        generator.addSetStack(temp, value.getValue());
                        index++;
                        if (index != paramsValues.Count - 1)
                            generator.addExpression(temp, temp, "1", "+");
                    }
                }
                
                generator.addNextEnv(ent.size);
                generator.addCall(symFunc.uniqueId);
                if(symFunc.type.tipo != Tipos.VOID) generator.addGetStack(temp, "SP");
                generator.addAntEnv(ent.size);

                generator.recoverTemps(ent, size);
                generator.addTemp(temp);

                if (symFunc.type.tipo != Tipos.BOOLEAN) return new Retorno(temp, true, symFunc.type);

                Retorno retorno = new Retorno("", false, symFunc.type);
                this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
                this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;
                generator.addIf(temp, "1", "==", this.trueLabel);
                generator.addGoto(this.falseLabel);
                retorno.trueLabel = this.trueLabel;
                retorno.falseLabel = this.falseLabel;
                return retorno;
            }
            else
            {

            }
            throw new Error("Semántico", "Funcion no implementada", ent.obtenerAmbito(), linea, columna);
        }

        public object compilar(Entorno ent, Errores errores)
        {
            try
            {
                this.compilar(ent);
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }
    }
}
