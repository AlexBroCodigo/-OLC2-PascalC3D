using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Analizador
{
    class Gramatica : Grammar
    {
        public Gramatica() : base(caseSensitive: false)
        {
            #region ER
            //Comentarios
            CommentTerminal comentlinea = new CommentTerminal("comentlinea", "//", "\n", "\r\n");
            CommentTerminal comentmulti = new CommentTerminal("comentmulti", "{", "}");
            CommentTerminal comentmulti2 = new CommentTerminal("comentmulti2", "(*", "*)");

            //Tipos de datos primitivos
            NumberLiteral numero = new NumberLiteral("numero");
            StringLiteral cadena = new StringLiteral("cadena", "'");
            RegexBasedTerminal booleano = new RegexBasedTerminal("booleano", "true|false");

            IdentifierTerminal id = new IdentifierTerminal("ID");
            #endregion

            #region Terminales
            var menor = ToTerm("<");
            var mayor = ToTerm(">");
            var menorigual = ToTerm("<=");
            var mayorigual = ToTerm(">=");
            var igual = ToTerm("=");
            var desigual = ToTerm("<>");
            var mas = ToTerm("+");
            var menos = ToTerm("-");
            var division = ToTerm("/");
            var mult = ToTerm("*");
            var mod = ToTerm("mod");
            var pt = ToTerm(".");
            var ptcoma = ToTerm(";");
            var parAbre = ToTerm("(");
            var parCierre = ToTerm(")");
            var corAbre = ToTerm("[");
            var corCierre = ToTerm("]");
            var dospt = ToTerm(":");
            var asig = ToTerm(":=");
            var coma = ToTerm(",");


            //Palabras reservadas
            var robject = ToTerm("object");
            var array = ToTerm("array");
            var integer = ToTerm("integer");
            var rbreak = ToTerm("break");
            var rcontinue = ToTerm("continue");
            var real = ToTerm("real");
            var write = ToTerm("write");
            var rstring = ToTerm("string");
            var rboolean = ToTerm("boolean");
            var writeln = ToTerm("writeln");
            var and = ToTerm("and");
            var or = ToTerm("or");
            var not = ToTerm("not");
            var type = ToTerm("type");
            var rvar = ToTerm("var");
            var begin = ToTerm("begin");
            var end = ToTerm("end");
            var rif = ToTerm("if");
            var then = ToTerm("then");
            var relse = ToTerm("else");
            var rcase = ToTerm("case");
            var of = ToTerm("of");
            var rwhile = ToTerm("while");
            var repeat = ToTerm("repeat");
            var until = ToTerm("until");
            var rfor = ToTerm("for");
            var rdo = ToTerm("do");
            var to = ToTerm("to");
            var downto = ToTerm("downto");
            var program = ToTerm("program");
            var exit = ToTerm("exit");
            var rconst = ToTerm("const");
            var procedure = ToTerm("procedure");
            var function = ToTerm("function");
            var graficar = ToTerm("graficar_ts");

            RegisterOperators(1, igual, desigual, mayor, menor, mayorigual, menorigual);
            RegisterOperators(2, mas, menos, or);
            RegisterOperators(3, mult, division, mod, and);
            RegisterOperators(4, Associativity.Right, not);

            NonGrammarTerminals.Add(comentlinea);
            NonGrammarTerminals.Add(comentmulti);
            NonGrammarTerminals.Add(comentmulti2);
            #endregion

            #region No Terminales
            NonTerminal
                S = new NonTerminal("S"),
                P = new NonTerminal("P"),
                L_AC = new NonTerminal("L_AC"),
                AC = new NonTerminal("AC"),
                L_CNT = new NonTerminal("L_CNT"),
                L_PROF = new NonTerminal("L_PROF"),
                MAIN = new NonTerminal("MAIN"),
                G_CNT = new NonTerminal("G_CNT"),
                G_TY = new NonTerminal("G_TY"),
                DECLAS = new NonTerminal("DECLAS"),
                L_TY = new NonTerminal("L_TY"),
                L_VR = new NonTerminal("L_VR"),
                CNT = new NonTerminal("CNT"),
                TY = new NonTerminal("TY"),
                VR = new NonTerminal("VR"),
                TYP = new NonTerminal("TYP"),
                OBJ = new NonTerminal("OBJ"),
                ARY = new NonTerminal("ARY"),
                L_DEF = new NonTerminal("L_DEF"),
                ASIG = new NonTerminal("ASIG"),
                ASID = new NonTerminal("ASID"),
                L_ID = new NonTerminal("L_ID"),
                TIPO = new NonTerminal("TIPO"),
                ZTIPO = new NonTerminal("ZTIPO"),
                IT = new NonTerminal("IT"),
                PROF = new NonTerminal("PROF"),
                DEF = new NonTerminal("DEF"),
                PRO = new NonTerminal("PRO"),
                BEG = new NonTerminal("BEG"),
                L_PARAM = new NonTerminal("L_PARAM"),
                PARAM = new NonTerminal("PARAM"),
                FUN = new NonTerminal("FUN"),
                SPACE = new NonTerminal("SPACE"),
                L_EXP = new NonTerminal("L_EXP"),
                CALL = new NonTerminal("CALL"),
                EXPLOG = new NonTerminal("EXPLOG"),
                EXPRELA = new NonTerminal("EXPRELA"),
                EXPNUMERICA = new NonTerminal("EXPNUMERICA"),
                L_SENCU = new NonTerminal("L_SENCU"),
                SENCU = new NonTerminal("SENCU"),
                SEN = new NonTerminal("SEN"),
                COT = new NonTerminal("COT"),
                BRK = new NonTerminal("BRK"),
                IF = new NonTerminal("IF"),
                ELSE = new NonTerminal("ELSE"),
                L_SEN = new NonTerminal("L_SEN"),
                FAD = new NonTerminal("FAD"),
                CASE = new NonTerminal("CASE"),
                L_OPC = new NonTerminal("L_OPC"),
                LETC = new NonTerminal("LETC"),
                OPC = new NonTerminal("OPC"),
                ETC = new NonTerminal("ETC"),
                REP = new NonTerminal("REP"),
                WH = new NonTerminal("WH"),
                FOR = new NonTerminal("FOR"),
                WRT = new NonTerminal("WRT"),
                EXT = new NonTerminal("EXT"),
                GTS = new NonTerminal("GTS"),
                PC = new NonTerminal("PC"),
                L_AT = new NonTerminal("L_AT"),
                AT = new NonTerminal("AT"),
                ACCESS = new NonTerminal("ACCESS"),
                ACCID = new NonTerminal("ACCID");

            #endregion

            #region Gramatica
            S.Rule = P;

            P.Rule = program + id + ptcoma + L_AC + MAIN
                   | program + id + ptcoma + MAIN;

            P.ErrorRule = SyntaxError + ptcoma; //ERROR: Por si viniera un error ahí, ya no es para ejecutar recuerda

            L_AC.Rule = MakePlusRule(L_AC, AC);

            AC.Rule = G_CNT
                    | G_TY
                    | DECLAS
                    | L_PROF;

            AC.ErrorRule = SyntaxError + ptcoma; //ERROR: por si no se recupero anteriormente

            G_CNT.Rule = rconst + L_CNT;

            L_CNT.Rule = MakePlusRule(L_CNT, CNT);

            CNT.Rule = id + igual + EXPLOG + ptcoma;

            CNT.ErrorRule = SyntaxError + ptcoma; //ERROR

            L_PROF.Rule = MakePlusRule(L_PROF, PROF);

            G_TY.Rule = type + L_TY;

            DECLAS.Rule = rvar + L_VR;

            L_TY.Rule = MakePlusRule(L_TY, TY);

            L_VR.Rule = MakePlusRule(L_VR, VR);

            TY.Rule = id + igual + TYP + ptcoma;

            TY.ErrorRule = SyntaxError + ptcoma; //ERROR

            VR.Rule = L_ID + dospt + ZTIPO + ptcoma
                    | L_ID + dospt + ZTIPO + igual + EXPLOG + ptcoma;

            VR.ErrorRule = SyntaxError + ptcoma; //ERROR

            L_ID.Rule = MakePlusRule(L_ID, coma, id);

            TYP.Rule = OBJ
                      | ARY;

            OBJ.Rule = robject + rvar + L_AT + end;

            L_AT.Rule = MakePlusRule(L_AT, AT);

            AT.Rule = L_ID + dospt + ZTIPO + ptcoma;

            ARY.Rule = array + corAbre + IT + corCierre + of + ZTIPO;

            MAIN.Rule = begin + L_SEN + end + pt;

            L_DEF.Rule = MakePlusRule(L_DEF, DEF);

            ASIG.Rule = ASID + asig + EXPLOG + PC;

            ASID.Rule = ASID + pt + id
                      | id;

            TIPO.Rule = integer
                      | rstring
                      | rboolean
                      | real;


            ZTIPO.Rule = TIPO
                       | id;

            IT.Rule = integer
                    | rboolean;

            PROF.Rule = PRO
                      | FUN;

            DEF.Rule = DECLAS
                     | L_PROF;

            PRO.Rule = procedure + id + parAbre + L_PARAM + parCierre + ptcoma + SPACE
                     | procedure + id + parAbre + parCierre + ptcoma + SPACE
                     | procedure + id + ptcoma + SPACE;

            BEG.Rule = begin + L_SEN + end + PC;

            L_PARAM.Rule = MakePlusRule(L_PARAM, ptcoma, PARAM);

            PARAM.Rule = rvar + L_ID + dospt + ZTIPO
                       | L_ID + dospt + ZTIPO;

            FUN.Rule = function + id + parAbre + L_PARAM + parCierre + dospt + ZTIPO + ptcoma + SPACE
                     | function + id + parAbre + parCierre + dospt + ZTIPO + ptcoma + SPACE
                     | function + id + dospt + ZTIPO + ptcoma + SPACE;

            SPACE.Rule = L_DEF + BEG
                       | BEG;

            L_EXP.Rule = MakePlusRule(L_EXP, coma, EXPLOG);

            CALL.Rule = id + parAbre + L_EXP + parCierre + PC
                      | id + parAbre + parCierre + PC;


            EXPLOG.Rule = EXPLOG + and + EXPLOG
                        | EXPLOG + or + EXPLOG
                        | not + EXPLOG                  //UNARIO
                        | parAbre + EXPLOG + parCierre
                        | EXPRELA;

            EXPRELA.Rule = EXPRELA + mayor + EXPRELA
                         | EXPRELA + menor + EXPRELA
                         | EXPRELA + mayorigual + EXPRELA
                         | EXPRELA + menorigual + EXPRELA
                         | EXPRELA + igual + EXPRELA
                         | EXPRELA + desigual + EXPRELA
                         | parAbre + EXPRELA + parCierre
                         | booleano
                         | EXPNUMERICA;

            EXPNUMERICA.Rule = EXPNUMERICA + mas + EXPNUMERICA
                             | EXPNUMERICA + menos + EXPNUMERICA
                             | EXPNUMERICA + mult + EXPNUMERICA
                             | EXPNUMERICA + division + EXPNUMERICA
                             | EXPNUMERICA + mod + EXPNUMERICA
                             | parAbre + EXPNUMERICA + parCierre
                             | menos + EXPNUMERICA   //UNARIO
                             | numero
                             | cadena
                             | ACCESS;

            ACCESS.Rule = ACCID
                        | CALL;

            ACCID.Rule = ACCID + pt + id
                      | id;

            L_SENCU.Rule = MakePlusRule(L_SENCU, SENCU);

            SENCU.Rule = SEN
                       | BEG;

            SEN.Rule = ASIG
                     | IF
                     | CASE
                     | WH
                     | REP
                     | FOR
                     | BRK
                     | COT
                     | WRT
                     | EXT
                     | GTS
                     | CALL;

            SEN.ErrorRule = SyntaxError + ptcoma
                          | SyntaxError + end; //ERROR


            COT.Rule = rcontinue + PC;

            BRK.Rule = rbreak + PC;

            IF.Rule = rif + EXPLOG + then + SEN
                    | rif + EXPLOG + then + SEN + ELSE
                    | rif + EXPLOG + then + BEG
                    | rif + EXPLOG + then + begin + L_SEN + end + ELSE;

            L_SEN.Rule = MakePlusRule(L_SEN, SEN);
            
            ELSE.Rule = relse + SEN
                      | relse + BEG;

            FAD.Rule = to
                     | downto;

            CASE.Rule = rcase + parAbre + id + parCierre + of + L_OPC + end + ptcoma
                      | rcase + parAbre + id + parCierre + of + L_OPC + ELSE + end + ptcoma
                      | rcase + id + of + L_OPC + end + ptcoma
                      | rcase + id + of + L_OPC + ELSE + end + ptcoma;

            L_OPC.Rule = MakePlusRule(L_OPC, OPC);

            LETC.Rule = MakePlusRule(LETC, coma, ETC);

            OPC.Rule = LETC + dospt + SENCU;

            ETC.Rule = menos + numero
                     | numero   //con negativo
                     | booleano
                     | cadena;

            REP.Rule = repeat + L_SENCU + until + EXPLOG + PC;

            WH.Rule = rwhile + EXPLOG + rdo + SENCU;

            FOR.Rule = rfor + id + asig + EXPNUMERICA + FAD + EXPNUMERICA + rdo + SENCU;

            WRT.Rule = writeln + parAbre + L_EXP + parCierre + PC
                     | write + parAbre + L_EXP + parCierre + PC;

            EXT.Rule = exit + parAbre + EXPLOG + parCierre + PC
                     | exit + parAbre + parCierre + PC;

            GTS.Rule = graficar + parAbre + parCierre + PC;

            PC.Rule = ptcoma
                     | Empty;

            #endregion

            #region Preferencias
            this.Root = S;
            #endregion
        }
    }
}
