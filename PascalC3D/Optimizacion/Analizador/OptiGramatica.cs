using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Optimizacion.Analizador
{
    class OptiGramatica : Grammar
    {
        public OptiGramatica() : base(caseSensitive: false)
        {
            #region ER
            //Comentarios
            CommentTerminal comentlinea = new CommentTerminal("comentlinea", "//", "\n", "\r\n");
            CommentTerminal comentmulti = new CommentTerminal("comentmulti", "/*", "*/");

            //Tipos de datos primitivos
            NumberLiteral numero = new NumberLiteral("numero");
            StringLiteral cadena = new StringLiteral("cadena", "\"");

            RegexBasedTerminal temporal = new RegexBasedTerminal("temporal", "T[0-9]+");
            temporal.Priority = TerminalPriority.High;
            RegexBasedTerminal label = new RegexBasedTerminal("label", "L[0-9]+");
            label.Priority = TerminalPriority.High;
            IdentifierTerminal id = new IdentifierTerminal("ID");
            id.Priority = TerminalPriority.Low;
            #endregion

            #region Terminales
            var numeral = ToTerm("#");
            var menorQue = ToTerm("<");
            var mayorQue = ToTerm(">");
            var ptComa = ToTerm(";");
            var coma = ToTerm(",");
            var parAbre = ToTerm("(");
            var parCierre = ToTerm(")");
            var llaveAbre = ToTerm("{");
            var llaveCierre = ToTerm("}");
            var igual = ToTerm("=");
            var dospt = ToTerm(":");
            var menos = ToTerm("-");
            var corAbre = ToTerm("[");
            var corCierre = ToTerm("]");
            var mas = ToTerm("+");
            var por = ToTerm("*");
            var div = ToTerm("/");
            var mod = ToTerm("%");
            var diferente = ToTerm("!=");
            var mayorIgual = ToTerm(">=");
            var menorIgual = ToTerm("<=");
            var comparacion = ToTerm("==");
            var pt = ToTerm(".");


            //Palabras reservadas
            var include = ToTerm("include");
            var stdio = ToTerm("stdio");
            var rfloat = ToTerm("float");
            var rvoid = ToTerm("void");
            var rint = ToTerm("int");
            var rif = ToTerm("if");
            var rgoto = ToTerm("goto");
            var printf = ToTerm("printf");
            var Heap = ToTerm("Heap");
            var Stack = ToTerm("Stack");
            var SP = ToTerm("SP");
            var HP = ToTerm("HP");
            var rreturn = ToTerm("return");
            var h = ToTerm("h");

            RegisterOperators(1, comparacion, diferente, mayorQue, menorQue, mayorIgual, menorIgual);
            RegisterOperators(2, mas, menos);
            RegisterOperators(3, por, div, mod);

            NonGrammarTerminals.Add(comentlinea);
            NonGrammarTerminals.Add(comentmulti);
            #endregion

            #region No Terminales
            NonTerminal
                S = new NonTerminal("S"),
                COD = new NonTerminal("COD"),
                HEAD = new NonTerminal("HEAD"),
                L_VR = new NonTerminal("L_VR"),
                VR = new NonTerminal("VR"),
                G_TMP = new NonTerminal("G_TMP"),
                L_TMP = new NonTerminal("l_TMP"),
                L_FUN = new NonTerminal("L_FUN"),
                FUN = new NonTerminal("FUN"),
                L_SEN = new NonTerminal("L_SEN"),
                SEN = new NonTerminal("SEN"),
                RET = new NonTerminal("RET"),
                CALL = new NonTerminal("CALL"),
                GO = new NonTerminal("GO"),
                ET = new NonTerminal("ET"),
                PRT = new NonTerminal("PRT"),
                VALP = new NonTerminal("VALP"),
                EXP = new NonTerminal("EXP"),
                IF = new NonTerminal("IF"),
                INDEX = new NonTerminal("INDEX"),
                COND = new NonTerminal("COND"),
                VALI = new NonTerminal("VALI"),
                RELA = new NonTerminal("RELA"),
                ASIG = new NonTerminal("ASIG"),
                TG = new NonTerminal("TG"),
                VALO = new NonTerminal("VALO"),
                ARI = new NonTerminal("ARI"),
                EXPNUM = new NonTerminal("EXPNUM"),
                PUN = new NonTerminal("PUN"),
                PRIMI = new NonTerminal("PRIMI"),
                TEMP = new NonTerminal("TEMP"),
                STR = new NonTerminal("STR"),
                L_ET = new NonTerminal("L_ET");
            #endregion

            #region Gramatica
            S.Rule = COD;

            COD.Rule = HEAD + L_FUN;

            HEAD.Rule = numeral + include + menorQue + stdio + pt + h + mayorQue + L_VR + G_TMP;

            HEAD.ErrorRule = SyntaxError + mayorQue; //ERROR

            L_VR.Rule = L_VR + VR
                      | VR;

            VR.Rule = rfloat + Heap + corAbre + numero + corCierre + ptComa
                    | rfloat + Stack + corAbre + numero + corCierre + ptComa
                    | rint + SP + ptComa
                    | rint + HP + ptComa;

            VR.ErrorRule = SyntaxError + ptComa; //ERROR


            G_TMP.Rule = rfloat + L_TMP + ptComa;

            G_TMP.ErrorRule = SyntaxError + ptComa; //ERROR

            L_TMP.Rule = MakePlusRule(L_TMP, coma, temporal);

            L_FUN.Rule = MakePlusRule(L_FUN, FUN);

            FUN.Rule = rvoid + id + parAbre + parCierre + llaveAbre + L_SEN + L_ET +  llaveCierre
                     | rvoid + id + parAbre + parCierre + llaveAbre + L_ET + llaveCierre
                     | rvoid + id + parAbre + parCierre + llaveAbre + L_SEN + llaveCierre;

            FUN.ErrorRule = SyntaxError + llaveCierre; //ERROR

            L_ET.Rule = MakePlusRule(L_ET, ET);

            ET.Rule = label + dospt + L_SEN
                    | label + dospt;

            L_SEN.Rule = MakePlusRule(L_SEN, SEN);

            SEN.Rule = ASIG
                     | IF
                     | GO
                     | PRT
                     | RET
                     | CALL;

            SEN.ErrorRule = SyntaxError + ptComa; //ERROR

            RET.Rule = rreturn + ptComa;

            CALL.Rule = id + parAbre + parCierre + ptComa;

            GO.Rule = rgoto + label + ptComa;

            PRT.Rule = printf + parAbre + cadena + coma + VALP + parCierre + ptComa;

            VALP.Rule = temporal
                      | parAbre + rint + parCierre + temporal
                      | numero
                      | menos + numero;

            IF.Rule = rif + parAbre + COND + parCierre + rgoto + label + ptComa;

            COND.Rule = VALI + RELA + VALI;

            VALI.Rule = TEMP
                      | PRIMI;

            RELA.Rule = comparacion
                      | diferente
                      | mayorIgual
                      | menorIgual
                      | mayorQue
                      | menorQue;

            ASIG.Rule = TG + igual + EXP + ptComa;

            TG.Rule = temporal
                    | SP
                    | HP
                    | Stack + corAbre + INDEX + corCierre
                    | Heap + corAbre + INDEX + corCierre;

            INDEX.Rule = SP
                       | HP
                       | parAbre + rint + parCierre + temporal
                       | numero;

            EXP.Rule = EXPNUM
                     | VALO;

            EXPNUM.Rule = VALO + ARI + VALO;

            ARI.Rule = mas
                     | menos
                     | por
                     | div
                     | mod;

            VALO.Rule = PUN
                      | PRIMI
                      | TEMP
                      | STR;

            PUN.Rule = SP
                     | HP;

            PRIMI.Rule = numero
                       | menos + numero;

            TEMP.Rule = temporal;

            STR.Rule = Stack + corAbre + INDEX + corCierre
                      | Heap + corAbre + INDEX + corCierre;

            #endregion


            #region Preferencias
            this.Root = S;
            #endregion
        }
    }
}
