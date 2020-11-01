using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace CHATSERVIDOR
{
    class Servidor_Chat
    {
        /* TcpListener:Esperando conexion del cliente*/
        private TcpListener servidor;

        /* TcpClient:Conexion entre el Servidor y el cliente*/
        private TcpClient cliente = new TcpClient();
        private IPEndPoint conexionip = new IPEndPoint(IPAddress.Any, 8000);/*almacena la direccion ip*/
        private List<Connection> list = new List<Connection>();/*Lista de estructura*/

        Connection con;

        private struct Connection
        {
            /*NetworkStream:Evia mensajes a traves del socket*/
            public NetworkStream Fdatos;
            public StreamReader Dleidos;
            public StreamWriter Descritos;
            public string usuario;
        }

        public Servidor_Chat()
        {
            Inicio();
        }

        public void Inicio()
        {
            Console.WriteLine("Servidor esta Funcionando");
            servidor = new TcpListener(conexionip);
            servidor.Start();/*El servidor se inicia*/

            while(true)
            {
                cliente = servidor.AcceptTcpClient();
                con = new Connection();
                con.Fdatos = cliente.GetStream();/*flujo de datos del cliente*/
                con.Dleidos = new StreamReader(con.Fdatos);
                con.Descritos = new StreamWriter(con.Fdatos);

                con.usuario = con.Dleidos.ReadLine();

                list.Add(con);
                Console.WriteLine(con.usuario + " se a conectado" );/*aparecera un mensaje*/

                Thread sesiones = new Thread(EscucharConexiones);
                sesiones.Start();
            }
        }
        public void EscucharConexiones()
        {
            Connection hcon = con;/*conexion para usar flujos de informacion*/
            do
            {
                try/*atrapa excepciones si se encuentra error*/
                {
                    string mensaje = hcon.Dleidos.ReadLine();
                    Console.WriteLine(hcon.usuario + ": " + mensaje);/*la consola mostrara el usuario que se conecto*/

                    foreach (Connection ce in list)
                    {
                        try
                        {
                            ce.Descritos.WriteLine(hcon.usuario + ": " + mensaje);
                            ce.Descritos.Flush();
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {
                    list.Remove(hcon);
                    Console.WriteLine("Se a Desconectado" + hcon.usuario);
                    break;
                }
            }
            while (true);
            {

            }
        }
    }
} 

