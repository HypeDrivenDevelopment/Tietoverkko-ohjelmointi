using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace teht8
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 9999);

            EndPoint pep = (IPEndPoint)ep;

            Laheta(palvelin, pep, "JOIN Samu");

            Boolean on = true;
            String TILA = "JOIN";
            while (on)
            {
                String[] palat = Vastaanota(palvelin);
                switch (TILA)
                {
                    case "JOIN":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    case "201":
                                        Console.WriteLine("Odotetaan toista pelaajaa");
                                        break;
                                    case "202":
                                        Console.WriteLine("Vastustajasi on {0}", palat[2]);
                                        Console.WriteLine("Anna numero");
                                        String numero = "";
                                        numero = Console.ReadLine();
                                        Laheta(palvelin, pep, "DATA " + numero);
                                        TILA = "GAME";
                                        break;
                                    case "203":
                                        Console.WriteLine("Vastustaja {0} aloittaa", palat[2]);
                                        TILA = "GAME";
                                        break;
                                    default:
                                        Console.WriteLine("Virhe1 " + palat[1]);
                                        break;
                                }
                                break;

                            default:
                                Console.WriteLine("Virhe2 " + palat[0]);
                                break;
                        }
                        break;
                    case "GAME":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    case "300":
                                        Console.WriteLine("Odotetaan toisen pelaajan arvausta");
                                        break;
                                    case "407":
                                        Console.WriteLine("Ei numero");
                                        Console.WriteLine("Anna numero");
                                        String luku = Console.ReadLine();
                                        Laheta(palvelin, pep, "DATA " + luku);
                                        break;
                                    default:
                                        Console.WriteLine("Virhe1 " + palat[1]);
                                        break;
                                }
                                break;

                            case "DATA":
                                Laheta(palvelin, pep, "ACK 300");
                                Console.WriteLine("Anna numero");
                                String numero = Console.ReadLine();
                                Laheta(palvelin, pep, "DATA " + numero);
                                break;

                            case "QUIT":
                                switch (palat[1])
                                {
                                    case "502":
                                        Console.WriteLine("Hävisit. {0}", palat[4]);
                                        Laheta(palvelin, pep, "ACK 500");
                                        on = false;
                                        break;
                                    case "501":
                                        Console.WriteLine("Voitit!");
                                        Laheta(palvelin, pep, "ACK 500");
                                        on = false;
                                        break;
                                    default:
                                        Console.WriteLine("Virhe1 " + palat[1]);
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }
            Console.ReadKey();
        }

        private static void Laheta(Socket palvelin, EndPoint pep, string msg)
        {
            palvelin.SendTo(Encoding.ASCII.GetBytes(msg), pep);
        }

        private static string[] Vastaanota(Socket palvelin)
        {
            byte[] rec = new byte[256];
            String[] palatvirhe = { "virhe", "virhe" };
            while (!Console.KeyAvailable)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Palvelinep = (EndPoint)remote;
                int paljon = 0;

                try
                {
                    paljon = palvelin.ReceiveFrom(rec, ref Palvelinep);
                    String viesti = Encoding.ASCII.GetString(rec, 0, paljon);
                    char[] delim = { ' ' };
                    String[] palat = viesti.Split(delim, 5);
                    Console.WriteLine("{0}: {1}", palat[0], palat[1]);
                    return palat;
                }
                catch
                {
                    return palatvirhe;
                }
                
            }
            return palatvirhe;
        }
    }
}
