using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;

        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            contentType = "Content-Type: " + contentType;
            int lengthOfContent = content.Length;
            DateTime todayDate = DateTime.Now;
            string stringLength = "Content-Length: " + lengthOfContent.ToString();
            string stringDate = "Date: " + todayDate.ToString();

            headerLines.Add(contentType);
            headerLines.Add(stringLength);
            headerLines.Add(stringDate);

            //lw al asm 2w al link at8yar
            if (redirectoinPath != "")
            {
                redirectoinPath = "Location: " + redirectoinPath;
                headerLines.Add(redirectoinPath);
            }

            // TODO: Create the request string
            string statusLine = GetStatusLine(code);
            string headerLine = "";
            for (int i = 0; i < headerLines.Count; i++)
            {
                headerLine += headerLines[i] + "\r\n";
            }

            responseString = statusLine + headerLine + "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            //string statusLine = string.Empty;
            string statusLine = Configuration.ServerHTTPVersion + ' ' + (int)code + ' '  + code.ToString() + "\r\n";
            return statusLine;
        }
    }
}
