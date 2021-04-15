using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Literal
{
    class NewStruct : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private string id;

        public NewStruct(string id, int linea, int columna)
        {
            this.id = id;
            this.columna = columna;
            this.linea = linea;
            trueLabel = falseLabel = "";
        }

        public Retorno compilar(Entorno ent)
        {
            SimboloStruct symStruct = ent.searchStruct(this.id);
            Generator generator = Generator.getInstance();
            if (symStruct == null) throw new Error("Semántico", "No existe el object " + this.id + " en este entorno",ent.obtenerAmbito(),linea,columna);
            string temp = generator.newTemporal();
            generator.addExpression(temp, "HP", "", "");
            //Llenamos de valores por defecto
            foreach(Param attribute in symStruct.attributes)
            {
                switch (attribute.type.tipo)
                {
                    case Tipos.INTEGER:
                    case Tipos.REAL:
                    case Tipos.BOOLEAN:
                        generator.addSetHeap("HP", "0");
                        generator.nextHeap();
                        break;
                    case Tipos.STRING:
                        generator.addSetHeap("HP", "-1");
                        generator.nextHeap();
                        break;
                    case Tipos.STRUCT:
                        generator.addComment("Inicia substruct");
                        compilarSubStruct(attribute);
                        generator.addComment("Finaliza substruct");
                        break;
                    default:
                        generator.addSetHeap("HP", "-1");
                        generator.nextHeap();
                        break;
                }
            }
            return new Retorno(temp, true, new Tipo(Tipos.STRUCT, symStruct.identifier, symStruct));
        }

        public void compilarSubStruct(Param actual)
        {
            Generator generator = Generator.getInstance();
            string temp = generator.newTemporal();
            generator.addExpression(temp, "HP","1", "+");
            generator.addSetHeap("HP",temp);
            generator.nextHeap();
            foreach(Param attribute in actual.type.symStruct.attributes)
            {
                switch (attribute.type.tipo)
                {
                    case Tipos.INTEGER:
                    case Tipos.REAL:
                    case Tipos.BOOLEAN:
                        generator.addSetHeap("HP", "0");
                        generator.nextHeap();
                        break;
                    case Tipos.STRING:
                        generator.addSetHeap("HP", "-1");
                        generator.nextHeap();
                        break;
                    case Tipos.STRUCT:
                        generator.addComment("Inicia substruct");
                        compilarSubStruct(attribute);
                        generator.addComment("Finaliza substruct");
                        break;
                    default:
                        generator.addSetHeap("HP", "-1");
                        generator.nextHeap();
                        break;
                }
            }
        }
    }
}
