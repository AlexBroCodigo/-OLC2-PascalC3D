using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Instrucciones.Control;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Instrucciones.Functions
{
    class FunctionSt : Instruccion
    {
        public int linea { get; set; }
        public int columna { get; set; }

        public string id;
        public LinkedList<Param> parametros;
        public Tipo type;
        private LinkedList<Bloque> bloques;
        private bool preCompile;

        public FunctionSt(Tipo type, string id, LinkedList<Param> parametros, LinkedList<Bloque> bloques, int linea, int columna)
        {
            this.type = type;
            this.id = id;
            this.parametros = parametros;
            this.bloques = bloques;
            this.preCompile = true;
            this.linea = linea;
            this.columna = columna;
        }

        public object compilar(Entorno ent, Errores errores)
        {

            try
            {
                if (this.preCompile)
                {
                    this.preCompile = false;
                    this.validateParams(ent);
                    this.validateType(ent);
                    string uniqueId = this.uniqueId(ent);
                    if (!ent.addFunc(this, uniqueId)) throw new Error("Semántico", "Ya existe una funcion con el id: " + this.id, ent.obtenerAmbito(), linea, columna);
                    return null;
                }

                SimboloFunction symbolFunc = ent.getFunc(this.id);
                if(symbolFunc != null)
                {
                    Generator generator = Generator.getInstance();
                    Entorno newEnv;
                    if (type.tipo == Tipos.VOID) newEnv = new Entorno(ent, "PROCEDURE", this.id.ToLower());
                    else newEnv = new Entorno(ent, "FUNCTION", this.id.ToLower());
                    string returnLbl = generator.newLabel();
                    LinkedList<string> tempStorage = generator.getTempStorage();

                    newEnv.setEntornoFunc(this.id, symbolFunc, returnLbl);
                    foreach(Param param in this.parametros)
                    {
                        if (param.isRef)
                        {
                            newEnv.addVarRef(param.id, param.type, false, false, linea, columna,true);
                        } else
                        {
                            newEnv.addVar(param.id, param.type, false, false, linea, columna);
                        }
                    }
                    generator.clearTempStorage();
                    generator.isFunc = "\t";
                    generator.addBegin(symbolFunc.uniqueId);

                    /* Codigo extra para la recursividad de funciones */
                    foreach(Bloque bloque in bloques)
                    {
                        if(bloque == bloques.Last.Value) foreach (Instruccion ins in bloque.instrucciones) ins.compilar(newEnv, errores); //El cuerpo de las a
                        else
                        {
                            //Primera pasada: TODO
                            foreach (Instruccion element in bloque.instrucciones) element.compilar(newEnv, errores);
                            
                            //Segunda pasada: solo funciones
                            foreach (Instruccion element in bloque.instrucciones) if (element is FunctionSt) element.compilar(newEnv, errores);
                        }
                        
                    }
                    /* Aqui termina el codigo extra para la recursividad */
                    //foreach (Instruccion sentencia in sentencias) sentencia.compilar(newEnv, errores);
                    
                    generator.addLabel(returnLbl);
                    generator.addEnd();
                    generator.isFunc = "";
                    generator.setTempStorage(tempStorage);



                }
            } catch(Error ex)
            {
                errores.agregarError(ex);
            }
            return null;
        }

        private void validateParams(Entorno ent)
        {
            LinkedList<string> set = new LinkedList<string>();
            foreach(Param param in this.parametros)
            {
                if (set.Contains(param.id.ToLower())) throw new Error("Semántico", "Ya existe un parametro con el id: " + param.id, ent.obtenerAmbito(), linea, columna);
                if(param.type.tipo == Tipos.STRUCT)
                {
                    SimboloStruct @struct = ent.getStruct(param.type.tipoId);
                    if (@struct == null) throw new Error("Semántico", "No existe el struct: " + param.type.tipoId,ent.obtenerAmbito(),linea,columna);
                }
                set.AddLast(param.id.ToLower());
            }
        }


        private void validateType(Entorno ent)
        {
            if(this.type.tipo == Tipos.STRUCT)
            {
                SimboloStruct @struct = ent.getStruct(this.type.tipoId);
                if (@struct == null) throw new Error("Semántico", "No existe el struct: " + this.type.tipoId, ent.obtenerAmbito(), linea, columna);
            }
        }

        public string uniqueId(Entorno ent)
        {
            string id = this.id;//ent.prop + "_" + this.id;
            if (this.parametros.Count == 0) return id + "_empty";
            foreach(Param param in this.parametros)
            {
                id += "_" + param.getUnicType();
            }
            return id;
        }

    }
}
