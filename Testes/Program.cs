using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Chama NovaThread em uma nova Thread
            new Thread(NovaThread).Start();
            // Chama NovaThread na thread principal

            void contar() { 
                for (int contador = 0; contador < 5; contador++)
                Console.Write('1');
             }

            contar();

            Console.ReadKey();
        }

        static void NovaThread()
        {
                for (int contador = 0; contador < 5; contador++)
                    Console.Write('1');
            }
        }
    }
