using System;
using System.Collections.Generic;
using System.Text;

namespace PascalC3D.Compilacion.Generador
{
    class Generator
    {
        private static Generator generator;
        private int temporal;
        private int label;
        private LinkedList<string> code;
        private LinkedList<string> tempStorage;
        public string isFunc;

        public Generator()
        {
            temporal = 4;
            label = 0;
            code = new LinkedList<string>();
            isFunc = "";
            tempStorage = new LinkedList<string>();
        }

        public static Generator getInstance()
        {
            if (generator is null) generator = new Generator(); 
            return generator;
        }

        public void clearTempStorage()
        {
            tempStorage.Clear();
        }

        public void clearCode()
        {
            temporal = 4;
            label = 0;
            code = new LinkedList<string>();
            tempStorage = new LinkedList<string>();
            isFunc = "";
        }

        public void addCode(string code)
        {
            this.code.AddLast(isFunc + code);
        }

        public string getCode()
        {
            return String.Join("\n",code);
        }

        public string getEncabezado()
        {
            string head = "#include <stdio.h>\n\nfloat Heap[100000];\nfloat Stack[100000];\n\nint SP;\nint HP;\n\n";
            if (temporal != 0) head += "float ";
            for (int i = 0; i < temporal; i++)
            {
                if (i + 1 == temporal) head += "T" + i + ";\n\n";
                else
                {
                    head += "T" + i + ",";
                    if (i % 20 == 0) head += "\n";
                }
            }
            return head;
        }

        public string getFuncionesNativas()
        {
            string cadfunciones = native_print_str();
            cadfunciones += native_concat_str();
            cadfunciones += native_compare_str();
            cadfunciones += native_less_str();
            cadfunciones += native_lessEq_str();
            cadfunciones += native_greater_str();
            cadfunciones += native_greaterEq_str();
            return cadfunciones;
        }

        public string native_print_str()
        {
            string str = "void native_print_str(){\n\tT0 = Stack[SP];\n\tT1 = Heap[(int)T0];\n\t";
            str += "L0:\n\t\tif(T1==-1) goto L1;\n\t\tprintf(\"%c\",(int)T1);\n\t\tT0 = T0 + 1;\n\t\tT1 = Heap[(int)T0];\n\t\tgoto L0;\n\tL1:\n\t\treturn;\n}\n\n";
            return str;
        }

        public string native_concat_str()
        {
            string concat = "void native_concat_str(){\n\tStack[SP] = HP;\n\tT0 = Stack[SP+1];\n\tT1 = Heap[(int)T0];\n\tL0:\n\t\t";
            concat += "if(T1==-1) goto L1;\n\t\tHeap[HP] = T1;\n\t\tHP = HP + 1;\n\t\tT0 = T0 + 1;\n\t\tT1 = Heap[(int)T0];\n\t\tgoto L0;\n\t";
            concat += "L1:\n\t\tT0 = Stack[SP+2];\n\t\tT1 = Heap[(int)T0];\n\tL2:\n\t\t";
            concat += "if(T1==-1) goto L3;\n\t\tHeap[HP] = T1;\n\t\tHP = HP + 1;\n\t\tT0 = T0 + 1;\n\t\tT1 = Heap[(int)T0];\n\t\tgoto L2;\n\tL3:\n\t\t";
            concat += "Heap[HP] = -1;\n\t\tHP = HP + 1;\n}\n\n";
            return concat;
        }

        public string native_compare_str()
        {
            string compare = "void native_compare_str(){\n\tStack[SP] = 0;\n\tT0 = Stack[SP + 1];\n\tT1 = Stack[SP + 2];\n\tT2 = Heap[(int)T0];\n\tT3 = Heap[(int)T1];\n\tL0:\n\t\t";
            compare += "if(T2==-1) goto L3;\n\t\tif(T3==-1) goto L3;\n\tL1:\n\t\tif(T2==T3) goto L2;\n\t\tgoto L4;\n\tL2:\n\t\tT0 = T0 + 1;\n\t\tT1 = T1 + 1;\n\t\tT2 = Heap[(int)T0];\n\t\t";
            compare += "T3 = Heap[(int)T1];\n\t\tgoto L0;\n\tL3:\n\t\tif(T2!=T3) goto L4;\n\t\tStack[SP] = 1;\n\tL4:\n\t\treturn;\n}\n\n";
            return compare;
        }

        public string native_less_str()
        {
            string less = "void native_less_str(){\n\tStack[SP] = 0;\n\tT0 = Stack[SP + 1];\n\tT1 = Stack[SP + 2];\n\tT2 = Heap[(int)T0];\n\tT3 = Heap[(int)T1];\n\tL0:\n\t\t";
            less += "if(T2==-1) goto L3;\n\t\tif(T3==-1) goto L3;\n\tL1:\n\t\tif(T2==T3) goto L2;\n\t\tgoto L3;\n\tL2:\n\t\tT0 = T0 + 1;\n\t\tT1 = T1 + 1;\n\t\tT2 = Heap[(int)T0];\n\t\t";
            less += "T3 = Heap[(int)T1];\n\t\tgoto L0;\n\tL3:\n\t\tif(T2>=T3) goto L4;\n\t\tStack[SP] = 1;\n\tL4:\n\t\treturn;\n}\n\n";
            return less;
        }

        public string native_lessEq_str()
        {
            string less = "void native_lessEq_str(){\n\tStack[SP] = 0;\n\tT0 = Stack[SP + 1];\n\tT1 = Stack[SP + 2];\n\tT2 = Heap[(int)T0];\n\tT3 = Heap[(int)T1];\n\tL0:\n\t\t";
            less += "if(T2==-1) goto L3;\n\t\tif(T3==-1) goto L3;\n\tL1:\n\t\tif(T2==T3) goto L2;\n\t\tgoto L3;\n\tL2:\n\t\tT0 = T0 + 1;\n\t\tT1 = T1 + 1;\n\t\tT2 = Heap[(int)T0];\n\t\t";
            less += "T3 = Heap[(int)T1];\n\t\tgoto L0;\n\tL3:\n\t\tif(T2>T3) goto L4;\n\t\tStack[SP] = 1;\n\tL4:\n\t\treturn;\n}\n\n";
            return less;
        }

        public string native_greater_str()
        {
            string greater = "void native_greater_str(){\n\tStack[SP] = 0;\n\tT0 = Stack[SP + 1];\n\tT1 = Stack[SP + 2];\n\tT2 = Heap[(int)T0];\n\tT3 = Heap[(int)T1];\n\tL0:\n\t\t";
            greater += "if(T2==-1) goto L3;\n\t\tif(T3==-1) goto L3;\n\tL1:\n\t\tif(T2==T3) goto L2;\n\t\tgoto L3;\n\tL2:\n\t\tT0 = T0 + 1;\n\t\tT1 = T1 + 1;\n\t\tT2 = Heap[(int)T0];\n\t\t";
            greater += "T3 = Heap[(int)T1];\n\t\tgoto L0;\n\tL3:\n\t\tif(T2<=T3) goto L4;\n\t\tStack[SP] = 1;\n\tL4:\n\t\treturn;\n}\n\n";
            return greater;
        }

        public string native_greaterEq_str()
        {
            string greater = "void native_greaterEq_str(){\n\tStack[SP] = 0;\n\tT0 = Stack[SP + 1];\n\tT1 = Stack[SP + 2];\n\tT2 = Heap[(int)T0];\n\tT3 = Heap[(int)T1];\n\tL0:\n\t\t";
            greater += "if(T2==-1) goto L3;\n\t\tif(T3==-1) goto L3;\n\tL1:\n\t\tif(T2==T3) goto L2;\n\t\tgoto L3;\n\tL2:\n\t\tT0 = T0 + 1;\n\t\tT1 = T1 + 1;\n\t\tT2 = Heap[(int)T0];\n\t\t";
            greater += "T3 = Heap[(int)T1];\n\t\tgoto L0;\n\tL3:\n\t\tif(T2<T3) goto L4;\n\t\tStack[SP] = 1;\n\tL4:\n\t\treturn;\n}\n\n";
            return greater;
        }
        
        public string newTemporal()
        {
            string temp = "T" + temporal++;
            tempStorage.AddLast(temp);
            return temp;
        }


        public string newLabel()
        {
            return "L" + label++;
        }

        public void addLabel(string label)
        {
            code.AddLast(isFunc + label + ":");
        }

        public void addExpression(string target,string left,string right = "",string operador = "")
        {
            code.AddLast(isFunc + target + " = " + left + " " + operador + " " + right + ";");
        }

        public void addGoto(string label)
        {
            code.AddLast(isFunc + "goto " + label + ";");
        }

        public void addIf(string left,string right, string operador, string label)
        {
            code.AddLast(isFunc + "if (" + left + " " + operador + " " + right + ") goto " + label + ";");
        }

        public void nextHeap()
        {
            code.AddLast(isFunc + "HP = HP + 1;");
        }

        public void addSetHeap(string index,string value)
        {
            if (index.Contains("T")) code.AddLast(isFunc + "Heap[(int)" + index + "] = " + value + ";");
            else code.AddLast(isFunc + "Heap[" + index + "] = " + value + ";");
        }

        public void addGetStack(string target,string index)
        {
            if(index.Contains("T")) code.AddLast(isFunc + target + " = Stack[(int)" + index + "];");
            else code.AddLast(isFunc + target + " = Stack[" + index + "];");
        }

        public void addSetStack(string index,string value)
        {
            if(index.Contains("T")) code.AddLast(isFunc + "Stack[(int)" + index + "] = " + value + ";");
            else code.AddLast(isFunc + "Stack[" + index + "] = " + value + ";");
        }

        public void addNextEnv(int size)
        {
            code.AddLast(isFunc + "SP = SP + " + size + ";");
        }

        public void addAntEnv(int size)
        {
            code.AddLast(isFunc + "SP = SP - " + size + ";");
        }

        public void addCall(string id)
        {
            code.AddLast(isFunc + id + "();");
        }

        public void addPrint(string formato, string value)
        {
            code.AddLast(isFunc + "printf(\"%" + formato + "\"," + value + ");");
        }

        public void addPrintTrue()
        {
            addPrint("c", "84"); //T
            addPrint("c", "82"); //R
            addPrint("c", "85"); //U
            addPrint("c", "69"); //E   
        }

        public void addPrintFalse()
        {
            addPrint("c", "70"); //F
            addPrint("c", "65"); //A
            addPrint("c", "76"); //L
            addPrint("c", "83"); //S
            addPrint("c", "69"); //E
        }

        public void addComment(string comment)
        {
            code.AddLast(isFunc + "/***** " + comment + " *****/");
        }

        public void freeTemp(string temp)
        {
            if (tempStorage.Contains(temp)) tempStorage.Remove(temp);
        }

        public string getOpenMain()
        {
            string main = "void main(){\n";
            return main;
        }

        public string getCloseMain()
        {
            string main = "\n" + isFunc + "return;\n}";
            return main;
        }




    }
}
