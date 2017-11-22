using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent base fonts available
    /// </summary>
    public class BaseFonts
    {
        static BaseFonts()
        {
            InitPremia();
        }

        private static void InitPremia()
        {
            premia = new Font("premia");
            premia.SetChar(' ' , new bool[1 , 2]);
            premia.SetChar('\t' , new bool[1 , 32]);
            premia.SetChar('a' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false,true,true,true,false },
                {false,false,false,false,true },
                {false,true,true,true,true },
                {true,false,false,false,true },
                {true,false,false,false,true  },
                {false,true,true,true,true } });

            premia.SetChar('b' , new bool[8 , 5] {
                {true,false,false,false,false },
                 {true,false,false,false,false },
                {true,false,false,false,false },
                 {true,true,true,true,false },
                 {true,false,false,false,true },
                 {true,false,false,false,true },
                {true,false,false,false,true },
                 {true,true,true,true,false } });

            premia.SetChar('c' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , true },
                {false , true , true , true , false } });

            premia.SetChar('d' , new bool[8 , 5] {
                {false , false , false , false , true },
                {false,false,false,false,true },
                {false , false , false , false , true },
                {false , true , true , true , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , true } });
            premia.SetChar('e' ,
            StringToChar(
                "     " ,
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1    " ,
                " 1111"));
            premia.SetChar('f' , new bool[8 , 3] {
                {false , false , false },
                {false,true,true },
                {true , false , false  },
                {true , true , true  },
                {true , false , false  },
                {true , false , false  },
                {true , false , false  },
                {true , false , false  } });
            premia.SetChar('g' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , true },
                {false , false , false , false , true },
                {false , true , true , true , false } });
            premia.SetChar('h' , new bool[8 , 4] {
                {true , false,false , false  },
                {true,false,false,false },
                {true , false,false , false  },
                {true , true,true , false  },
                {true , false,false , true },
                {true , false,false , true },
                {true , false,false , true },
                {true , false,false , true } });
            premia.SetChar('i' , new bool[8 , 1] {
                {false  },
                {true },
                {false  },
                {true  },
                {true  },
                {true  },
                {true  },
                {true  } });
            premia.SetChar('j' , new bool[8 , 4] {
                {false , false , false , true },
                {false,false,false,false },
                {false , false , true , true },
                {false , false , false , true  },
                {false , false , false , true  },
                {false , false , false , true  },
                {true , false , false , true  },
                {false , true , true , false  } });
            premia.SetChar('k' , new bool[8 , 4] {
                {true , false , false , false },
                {true,false,false,false },
                {true , false , false , true  },
                {true , false , true , false  },
                {true , true , false , false  },
                {true , false , true , false  },
                {true , false , false , true  },
                {true , false , false , true  } });
            premia.SetChar('l' , new bool[8 , 2] {
                {true , true  },
                {false,true},
                {false , true  },
                {false , true  },
                {false , true  },
                {false , true },
                {false , true  },
                {false , true  } });
            premia.SetChar('m' , new bool[8 , 7] {
                {false , false , false , false , false,false,false },
                {false , false , false , false , false,false,false },
                {false , true , true , false , true,true,false },
                {true , false , false , true , false,false,true },
                {true , false , false , true , false,false,true },
                {true , false , false , true , false,false,true },
                 {true , false , false , true , false,false,true },
                {true , false , false , true , false ,false,true} });
            premia.SetChar('n' , new bool[8 , 4] {
                {false , false , false , false},
                {false,false,false,false},
                {false , true , true , false  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  } });
            premia.SetChar('o' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , false } });
            premia.SetChar('p' , new bool[8 , 4] {
                {false , false , false , false  },
                {false , false , false , false  },
                {false,true,true,false },
                {true , false , false , true},
                {true , false , false , true },
                {true , true , true , false },
                {true , false , false , false },
                {true , false , false , false } });
            premia.SetChar('q' , new bool[8 , 4] {
                {false , false , false , false  },
                 {false , false , false , false  },
                {false,true,true,false },
                {true , false , false , true  },
                {true , false , false , true  },
                {false , true , true , true  },
                {false , false , false , true  },
                {false , false , false , true  } });
            premia.SetChar('r' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , true , true , false },
                {true , true , false , false , true },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , false } });
            premia.SetChar('s' ,
            StringToChar(
                "     " ,
                "     " ,
                "     " ,
                " 1111" ,
                "1    " ,
                " 1111" ,
                "    1" ,
                "1111 "));
            premia.SetChar('t' , new bool[8 , 3] {
                {false , true , false },
                { false , true , false },
                {false , true , false },
                {true,true,true},
                {false , true , false  },
                {false , true , false  },
                {false , true , false  },
                {false , true , false  } });
            premia.SetChar('u' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , false } });
            premia.SetChar('v' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , false , true , false },
                {false , true , false , true , false },
                {false , false , true , false , false },
                {false , false , true , false , false } });
            premia.SetChar('w' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , true , false , true },
                {true , true , false , true , true },
                {true , false , false , false , true } });
            premia.SetChar('x' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , false , false , false , false },
                {true , false , false , false , true },
                {false , true , false , true , false },
                {false , false , true , false , false },
                {false , true , false , true , false },
                {true , false , false , false , true } });
            premia.SetChar('y' , new bool[8 , 4] {
                {false , false,false , false  },
                {false,false,false,false },
                {true , false,false , true  },
                {true , false,false , true  },
                {true , false,false , true  },
                {false , true,true , true  },
                {false , false,false , true  },
                {false , false , true ,false } });
            premia.SetChar('z' , new bool[8 , 4] {
                {false , false , false , false },
                {false,false,false,false },
                {true , true , true , true},
                {false , false , false , true },
                {false , false , true , false  },
                {false , true , false , false },
                {true , false , false , false  },
                {true , true , true , true  } });
            premia.SetChar('0' ,
                StringToChar(
                    " 111 " ,
                    "1  11" ,
                    "1  11" ,
                    "1 1 1" ,
                    "1 1 1" ,
                    "11  1" ,
                    "11  1" ,
                    " 111 "));
            premia.SetChar('1' ,
                StringToChar(
                    "11" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1"));
            premia.SetChar('2' ,
                StringToChar(
                    "1111" ,
                    "   1" ,
                    "   1" ,
                    "   1" ,
                    "1111" ,
                    "1   " ,
                    "1   " ,
                    "1111"));
            premia.SetChar('3' ,
               StringToChar(
                   "1111" ,
                   "   1" ,
                   "   1" ,
                   "   1" ,
                   " 111" ,
                   "   1" ,
                   "   1" ,
                   "1111"));
            premia.SetChar('4' ,
              StringToChar(
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1111" ,
                  "  1 " ,
                  "  1 "));
            premia.SetChar('5' ,
             StringToChar(
                 "1111" ,
                 "1   " ,
                 "1   " ,
                 "1   " ,
                 "1111" ,
                 "   1" ,
                 "   1" ,
                 "1111"));
            premia.SetChar('6' ,
             StringToChar(
                 "1111" ,
                 "1   " ,
                 "1   " ,
                 "1   " ,
                 "1111" ,
                 "1  1" ,
                 "1  1" ,
                 "1111"));
            premia.SetChar('7' ,
             StringToChar(
                 "11111" ,
                 "1   1" ,
                 "    1" ,
                 "   1 " ,
                 "   1 " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  "));
            premia.SetChar('8' ,
             StringToChar(
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "11111"));
            premia.SetChar('9' ,
             StringToChar(
                 "1111" ,
                 "1  1" ,
                 "1  1" ,
                 "1  1" ,
                 "1111" ,
                 "   1" ,
                 "   1" ,
                 "1111"));
            premia.SetChar('_' ,
             StringToChar(
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "11111111"));
            premia.SetChar('!' ,
             StringToChar(
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 " " ,
                 "1"));
            premia.SetChar('?' ,
             StringToChar(
                 " 111 " ,
                 "1   1" ,
                 "    1" ,
                 "   1 " ,
                 "  1  " ,
                 "  1  " ,
                 "     " ,
                 "  1  "));
            premia.SetChar('"' ,
             StringToChar(
                 "1 1" ,
                 "1 1" ,
                 "1 1" ,
                 "1 1"));
            premia.SetChar('(' ,
             StringToChar(
                 " 11" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 11"));
            premia.SetChar(')' ,
             StringToChar(
                 "11 " ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "11 "));
            premia.SetChar('|' ,
             StringToChar(
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1"));
            premia.SetChar('.' ,
             StringToChar(
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 "1"));
            premia.SetChar(',' ,
             StringToChar(
                 "  " ,
                 "  " ,
                 "  " ,
                 "  " ,
                 "  " ,
                 " 1" ,
                 " 1" ,
                 "1 "));
            premia.SetChar(':' ,
             StringToChar(
                 " " ,
                 " " ,
                 "1" ,
                 " " ,
                 " " ,
                 "1"));
            premia.SetChar(';' ,
             StringToChar(
                 "  " ,
                 "  " ,
                 " 1" ,
                 "  " ,
                 "  " ,
                 " 1" ,
                 " 1" ,
                 "1 "));
            premia.SetChar('$' ,
             StringToChar(
                 "  1  " ,
                 " 111 " ,
                 "1 1 1" ,
                 "111  " ,
                 "  111" ,
                 "1 1 1" ,
                 "11111" ,
                 "  1  "));
            premia.SetChar('[' ,
             StringToChar(
                 "111" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "111"));
            premia.SetChar(']' ,
             StringToChar(
                 "111" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "111"));

            premia.SetChar('<' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "  1" ,
                 " 1 " ,
                 "1  " ,
                 " 1 " ,
                 "  1"));
            premia.SetChar('>' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "1  " ,
                 " 1 " ,
                 "  1" ,
                 " 1 " ,
                 "1  "));

            premia.SetChar('{' ,
             StringToChar(
                 "  11" ,
                 " 1  " ,
                 " 1  " ,
                 "1   " ,
                 " 1  " ,
                 " 1  " ,
                 " 1  " ,
                 "  11"));
            premia.SetChar('}' ,
             StringToChar(
                 "11  " ,
                 "  1 " ,
                 "  1 " ,
                 "   1" ,
                 "  1 " ,
                 "  1 " ,
                 "  1 " ,
                 "11  "));
            premia.SetChar('(' ,
             StringToChar(
                 " 11" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 11"));
            premia.SetChar('%' ,
             StringToChar(
                 "11   1" ,
                 "11  1 " ,
                 "   1  " ,
                 "   1  " ,
                 "  1   " ,
                 "  1   " ,
                 " 1  11" ,
                 "1   11"));
            premia.SetChar('@' ,
             StringToChar(
                 " 1111 " ,
                 "1    1" ,
                 "1 11 1" ,
                 "1 1 11" ,
                 "1 1  1" ,
                 "1  11 " ,
                 "1     " ,
                 " 1111 "));
            premia.SetChar('-' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "   " ,
                 "111"));
            premia.SetChar('+' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "  1  " ,
                 "  1  " ,
                 "11111" ,
                 "  1  " ,
                 "  1  "));
            premia.SetChar('*' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "1 1 1" ,
                 " 111 " ,
                 "11111" ,
                 " 111 " ,
                 "1 1 1"));
            premia.SetChar('/' ,
             StringToChar(
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 "1  " ,
                 "1  "));
            premia.SetChar('\\' ,
             StringToChar(
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 "  1" ,
                 "  1"));
            premia.SetChar('^' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1"));
            premia.SetChar('#' ,
             StringToChar(
                 "     " ,
                 " 1 1 " ,
                 "11111" ,
                 " 1 1 " ,
                 "11111" ,
                 " 1 1 "));
            premia.SetChar('&' ,
             StringToChar(
                 " 11  " ,
                 "1  1 " ,
                 "1  1 " ,
                 " 11  " ,
                 "1 1 1" ,
                 "1  1 " ,
                 "1  11" ,
                 " 11 1"));
            premia.SetChar('=' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "     " ,
                 "11111" ,
                 "     " ,
                 "11111"));

            premia.SetChar('é' ,
             StringToChar(
                 "   11" ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            premia.SetChar('è' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            premia.SetChar('ê' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            premia.SetChar('ü' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('â' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            premia.SetChar('ä' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            premia.SetChar('à' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            premia.SetChar('ç' ,
             StringToChar(
                 "     " ,
                 " 111 " ,
                 "1    " ,
                 "1    " ,
                 "1    " ,
                 " 111 " ,
                 "  1  " ,
                 " 111 "));
            premia.SetChar('ë' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            premia.SetChar('ï' ,
             StringToChar(
                 "   " ,
                 "1 1" ,
                 "   " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 "));
            premia.SetChar('î' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  "));
            premia.SetChar('ì' ,
             StringToChar(
                 "11 " ,
                 "  1" ,
                 "   " ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1"));
            premia.SetChar('ô' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('ö' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('ò' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('ÿ' ,
             StringToChar(
                 "11 11" ,
                 "     " ,
                 "1   1" ,
                 " 1 1 " ,
                 "  1  " ,
                 "  1  " ,
                 " 1   " ,
                 "1    "));

            premia.SetChar('A' ,
             StringToChar(
                 "     " ,
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1"));
            premia.SetChar('B' ,
             StringToChar(
                 "     " ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1111 "));
            premia.SetChar('C' ,
             StringToChar(
                 "     " ,
                 " 1111" ,
                 "1   1" ,
                 "1    " ,
                 "1    " ,
                 "1    " ,
                 "1   1" ,
                 " 1111"));
            premia.SetChar('D' ,
             StringToChar(
                 "     " ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1111 "));
            premia.SetChar('E' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "1    " ,
                 "1    " ,
                 "1111 " ,
                 "1    " ,
                 "1    " ,
                 "11111"));
            premia.SetChar('F' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "1    " ,
                 "1    " ,
                 "1111 " ,
                 "1    " ,
                 "1    " ,
                 "1    "));
            premia.SetChar('G' ,
             StringToChar(
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1    " ,
                 "1 111" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('H' ,
              StringToChar(
                  "     " ,
                  "1   1" ,
                  "1   1" ,
                  "1   1" ,
                  "11111" ,
                  "1   1" ,
                  "1   1" ,
                  "1   1"));
            premia.SetChar('I' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "11111"));
            premia.SetChar('J' ,
             StringToChar(
                 "     " ,
                 "    1" ,
                 "    1" ,
                 "    1" ,
                 "    1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('K' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                "1  1 " ,
                "111  " ,
                "1  1 " ,
                "1   1" ,
                "1   1"));
            premia.SetChar('L' ,
            StringToChar(
                "     " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "11111"));
            premia.SetChar('M' ,
            StringToChar(
                "       " ,
                "1     1" ,
                "11   11" ,
                "1 1 1 1" ,
                "1  1  1" ,
                "1     1" ,
                "1     1" ,
                "1     1"));
            premia.SetChar('N' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "11  1" ,
                "11  1" ,
                "1 1 1" ,
                "1  11" ,
                "1  11" ,
                "1   1"));
            premia.SetChar('O' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                " 111 "));
            premia.SetChar('P' ,
            StringToChar(
                "     " ,
                "1111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "1    "));
            premia.SetChar('Q' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1 1 1" ,
                "1  11" ,
                " 1111"));
            premia.SetChar('R' ,
            StringToChar(
                "     " ,
                "1111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1  1 " ,
                "1   1" ,
                "1   1"));
            premia.SetChar('S' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1    " ,
                " 111 " ,
                "    1" ,
                "1   1" ,
                " 111 "));
            premia.SetChar('T' ,
            StringToChar(
                "     " ,
                "11111" ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  "));
            premia.SetChar('U' ,
             StringToChar(
                 "     " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            premia.SetChar('V' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                " 1 1 " ,
                " 1 1 " ,
                "  1  "));
            premia.SetChar('W' ,
            StringToChar(
                "         " ,
                "1   1   1" ,
                "1   1   1" ,
                "1   1   1" ,
                "1   1   1" ,
                " 1 1 1 1 " ,
                " 1 1 1 1 " ,
                "  1   1  "));
            premia.SetChar('X' ,
            StringToChar(
                "     " ,
                "1   1" ,
                " 1 1 " ,
                " 1 1 " ,
                "  1  " ,
                " 1 1 " ,
                " 1 1 " ,
                "1   1"));
            premia.SetChar('Y' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                " 1 1 " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  "));
            premia.SetChar('Z' ,
            StringToChar(
                "     " ,
                "11111" ,
                "    1" ,
                "   1 " ,
                "  1  " ,
                " 1   " ,
                "1    " ,
                "11111"));
            premia.SetChar('\'' ,
            StringToChar(
                "1" ,
                "1"));
            premia.SetChar('`' ,
           StringToChar(
               "1" ,
               "1"));
            premia.SetChar('~' ,
            StringToChar(
                "       " ,
                "       " ,
                "       " ,
                " 11  11" ,
                "1  11  "));

            premia.SetChar('Ç' ,
             StringToChar(
                "     " ,
                " 1111" ,
                "1   1" ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1   1" ,
                " 1111" ,
                "   1 " ,
                " 111 "));
            premia.SetChar('È' ,
            StringToChar(
                "111  " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            premia.SetChar('É' ,
            StringToChar(
                "  111" ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            premia.SetChar('Ë' ,
            StringToChar(
                " 1 1 " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            premia.SetChar('Ê' ,
            StringToChar(
                " 111 " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            premia.SetChar('À' ,
            StringToChar(
                "111  " ,
                "  1  " ,
                " 1 1 " ,
                "1   1" ,
                "11111" ,
                "1   1" ,
                "1   1" ,
                "1   1"));
        }

        private static Font premia;
        /// <summary>
        /// Premia FOnt
        /// </summary>
        public static Font Premia { get { return premia; } }

        /// <summary>
        /// Helper to translate string array to bool[,]
        /// </summary>
        /// <param name="entry">Any character except space is true</param>
        /// <returns></returns>
        public static bool[ , ] StringToChar(params string[ ] entry)
        {
            bool[,] character = new bool[entry.Length,entry[0].Length];
            for ( var i = 0 ; i < entry.Length ; i++ )
            {
                for ( var j = 0 ; j < entry[i].Length ; j++ )
                {
                    character[i , j] = entry[i][j] != ' ';
                }
            }
            return character;
        }
    }
}
