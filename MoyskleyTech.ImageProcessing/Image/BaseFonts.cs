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
            Premia = new Font("premia");
            Premia.SetChar(' ' , new bool[1 , 2]);
            Premia.SetChar('\t' , new bool[1 , 32]);
            Premia.SetChar('a' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false,true,true,true,false },
                {false,false,false,false,true },
                {false,true,true,true,true },
                {true,false,false,false,true },
                {true,false,false,false,true  },
                {false,true,true,true,true } });

            Premia.SetChar('b' , new bool[8 , 5] {
                {true,false,false,false,false },
                 {true,false,false,false,false },
                {true,false,false,false,false },
                 {true,true,true,true,false },
                 {true,false,false,false,true },
                 {true,false,false,false,true },
                {true,false,false,false,true },
                 {true,true,true,true,false } });

            Premia.SetChar('c' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , true },
                {false , true , true , true , false } });

            Premia.SetChar('d' , new bool[8 , 5] {
                {false , false , false , false , true },
                {false,false,false,false,true },
                {false , false , false , false , true },
                {false , true , true , true , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , true } });
            Premia.SetChar('e' ,
            StringToChar(
                "     " ,
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1    " ,
                " 1111"));
            Premia.SetChar('f' , new bool[8 , 3] {
                {false , false , false },
                {false,true,true },
                {true , false , false  },
                {true , true , true  },
                {true , false , false  },
                {true , false , false  },
                {true , false , false  },
                {true , false , false  } });
            Premia.SetChar('g' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , true },
                {false , false , false , false , true },
                {false , true , true , true , false } });
            Premia.SetChar('h' , new bool[8 , 4] {
                {true , false,false , false  },
                {true,false,false,false },
                {true , false,false , false  },
                {true , true,true , false  },
                {true , false,false , true },
                {true , false,false , true },
                {true , false,false , true },
                {true , false,false , true } });
            Premia.SetChar('i' , new bool[8 , 1] {
                {false  },
                {true },
                {false  },
                {true  },
                {true  },
                {true  },
                {true  },
                {true  } });
            Premia.SetChar('j' , new bool[8 , 4] {
                {false , false , false , true },
                {false,false,false,false },
                {false , false , true , true },
                {false , false , false , true  },
                {false , false , false , true  },
                {false , false , false , true  },
                {true , false , false , true  },
                {false , true , true , false  } });
            Premia.SetChar('k' , new bool[8 , 4] {
                {true , false , false , false },
                {true,false,false,false },
                {true , false , false , true  },
                {true , false , true , false  },
                {true , true , false , false  },
                {true , false , true , false  },
                {true , false , false , true  },
                {true , false , false , true  } });
            Premia.SetChar('l' , new bool[8 , 2] {
                {true , true  },
                {false,true},
                {false , true  },
                {false , true  },
                {false , true  },
                {false , true },
                {false , true  },
                {false , true  } });
            Premia.SetChar('m' , new bool[8 , 7] {
                {false , false , false , false , false,false,false },
                {false , false , false , false , false,false,false },
                {false , true , true , false , true,true,false },
                {true , false , false , true , false,false,true },
                {true , false , false , true , false,false,true },
                {true , false , false , true , false,false,true },
                 {true , false , false , true , false,false,true },
                {true , false , false , true , false ,false,true} });
            Premia.SetChar('n' , new bool[8 , 4] {
                {false , false , false , false},
                {false,false,false,false},
                {false , true , true , false  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  },
                {true , false , false , true  } });
            Premia.SetChar('o' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , true , true , true , false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , false } });
            Premia.SetChar('p' , new bool[8 , 4] {
                {false , false , false , false  },
                {false , false , false , false  },
                {false,true,true,false },
                {true , false , false , true},
                {true , false , false , true },
                {true , true , true , false },
                {true , false , false , false },
                {true , false , false , false } });
            Premia.SetChar('q' , new bool[8 , 4] {
                {false , false , false , false  },
                 {false , false , false , false  },
                {false,true,true,false },
                {true , false , false , true  },
                {true , false , false , true  },
                {false , true , true , true  },
                {false , false , false , true  },
                {false , false , false , true  } });
            Premia.SetChar('r' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , true , true , false },
                {true , true , false , false , true },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , false },
                {true , false , false , false , false } });
            Premia.SetChar('s' ,
            StringToChar(
                "     " ,
                "     " ,
                "     " ,
                " 1111" ,
                "1    " ,
                " 1111" ,
                "    1" ,
                "1111 "));
            Premia.SetChar('t' , new bool[8 , 3] {
                {false , true , false },
                { false , true , false },
                {false , true , false },
                {true,true,true},
                {false , true , false  },
                {false , true , false  },
                {false , true , false  },
                {false , true , false  } });
            Premia.SetChar('u' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , true , true , false } });
            Premia.SetChar('v' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {false , true , false , true , false },
                {false , true , false , true , false },
                {false , false , true , false , false },
                {false , false , true , false , false } });
            Premia.SetChar('w' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , false , false , true },
                {true , false , true , false , true },
                {true , true , false , true , true },
                {true , false , false , false , true } });
            Premia.SetChar('x' , new bool[8 , 5] {
                {false , false , false , false , false },
                {false,false,false,false,false },
                {false , false , false , false , false },
                {true , false , false , false , true },
                {false , true , false , true , false },
                {false , false , true , false , false },
                {false , true , false , true , false },
                {true , false , false , false , true } });
            Premia.SetChar('y' , new bool[8 , 4] {
                {false , false,false , false  },
                {false,false,false,false },
                {true , false,false , true  },
                {true , false,false , true  },
                {true , false,false , true  },
                {false , true,true , true  },
                {false , false,false , true  },
                {false , false , true ,false } });
            Premia.SetChar('z' , new bool[8 , 4] {
                {false , false , false , false },
                {false,false,false,false },
                {true , true , true , true},
                {false , false , false , true },
                {false , false , true , false  },
                {false , true , false , false },
                {true , false , false , false  },
                {true , true , true , true  } });
            Premia.SetChar('0' ,
                StringToChar(
                    " 111 " ,
                    "1  11" ,
                    "1  11" ,
                    "1 1 1" ,
                    "1 1 1" ,
                    "11  1" ,
                    "11  1" ,
                    " 111 "));
            Premia.SetChar('1' ,
                StringToChar(
                    "11" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1" ,
                    " 1"));
            Premia.SetChar('2' ,
                StringToChar(
                    "1111" ,
                    "   1" ,
                    "   1" ,
                    "   1" ,
                    "1111" ,
                    "1   " ,
                    "1   " ,
                    "1111"));
            Premia.SetChar('3' ,
               StringToChar(
                   "1111" ,
                   "   1" ,
                   "   1" ,
                   "   1" ,
                   " 111" ,
                   "   1" ,
                   "   1" ,
                   "1111"));
            Premia.SetChar('4' ,
              StringToChar(
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1 1 " ,
                  "1111" ,
                  "  1 " ,
                  "  1 "));
            Premia.SetChar('5' ,
             StringToChar(
                 "1111" ,
                 "1   " ,
                 "1   " ,
                 "1   " ,
                 "1111" ,
                 "   1" ,
                 "   1" ,
                 "1111"));
            Premia.SetChar('6' ,
             StringToChar(
                 "1111" ,
                 "1   " ,
                 "1   " ,
                 "1   " ,
                 "1111" ,
                 "1  1" ,
                 "1  1" ,
                 "1111"));
            Premia.SetChar('7' ,
             StringToChar(
                 "11111" ,
                 "1   1" ,
                 "    1" ,
                 "   1 " ,
                 "   1 " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  "));
            Premia.SetChar('8' ,
             StringToChar(
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "11111"));
            Premia.SetChar('9' ,
             StringToChar(
                 "1111" ,
                 "1  1" ,
                 "1  1" ,
                 "1  1" ,
                 "1111" ,
                 "   1" ,
                 "   1" ,
                 "1111"));
            Premia.SetChar('_' ,
             StringToChar(
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "        " ,
                 "11111111"));
            Premia.SetChar('!' ,
             StringToChar(
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 " " ,
                 "1"));
            Premia.SetChar('?' ,
             StringToChar(
                 " 111 " ,
                 "1   1" ,
                 "    1" ,
                 "   1 " ,
                 "  1  " ,
                 "  1  " ,
                 "     " ,
                 "  1  "));
            Premia.SetChar('"' ,
             StringToChar(
                 "1 1" ,
                 "1 1" ,
                 "1 1" ,
                 "1 1"));
            Premia.SetChar('(' ,
             StringToChar(
                 " 11" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 11"));
            Premia.SetChar(')' ,
             StringToChar(
                 "11 " ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "11 "));
            Premia.SetChar('|' ,
             StringToChar(
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1" ,
                 "1"));
            Premia.SetChar('.' ,
             StringToChar(
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 " " ,
                 "1"));
            Premia.SetChar(',' ,
             StringToChar(
                 "  " ,
                 "  " ,
                 "  " ,
                 "  " ,
                 "  " ,
                 " 1" ,
                 " 1" ,
                 "1 "));
            Premia.SetChar(':' ,
             StringToChar(
                 " " ,
                 " " ,
                 "1" ,
                 " " ,
                 " " ,
                 "1"));
            Premia.SetChar(';' ,
             StringToChar(
                 "  " ,
                 "  " ,
                 " 1" ,
                 "  " ,
                 "  " ,
                 " 1" ,
                 " 1" ,
                 "1 "));
            Premia.SetChar('$' ,
             StringToChar(
                 "  1  " ,
                 " 111 " ,
                 "1 1 1" ,
                 "111  " ,
                 "  111" ,
                 "1 1 1" ,
                 "11111" ,
                 "  1  "));
            Premia.SetChar('[' ,
             StringToChar(
                 "111" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "111"));
            Premia.SetChar(']' ,
             StringToChar(
                 "111" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "111"));

            Premia.SetChar('<' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "  1" ,
                 " 1 " ,
                 "1  " ,
                 " 1 " ,
                 "  1"));
            Premia.SetChar('>' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "1  " ,
                 " 1 " ,
                 "  1" ,
                 " 1 " ,
                 "1  "));

            Premia.SetChar('{' ,
             StringToChar(
                 "  11" ,
                 " 1  " ,
                 " 1  " ,
                 "1   " ,
                 " 1  " ,
                 " 1  " ,
                 " 1  " ,
                 "  11"));
            Premia.SetChar('}' ,
             StringToChar(
                 "11  " ,
                 "  1 " ,
                 "  1 " ,
                 "   1" ,
                 "  1 " ,
                 "  1 " ,
                 "  1 " ,
                 "11  "));
            Premia.SetChar('(' ,
             StringToChar(
                 " 11" ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 11"));
            Premia.SetChar('%' ,
             StringToChar(
                 "11   1" ,
                 "11  1 " ,
                 "   1  " ,
                 "   1  " ,
                 "  1   " ,
                 "  1   " ,
                 " 1  11" ,
                 "1   11"));
            Premia.SetChar('@' ,
             StringToChar(
                 " 1111 " ,
                 "1    1" ,
                 "1 11 1" ,
                 "1 1 11" ,
                 "1 1  1" ,
                 "1  11 " ,
                 "1     " ,
                 " 1111 "));
            Premia.SetChar('-' ,
             StringToChar(
                 "   " ,
                 "   " ,
                 "   " ,
                 "   " ,
                 "111"));
            Premia.SetChar('+' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "  1  " ,
                 "  1  " ,
                 "11111" ,
                 "  1  " ,
                 "  1  "));
            Premia.SetChar('*' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "1 1 1" ,
                 " 111 " ,
                 "11111" ,
                 " 111 " ,
                 "1 1 1"));
            Premia.SetChar('/' ,
             StringToChar(
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 "1  " ,
                 "1  "));
            Premia.SetChar('\\' ,
             StringToChar(
                 "1  " ,
                 "1  " ,
                 "1  " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 "  1" ,
                 "  1"));
            Premia.SetChar('^' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1"));
            Premia.SetChar('#' ,
             StringToChar(
                 "     " ,
                 " 1 1 " ,
                 "11111" ,
                 " 1 1 " ,
                 "11111" ,
                 " 1 1 "));
            Premia.SetChar('&' ,
             StringToChar(
                 " 11  " ,
                 "1  1 " ,
                 "1  1 " ,
                 " 11  " ,
                 "1 1 1" ,
                 "1  1 " ,
                 "1  11" ,
                 " 11 1"));
            Premia.SetChar('=' ,
             StringToChar(
                 "     " ,
                 "     " ,
                 "     " ,
                 "11111" ,
                 "     " ,
                 "11111"));

            Premia.SetChar('é' ,
             StringToChar(
                 "   11" ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            Premia.SetChar('è' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            Premia.SetChar('ê' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            Premia.SetChar('ü' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('â' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            Premia.SetChar('ä' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            Premia.SetChar('à' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "    1" ,
                 "11111" ,
                 "1   1" ,
                 " 1111"));
            Premia.SetChar('ç' ,
             StringToChar(
                 "     " ,
                 " 111 " ,
                 "1    " ,
                 "1    " ,
                 "1    " ,
                 " 111 " ,
                 "  1  " ,
                 " 111 "));
            Premia.SetChar('ë' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1111 " ,
                 "1    " ,
                 " 111 "));
            Premia.SetChar('ï' ,
             StringToChar(
                 "   " ,
                 "1 1" ,
                 "   " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 " ,
                 " 1 "));
            Premia.SetChar('î' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  "));
            Premia.SetChar('ì' ,
             StringToChar(
                 "11 " ,
                 "  1" ,
                 "   " ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1" ,
                 "  1"));
            Premia.SetChar('ô' ,
             StringToChar(
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('ö' ,
             StringToChar(
                 "     " ,
                 "11 11" ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('ò' ,
             StringToChar(
                 "11   " ,
                 "  1  " ,
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('ÿ' ,
             StringToChar(
                 "11 11" ,
                 "     " ,
                 "1   1" ,
                 " 1 1 " ,
                 "  1  " ,
                 "  1  " ,
                 " 1   " ,
                 "1    "));

            Premia.SetChar('A' ,
             StringToChar(
                 "     " ,
                 "  1  " ,
                 " 1 1 " ,
                 "1   1" ,
                 "11111" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1"));
            Premia.SetChar('B' ,
             StringToChar(
                 "     " ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1111 "));
            Premia.SetChar('C' ,
             StringToChar(
                 "     " ,
                 " 1111" ,
                 "1   1" ,
                 "1    " ,
                 "1    " ,
                 "1    " ,
                 "1   1" ,
                 " 1111"));
            Premia.SetChar('D' ,
             StringToChar(
                 "     " ,
                 "1111 " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1111 "));
            Premia.SetChar('E' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "1    " ,
                 "1    " ,
                 "1111 " ,
                 "1    " ,
                 "1    " ,
                 "11111"));
            Premia.SetChar('F' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "1    " ,
                 "1    " ,
                 "1111 " ,
                 "1    " ,
                 "1    " ,
                 "1    "));
            Premia.SetChar('G' ,
             StringToChar(
                 "     " ,
                 " 111 " ,
                 "1   1" ,
                 "1    " ,
                 "1 111" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('H' ,
              StringToChar(
                  "     " ,
                  "1   1" ,
                  "1   1" ,
                  "1   1" ,
                  "11111" ,
                  "1   1" ,
                  "1   1" ,
                  "1   1"));
            Premia.SetChar('I' ,
             StringToChar(
                 "     " ,
                 "11111" ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "  1  " ,
                 "11111"));
            Premia.SetChar('J' ,
             StringToChar(
                 "     " ,
                 "    1" ,
                 "    1" ,
                 "    1" ,
                 "    1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('K' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                "1  1 " ,
                "111  " ,
                "1  1 " ,
                "1   1" ,
                "1   1"));
            Premia.SetChar('L' ,
            StringToChar(
                "     " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "1    " ,
                "11111"));
            Premia.SetChar('M' ,
            StringToChar(
                "       " ,
                "1     1" ,
                "11   11" ,
                "1 1 1 1" ,
                "1  1  1" ,
                "1     1" ,
                "1     1" ,
                "1     1"));
            Premia.SetChar('N' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "11  1" ,
                "11  1" ,
                "1 1 1" ,
                "1  11" ,
                "1  11" ,
                "1   1"));
            Premia.SetChar('O' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                " 111 "));
            Premia.SetChar('P' ,
            StringToChar(
                "     " ,
                "1111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "1    "));
            Premia.SetChar('Q' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1 1 1" ,
                "1  11" ,
                " 1111"));
            Premia.SetChar('R' ,
            StringToChar(
                "     " ,
                "1111 " ,
                "1   1" ,
                "1   1" ,
                "1111 " ,
                "1  1 " ,
                "1   1" ,
                "1   1"));
            Premia.SetChar('S' ,
            StringToChar(
                "     " ,
                " 111 " ,
                "1   1" ,
                "1    " ,
                " 111 " ,
                "    1" ,
                "1   1" ,
                " 111 "));
            Premia.SetChar('T' ,
            StringToChar(
                "     " ,
                "11111" ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  "));
            Premia.SetChar('U' ,
             StringToChar(
                 "     " ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 "1   1" ,
                 " 111 "));
            Premia.SetChar('V' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                "1   1" ,
                " 1 1 " ,
                " 1 1 " ,
                "  1  "));
            Premia.SetChar('W' ,
            StringToChar(
                "         " ,
                "1   1   1" ,
                "1   1   1" ,
                "1   1   1" ,
                "1   1   1" ,
                " 1 1 1 1 " ,
                " 1 1 1 1 " ,
                "  1   1  "));
            Premia.SetChar('X' ,
            StringToChar(
                "     " ,
                "1   1" ,
                " 1 1 " ,
                " 1 1 " ,
                "  1  " ,
                " 1 1 " ,
                " 1 1 " ,
                "1   1"));
            Premia.SetChar('Y' ,
            StringToChar(
                "     " ,
                "1   1" ,
                "1   1" ,
                " 1 1 " ,
                "  1  " ,
                "  1  " ,
                "  1  " ,
                "  1  "));
            Premia.SetChar('Z' ,
            StringToChar(
                "     " ,
                "11111" ,
                "    1" ,
                "   1 " ,
                "  1  " ,
                " 1   " ,
                "1    " ,
                "11111"));
            Premia.SetChar('\'' ,
            StringToChar(
                "1" ,
                "1"));
            Premia.SetChar('`' ,
           StringToChar(
               "1" ,
               "1"));
            Premia.SetChar('~' ,
            StringToChar(
                "       " ,
                "       " ,
                "       " ,
                " 11  11" ,
                "1  11  "));

            Premia.SetChar('Ç' ,
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
            Premia.SetChar('È' ,
            StringToChar(
                "111  " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            Premia.SetChar('É' ,
            StringToChar(
                "  111" ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            Premia.SetChar('Ë' ,
            StringToChar(
                " 1 1 " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            Premia.SetChar('Ê' ,
            StringToChar(
                " 111 " ,
                "11111" ,
                "1    " ,
                "1    " ,
                "1111 " ,
                "1    " ,
                "1    " ,
                "11111"));
            Premia.SetChar('À' ,
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

        /// <summary>
        /// Premia FOnt
        /// </summary>
        public static Font Premia { get; private set; }

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
