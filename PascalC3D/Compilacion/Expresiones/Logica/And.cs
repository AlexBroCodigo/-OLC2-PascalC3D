using PascalC3D.Compilacion.Generador;
using PascalC3D.Compilacion.Interfaces;
using PascalC3D.Compilacion.TablaSimbolos;
using PascalC3D.Utils;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Compilacion.Expresiones.Logica
{
    class And : Expresion
    {
        public string trueLabel { get; set; }
        public string falseLabel { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }

        private Expresion left;
        private Expresion right;

        public And(Expresion left, Expresion right, int linea, int columna)
        {
            this.left = left;
            this.right = right;
            this.linea = linea;
            this.columna = columna;
            trueLabel = falseLabel = "";
        }


        public Retorno compilar(Entorno ent)
        {
            Generator generator = Generator.getInstance();
            this.trueLabel = this.trueLabel == "" ? generator.newLabel() : this.trueLabel;
            this.falseLabel = this.falseLabel == "" ? generator.newLabel() : this.falseLabel;

            this.left.trueLabel = generator.newLabel();
            this.right.trueLabel = this.trueLabel;
            this.left.falseLabel = this.right.falseLabel = this.falseLabel;

            generator.addComment("Inicia AND");
            Retorno left = this.left.compilar(ent);
            generator.addLabel(this.left.trueLabel);
            Retorno right = this.right.compilar(ent);
            generator.addComment("Finaliza AND");

            Tipos tipoResultado = TablaTipos.obtenerTipo("and", left.type, right.type);
            if (tipoResultado == Tipos.ERROR) throw new Error("Semántico", "No se puede evaluar un AND entre un " + left.type.tipoToString() + " y un " + right.type.tipoToString(), ent.obtenerAmbito(), linea, columna);
            Tipo tipo = new Tipo(tipoResultado); //INTEGER o BOOLEAN
            Retorno retorno = new Retorno("", false, tipo);
            retorno.trueLabel = this.trueLabel;
            retorno.falseLabel = this.right.falseLabel;
            return retorno;
        }   
    }
}
