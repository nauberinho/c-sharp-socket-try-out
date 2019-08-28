using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
  
// Socket Listener acts as a server and listens to the incoming   
// messages on the specified port and protocol.  
public class SocketListener  
{  
    public static int Main(String[] args)  
    {  
        StartClient();  
        return 0;  
    }  
  
     
    public static void StartServer()  
    {  
        // Get Host IP Address that is used to establish a connection  
        // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
        // If a host has multiple addresses, you will get a list of addresses  
        IPHostEntry host = Dns.GetHostEntry("localhost");  
        IPAddress ipAddress = host.AddressList[0];  
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);    
        
        try {   
            // Create a Socket that will use Tcp protocol      
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
            // A Socket must be associated with an endpoint using the Bind method  
            listener.Bind(localEndPoint);  
            listener.Listen(10);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Waiting for a connection...");

            Socket handler = listener.Accept();
  
            // Incoming data from the client.    
            string data = null;  
            byte[] bytes = null;
  
            while (true)  
            {  
                bytes = new byte[1024];  
                int bytesRec = handler.Receive(bytes);  
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {  
                    Console.WriteLine("Text received");
                    break;  
                }
                break;
            }
  
            byte[] msg = Encoding.ASCII.GetBytes("Hello from server");  
            handler.Send(msg);
            handler.Receive(bytes);
            Console.WriteLine("Text received from client: {0}", data);
            handler.Shutdown(SocketShutdown.Send);  
            handler.Close();
            
        }

        catch (Exception e)  
        {  
            Console.WriteLine(e.ToString());  
        }  
    }          

    public static void StartClient()  
    {  
        byte[] bytes = new byte[1024];  
  
        try  
        {  
            // Connect to a Remote server  
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
  
            // Create a TCP/IP  socket.    
            Socket sender = new Socket(ipAddress.AddressFamily,  
            SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect the socket to the remote endpoint. 
            try  
            { 
                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);  
  
                Console.WriteLine("Socket connected to {0}",  
                    sender.RemoteEndPoint.ToString());  
  
                   
                byte[] msg = Encoding.ASCII.GetBytes("This is a test");  
                int bytesSent = sender.Send(msg);  
  
                // Receive the response from the remote device.    
                int bytesRec = sender.Receive(bytes);  
                Console.WriteLine("Echoed test = {0}",  
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));  
                  
                while (true)  
                {  
                    
                    Console.WriteLine("\n Press any key to continue...");
                    while (Console.ReadKey().Key != ConsoleKey.Enter) {
                        // Encode the data string into a byte array. 
                        byte[] msg2 = Encoding.ASCII.GetBytes(Console.ReadKey().Key.ToString());
                        // Send the data through the socket. 
                        int bytesSent2 = sender.Send(msg2); 
                        Console.WriteLine(Console.ReadKey().Key);
                    }  
                    
                    break; 
                }

                // Release the socket.    
                sender.Shutdown(SocketShutdown.Both);  
                sender.Close();
  
            }
            
            catch (SocketException)  
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("No server exists, will start a server.");
                // No existing server - this instance will work as a server.
                StartServer();
            }  
  
        } 

        catch (Exception e)  
        {
            Console.WriteLine(e.ToString());
        }  
    }
}  
