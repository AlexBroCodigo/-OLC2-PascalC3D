using System;
using System.Collections.Generic;
using System.Text;
using static PascalC3D.Utils.Tipo;

namespace PascalC3D.Utils
{
    class TablaTipos
    {

        public static Tipos[,] suma = new Tipos[4, 4] { {Tipos.INTEGER,Tipos.REAL,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.REAL,Tipos.REAL,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.STRING, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR}};
        public static Tipos[,] restamultdiv = new Tipos[4, 4] { {Tipos.INTEGER,Tipos.REAL,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.REAL,Tipos.REAL,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR}};

        public static Tipos[,] modulo = new Tipos[4, 4] { {Tipos.INTEGER,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR}};

        public static Tipos[,] relacional = new Tipos[4, 4] {{Tipos.BOOLEAN,Tipos.BOOLEAN,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.BOOLEAN,Tipos.BOOLEAN,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.BOOLEAN, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.BOOLEAN}};

        public static Tipos[,] andOr = new Tipos[4, 4] {{Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.ERROR},
                                                         {Tipos.ERROR,Tipos.ERROR,Tipos.ERROR, Tipos.BOOLEAN}};

        public static Tipos obtenerTipo(string tipoOperacion, Tipo izquierda, Tipo derecha)
        {
            try
            {
                switch (tipoOperacion)
                {
                    case "and":
                    case "or":
                        return andOr[(int)izquierda.tipo, (int)derecha.tipo];
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                    case "=":
                    case "<>":
                        return relacional[(int)izquierda.tipo, (int)derecha.tipo];
                    case "+": return suma[(int)izquierda.tipo, (int)derecha.tipo];
                    case "-":
                    case "*":
                    case "/":
                        return restamultdiv[(int)izquierda.tipo, (int)derecha.tipo];
                    default: //MODULO
                        return modulo[(int)izquierda.tipo, (int)derecha.tipo];
                }
            }
            catch (Exception)
            {
                return Tipos.ERROR;
            }
        }

    }
}
