using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ipEnd);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(100);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine(" client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] requestMessage;
                    requestMessage = new byte[1024];
                    int receivedLength = clientSocket.Receive(requestMessage);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    string message = Encoding.ASCII.GetString(requestMessage, 0, receivedLength);
                    Request requestObj = new Request(message);

                    // TODO: Call HandleRequest Method that returns the response
                    Response res = HandleRequest(requestObj);

                    // TODO: Send Response back to client
                    byte[] responseByte = Encoding.ASCII.GetBytes(res.ResponseString);
                    clientSocket.Send(responseByte);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket
            clientSocket.Close();
            Console.ReadLine();
        }

        Response HandleRequest(Request request)
        {
            Response response;
            StatusCode code;
            string contentType = "text/html";
            string content;
            string redirectoinPath = string.Empty;
            try
            {
                //TODO: check for bad request 
                bool goodRequest = request.ParseRequest();
                if (!goodRequest)
                {
                    code = StatusCode.BadRequest;
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    response = new Response(code, contentType, content, redirectoinPath);
                    return response;
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physicalPath = (request.relativeURI).Remove(0, 1);

                //TODO: check for redirect
                redirectoinPath = GetRedirectionPagePathIFExist(physicalPath);
                if (redirectoinPath != string.Empty)
                {
                    code = StatusCode.Redirect;
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    response = new Response(code, contentType, content, redirectoinPath);
                    return response;
                }

                //TODO: check file exists
                if (LoadDefaultPage(physicalPath) == string.Empty)
                {
                    code = StatusCode.NotFound;
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    response = new Response(code, contentType, content, redirectoinPath);
                    return response;
                }

                //TODO: read the physical file
                string physicalFileContent = LoadDefaultPage(physicalPath);
                //throw new NotImplementedException();
                // Create OK response
                code = StatusCode.OK;
                content = physicalFileContent;
                response = new Response(code, contentType, content, redirectoinPath);
                return response;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                code = StatusCode.InternalServerError;
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                response = new Response(code, contentType, content, redirectoinPath);
                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            else
                return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new FileNotFoundException("cannot find the file", filePath));
                return string.Empty;
            }

            // else read file and return its content
            else
                return File.ReadAllText(filePath);
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                IEnumerable<string> lines = File.ReadLines(filePath);
                foreach (string line in lines)
                {
                    string[] words = line.Split(',');
                    // then fill Configuration.RedirectionRules dictionary 
                    Configuration.RedirectionRules.Add(words[0], words[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
