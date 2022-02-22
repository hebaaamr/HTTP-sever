using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>

        //"GET / HTTP/1.1\r\nHost: 127.0.0.1\r\nConnection: keep-alive\r\nAccept: text/html\r\nUser-Agent: CSharpTests\r\n\r\n";
        public bool ParseRequest()
        {
            //throw new NotImplementedException();
            //string[] token = requestString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] token = requestString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int length = 0;
            string content = token[token.Length - 1];
            for (int i = token.Length - 1; i >= 1; i--)
            {
                if (token[i] == "")
                    continue;
                length += 1;

            }
            string[] hostHeader = new string[length];
            for (int i = 1; i < token.Length; i++)
            {
                if (token[i] == "")
                {
                    break;
                }
                else
                {

                    hostHeader[i - 1] = token[i];
                }

            }

            string blankLine = null;

            if (ValidateBlankLine(token) == true)
            {
                blankLine = "";
            }
            string headerLine = "";
            for (int i = 0; i < hostHeader.Length; i++)
            {
                headerLine += hostHeader[i];
            }
            string concat = token[0] + "\n" + headerLine + "\n" + blankLine;

            string[] requestLines = concat.Split('\n');
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length >= 3 && ParseRequestLine(requestLines[0]) == true && LoadHeaderLines(hostHeader) == true && ValidateBlankLine(token) == true)
            {
                return true;
            }
            // Parse Request line

            // Validate blank line exists

            // Load header lines into HeaderLines dictionary

            return false;
        }

        private bool ParseRequestLine(string requestLines)
        {
            //throw new NotImplementedException();
            string[] firstLine = requestLines.Split(' ');

            if (firstLine[0].Equals("GET"))
            {
                method = RequestMethod.GET;
            }

            else
                return false;
            if (firstLine[2].Equals("HTTP/1.0"))
            {
                httpVersion = HTTPVersion.HTTP10;
            }
            else if (firstLine[2].Equals("HTTP/1.1"))
            {
                httpVersion = HTTPVersion.HTTP11;

            }
            else if (firstLine[2].Equals("HTTP/0.9"))
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else
            {
                return false;
            }
            relativeURI = firstLine[1];
            return ValidateIsURI(relativeURI);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(string[] hostHeader)
        {
            //throw new NotImplementedException();
            for (int i = 0; i < hostHeader.Length; i++)
            {

                if (hostHeader[i].Contains(":"))
                {
                    string[] arr = hostHeader[i].Split(':');
                    string key = arr[0];
                    string value = arr[1];
                    headerLines.Add(key, value);
                }
                else
                    return false;
            }
            return true;
        }

        private bool ValidateBlankLine(string[] token)
        {
            //throw new NotImplementedException();
            if (token[token.Length - 2] == "")
            {
                return true;
            }
            return false;
        }

    }
}
